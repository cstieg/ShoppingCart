using Cstieg.Sales.Models;

namespace Cstieg.Sales.Interfaces
{
    public interface IPaymentProviderService
    {
        Customer GetCustomer();

        IAddress GetShippingAddress();

        IAddress GetBillingAddress();

        string GetCountryCode();

        Order GetOrder();

        string GetCartId();
    }
}
