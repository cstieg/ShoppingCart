namespace Cstieg.Sales.RSS.Interfaces
{
    /// <summary>
    /// Interface for RSS Item
    /// </summary>
    public interface IItem
    {
        string Title { get; set; }

        string Link { get; set; }

        string Description { get; set; }
    }
}