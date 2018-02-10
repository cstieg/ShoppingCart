using Cstieg.Sales.Interfaces;

namespace Cstieg.Sales
{
    public interface IPaymentProviderService
    {
        ICustomer GetCustomer();

        IAddress GetShippingAddress();

        IAddress GetBillingAddress();

        string GetCountryCode();

        IOrder GetOrder();

        string GetCartId();
    }
}
