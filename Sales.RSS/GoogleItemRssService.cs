using Cstieg.Sales.Interfaces;
using Cstieg.Sales.Models;
using Cstieg.Sales.Repositories;
using Cstieg.Sales.RSS.Exceptions;
using Cstieg.Sales.RSS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cstieg.Sales.RSS
{
    /// <summary>
    /// Service to create an RSS feed listing all the products in the store, to be consumed by Google Shopping
    /// </summary>
    public class GoogleItemRssService
    {
        private ISalesDbContext _context;
        private string _googleNameSpace = "http://base.google.com/ns/1.0";
        private string _rssVersion = "2.0";
        private Store _store;
        private string _baseUrl;
        private string _currency;
        private string _country;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Database context containing product information</param>
        public GoogleItemRssService(ISalesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initializes variables requiring async methods, not able to be initialized in constructor
        /// </summary>
        public async Task InitializeAsync()
        {
            _store = await _context.Stores.FirstAsync();
            _baseUrl = _store.BaseUrl;
            _currency = _store.Currency;
            _country = _store.Country;
        }

        /// <summary>
        /// Gets the RSS feed listing all the products in the store
        /// </summary>
        /// <returns>A MemoryStream containing the RSS text</returns>
        public async Task<Stream> GetRssAsync()
        {
            // Populate with info from Store table
            await InitializeAsync();

            // Set Google namespace
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("g", _googleNameSpace);

            // Serialize models to XML
            var serializer = new XmlSerializer(typeof(GoogleFeed));
            var stream = new MemoryStream();
            var feed = await GetGoogleFeedAsync();
            serializer.Serialize(stream, feed, namespaces);

            // Set stream position back to 0, so it will not be at EOS
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Gets the model of the RSS feed for Google shopping
        /// </summary>
        /// <returns>A model of an RSS feed containing all the products in the store</returns>
        public async Task<GoogleFeed> GetGoogleFeedAsync()
        {
            return new GoogleFeed()
            {
                RssVersion = _rssVersion,
                Channel = new GoogleChannel()
                {
                    Title = _store.Name,
                    Description = _store.Description,
                    Link = _baseUrl,
                    Items = await GetGoogleItemsAsync()
                }
            };
        }

        /// <summary>
        /// Gets the list of items in the store
        /// </summary>
        /// <returns>A list of item models for all the products in the store</returns>
        public async Task<List<GoogleItem>> GetGoogleItemsAsync()
        {
            var googleItems = new List<GoogleItem>();
            foreach (var product in await _context.Products.Where(p => !p.DoNotDisplay).ToListAsync())
            {
                try
                {
                    googleItems.Add(GetGoogleItem(product));
                }
                catch (Exception)
                {
                    // if a product fails validation, proceed to the next product
                }
            }
            return googleItems;
        }

        /// <summary>
        /// Gets the model in Google product form of a product in the store
        /// </summary>
        /// <param name="product">A product from the website store</param>
        /// <returns>The GoogleItem model containing the product info</returns>
        public GoogleItem GetGoogleItem(IProduct product)
        {
            var googleItem = new GoogleItem()
            {
                Id = product.Sku,
                Title = product.Name,
                Description = product.MetaDescription,
                Link = _baseUrl + "/Product/" + product.UrlName,
                ImageLink = _baseUrl + GetLargestImage(product.WebImages.First()),
                Availability = "in stock",
                Price = product.Price.ToString() + " " + _currency,
                Shipping = new List<GoogleShipping>(),
                Brand = product.Brand,
                Gtin = product.Gtin,
                Mpn = product.Sku,
                GoogleProductCategory = product.GoogleProductCategory,
                Condition = product.Condition
            };

            // get additional image links
            if (product.WebImages.Count > 1)
            {
                var webImages = product.WebImages.GetRange(1, product.WebImages.Count - 1);
                var additionalImageLinks = new List<string>();
                foreach (var webImage in webImages)
                {
                    additionalImageLinks.Add(_baseUrl + GetLargestImage(webImage));
                }
                googleItem.AdditionalImageLinks = additionalImageLinks;
            }

            // set shipping
            // add shipping info for default country
            if (product.ShippingScheme == null || product.ShippingScheme.ShippingCountries.Exists(s => s.Country.IsoCode2 == _country))
            {
                googleItem.Shipping.Add(new GoogleShipping()
                {
                    Country = _country,
                    Price = GetShippingPrice(product, _country).ToString() + " " + _currency
                });
            }
            // add shipping info for other countries in shipping scheme
            // exclude "--" for other countries
            if (product.ShippingScheme != null
                && product.ShippingScheme.ShippingCountries.Exists(s => s.Country.IsoCode2 != _country && s.Country.IsoCode2 != "--"))
            {
                foreach (var shippingCountry in product.ShippingScheme.ShippingCountries.FindAll(s => s.Country.IsoCode2 != _country
                     && s.Country.IsoCode2 != "--" && s.MinQty == null || s.MinQty == 1))
                {
                    googleItem.Shipping.Add(new GoogleShipping()
                    {
                        Country = shippingCountry.Country.IsoCode2,
                        Price = GetShippingPrice(product.Shipping, shippingCountry).ToString() + " " + _currency
                    });
                }
            }

            // validate model
            var context = new ValidationContext(googleItem);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(googleItem, context, results))
            {
                throw new InvalidModelException("Invalid item");
            }

            return googleItem;
        }

        /// <summary>
        /// Gets the shipping price from the shipping scheme for a product given a certain country
        /// </summary>
        /// <param name="product">The product model including a shipping scheme</param>
        /// <param name="_country">The country where the product will be shipped</param>
        /// <returns>The shipping price for the product to the given country</returns>
        private decimal GetShippingPrice(IProduct product, string country)
        {
            if (product.ShippingScheme != null)
            {
                var shippingCountry = product.ShippingScheme.ShippingCountries.Find(s => s.Country.IsoCode2 == country
                    && s.MinQty == null || s.MinQty == 1);
                return GetShippingPrice(product.Shipping, shippingCountry);
            }

            return product.Shipping;
        }

        /// <summary>
        /// Gets the shipping price from the shipping country rules and the base shipping price
        /// </summary>
        /// <param name="baseShipping">The base shipping price</param>
        /// <param name="shippingCountry">The model of shipping rules for a country</param>
        /// <returns></returns>
        private decimal GetShippingPrice(decimal baseShipping, ShippingCountry shippingCountry)
        {
            var shippingPrice = baseShipping;
            shippingPrice += shippingCountry.AdditionalShipping;
            if (shippingCountry.FreeShipping)
            {
                shippingPrice = 0.00M;
            }
            return shippingPrice;
        }

        /// <summary>
        /// Gets the URL of the largest image of a WebImage srcset.
        /// Based on the fact that the WebImage module puts the largest image URL first, and puts a space between the URL and the width
        /// </summary>
        /// <param name="webImage">The WebImage object</param>
        /// <returns>The URL of the largest image</returns>
        private string GetLargestImage(WebImage webImage)
        {
            int firstSpace = webImage.ImageSrcSet.IndexOf(" ");
            return webImage.ImageSrcSet.Substring(0, firstSpace);
        }

    }
}
