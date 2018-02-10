namespace Cstieg.Sales.Interfaces
{
    public interface ICountry : ISalesEntity
    {
        string Name { get; set; }

        string IsoCode2 { get; set; }
    }
}
