using System.Collections.Immutable;

namespace Interfaces
{
    public interface IRulesFactory
    {
        ImmutableDictionary<int, IPricingRule> GetImmutablePricingRulesForItems();
    }
}
