using System;
using System.Collections.Generic;

namespace Interfaces
{
    public interface ICheckoutFactory
    {
        Guid CreateCheckout();
        IEnumerable<(ICheckout checkout, DateTime lastModified, Guid guid)> GetAllCheckoutsWithData();
        ICheckout GetCheckout(Guid guid);
        ICheckout RemoveCheckout(Guid guid);
    }
}
