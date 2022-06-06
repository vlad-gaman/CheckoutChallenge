using Interfaces;
using Interfaces.Models;
using System.Collections.Immutable;

namespace Services
{
    public class PricingRules : IPricingRules
    {
        private readonly ImmutableDictionary<int, IPricingRule> _rules;

        public PricingRules(ImmutableDictionary<int, IPricingRule> rules)
        {
            _rules = rules;
        }

        public decimal CalculatePrice(Item item, int numberOfEntries = 1)
        {
            if (_rules.TryGetValue(item.Id, out var pricingRule))
            {
                return pricingRule.CalculatePrice(item, numberOfEntries);
            }

            return item.UnitPrice * numberOfEntries;
        }
    }
}
