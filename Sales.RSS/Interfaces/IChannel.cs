using System.Collections.Generic;

namespace Cstieg.Sales.RSS.Interfaces
{
    /// <summary>
    /// Interface for RSS Channel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChannel<T> where T : IItem
    {
        string Title { get; set; }

        string Link { get; set; }

        string Description { get; set; }

        List<T> Items { get; set; }
    }
}
