using Cstieg.Sales.RSS.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cstieg.Sales.RSS
{
    /// <summary>
    /// Model of item in RSS feed for Google Shopping
    /// Child of GoogleChannel
    /// </summary>
    [XmlType(Namespace = "http://base.google.com/ns/1.0")]
    public class GoogleItem : IItem
    {
        /// <summary>
        /// Your product's unique identifier.  Ex. Sku
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// Your product's name
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(150)]
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// Your product's description.  
        /// Don't include ALL CAPS, foreign characters, or promotional text like "free shipping". Just description.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(5000)]
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Your product's landing page
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(2000)]
        [XmlElement("link")]
        public string Link { get; set; }

        /// <summary>
        /// The URL of your product's main image
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(2000)]
        [XmlElement("image_link")]
        public string ImageLink { get; set; }

        /// <summary>
        /// The URL of an additional image for your product
        /// </summary>
        [MaxLength(10)]
        [StringLength(2000)]
        [XmlElement("additional_image_link")]
        public List<string> AdditionalImageLinks { get; set; }
        public bool ShouldSerializeAdditionalImageLinks() { return AdditionalImageLinks != null && AdditionalImageLinks.Count > 0; }

        /// <summary>
        /// Your product's mobile-optimized landing page when you have a different URL for mobile and desktop traffic
        /// </summary>
        [StringLength(2000)]
        [XmlElement("mobile_link")]
        public string MobileLink { get; set; }
        public bool ShouldSerializeMobileLinks() { return MobileLink.Length > 0; }

        /// <summary>
        /// Your product's availability.  Supported values:
        /// "in stock", "out of stock", "preorder"
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [XmlElement("availability")]
        public string Availability { get; set; }

        /// <summary>
        /// The date a pre-ordered product becomes available for delivery
        /// </summary>
        [XmlElement("availability_date")]
        public DateTime? AvailabilityDate { get; set; }
        public bool ShouldSerializeAvailabilityDate() { return AvailabilityDate != null; }

        /// <summary>
        /// The date that your product should stop showing
        /// </summary>
        [XmlElement("expiration_date")]
        public DateTime? ExpirationDate { get; set; }
        public bool ShouldSerializeExpirationDate() { return ExpirationDate != null; }

        /// <summary>
        /// Your product's price with the currency. Example: 15.00 USD
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// Your product's sale price with the currency.  Example: 15.00 USD
        /// </summary>
        [XmlElement("sale_price")]
        public string SalePrice { get; set; }

        /// <summary>
        /// The date range during which the product's sale_price applies
        /// Example: 2016-02-24T11:07:31+0100 / 2016-02-29T23:07:31+0100
        /// </summary>
        [StringLength(51)]
        [XmlElement("sale_price_effective_date")]
        public string SalePriceEffectiveDate { get; set; }

        /// <summary>
        /// The measure and dimension of your product as it is sold.  Example: 1.5kg (numerical value + unit)
        /// </summary>
        [XmlElement("unit_pricing_measure")]
        public string UnitPricingMeasure { get; set; }

        /// <summary>
        /// The product's base measure for pricing (e.g. 100ml means the price is calculated based on a 100ml unit)
        /// </summary>
        [XmlElement("unit_pricing_base_measure")]
        public string UnitPricingBaseMeasure { get; set; }

        /// <summary>
        /// Details of an installment payment plan.  Example: 6, 50 BRL.  (Months, amount)
        /// </summary>
        [XmlElement("installment")]
        public string Installment { get; set; }

        /// <summary>
        /// Your product's shipping cost.  Supported prices: 0-1000 USD
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [XmlElement("shipping")]
        public List<GoogleShipping> Shipping { get; set; }

        /// <summary>
        /// Your product's sales tax rate in percent.
        /// Use this setting only to override the account tax settings for an individual item.
        /// Submit tax information for all your products using the account settings in Merchant Center.
        /// Example: US:CA:5.00:y  --- (rate - required; country (optional); region (optional); tax_ship (optional))
        /// </summary>
        [XmlElement("tax")]
        public string Tax { get; set; }

        /// <summary>
        /// Your product's brand name
        /// </summary>
        [XmlElement("brand")]
        [StringLength(70)]
        public string Brand { get; set; }

        /// <summary>
        /// Your product's Global Trade Item Number (UPC)
        /// </summary>
        [XmlElement("gtin")]
        [StringLength(14)]
        public string Gtin { get; set; }

        /// <summary>
        /// Your product's Manufacturer Part Number
        /// </summary>
        [XmlElement("mpn")]
        [StringLength(70)]
        public string Mpn { get; set; }

        /// <summary>
        /// To be used if your new product doesn't have a GTIN and brand or MPN and brand.  Supported values:
        /// "yes", "no"
        /// </summary>
        [XmlElement("identifier_exists")]
        [StringLength(3)]
        public string IdentifierExists { get; set; }

        /// <summary>
        /// Google-defined product category for your product.  
        /// Values from the Google product taxonomy: the numerical category id or the full path of the category.
        /// </summary>
        [XmlElement("google_product_category")]
        public string GoogleProductCategory { get; set; }

        /// <summary>
        /// Product category that you define for your product
        /// </summary>
        [XmlElement("product_type")]
        [StringLength(750)]
        public string ProductType { get; set; }

        /// <summary>
        /// Your product's condition.  Supported values:
        /// "new", "refurbished", "used"
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [XmlElement("condition")]
        public string Condition { get; set; }

        /// <summary>
        /// The number of identical products sold within a merchant-defined multipack.
        /// If the product's manufacturer assembled the multipack instead of you, don't submit this attribute.
        /// </summary>
        [XmlElement("multipack")]
        public int? MultiPack { get; set; }
        public bool ShouldSerializeMultiPack() { return MultiPack != null; }

        /// <summary>
        /// Indicates a product is a merchant-defined custom group of different products featuring one main product.  Yes or no.
        /// </summary>
        [XmlElement("is_bundle")]
        public bool IsBundle { get; set; }
        public bool ShouldSerializeIsBundle() { return IsBundle; }

        // Annnndd, there's more that I didn't take the time to reference. Add them later if necessary.
    }
}
