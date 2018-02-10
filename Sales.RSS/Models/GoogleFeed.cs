using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cstieg.Sales.RSS.Models
{
    /// <summary>
    /// Model of RSS feed for Google Shopping
    /// </summary>
    [XmlRoot(ElementName = "rss")]
    public class GoogleFeed
    {
        [XmlAttribute(AttributeName = "version")]
        public string RssVersion { get; set; }

        [Required]
        [XmlElement("channel")]
        public GoogleChannel Channel { get; set; }
    }
}
