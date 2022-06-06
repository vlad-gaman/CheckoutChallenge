using Interfaces.Models;

namespace Interfaces
{
    public interface IPricingRules
    {
        decimal CalculatePrice(Item item, int numberOfEntries = 1);
    }
}