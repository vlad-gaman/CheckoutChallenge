using Interfaces.Models;

namespace Interfaces
{
    public interface ICheckout
    {
        void Scan(Item item);
        bool Scan(string sku);

        decimal Total { get; }
    }
}
