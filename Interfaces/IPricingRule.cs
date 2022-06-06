using Interfaces.Models;

namespace Interfaces
{
    public interface IPricingRule
    {
        decimal CalculatePrice(Item item, int numberOfEntries = 1);
    }
}
