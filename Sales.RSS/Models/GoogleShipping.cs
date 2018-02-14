using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cstieg.Sales.RSS
{
    /// <summary>
    /// Model of shipping information in RSS feed for Google Shopping
    /// Child of GoogleItem
    /// </summary>
    public class GoogleShipping
    {
        /// <summary>
        /// ISO 3166 country code
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(2)]
        [XmlElement("country")]
        public string Country { get; set; }
        
        /// <summary>
        /// Service class or shipping speed
        /// </summary>
        [XmlElement("service")]
        public string Service { get; set; }

        /// <summary>
        /// Fixed shipping cost, including VAT if required
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [XmlElement("price")]
        public string Price { get; set; }
    }
}