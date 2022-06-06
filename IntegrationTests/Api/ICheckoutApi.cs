using Refit;
using System;
using System.Threading.Tasks;

namespace IntegrationTests.Api
{
    public interface ICheckoutApi
    {
        [Get("/api/Checkout")]
        Task CheckIfSessionAlive(Guid guid);
        [Get("/api/Checkout/Start")]
        Task<Guid> Start();
        [Patch("/api/Checkout/Scan")]
        Task<decimal> Scan(Guid guid, string sku);
        [Delete("/api/Checkout")]
        Task<decimal> EndSession(Guid guid);
    }
}
