namespace Cstieg.Sales.Interfaces
{
    public interface IAddress
    {
        string Address1 { get; set; }

        string Address2 { get; set; }

        string City { get; set; }

        string State { get; set; }

        string PostalCode { get; set; }

        string Country { get; set; }

        bool IsSame(IAddress address);
    }
}
