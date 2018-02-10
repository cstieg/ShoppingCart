using Cstieg.Sales.RSS.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cstieg.Sales.RSS.Models
{
    /// <summary>
    /// Model representing the Channel element of a Google Shopping RSS feed
    /// Child of GoogleFeed
    /// </summary>
    public class GoogleChannel : IChannel<GoogleItem>
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("item")]
        public List<GoogleItem> Items  { get; set; }
    }
}
