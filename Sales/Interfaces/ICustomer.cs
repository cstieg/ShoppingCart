using System;
using System.Collections.Generic;

namespace Cstieg.Sales.Interfaces
{
    public interface ICustomer : ISalesEntity
    {
        string CustomerName { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        DateTime Registered { get; set; }

        DateTime LastVisited { get; set; }

        int TimesVisited { get; set; }

        string EmailAddress { get; set; }

        ICollection<IAddress> Addresses { get; set; }

    }
}
