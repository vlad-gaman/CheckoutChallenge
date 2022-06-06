using Interfaces.Dtos;
using System.Collections.Generic;

namespace Interfaces
{
    public interface IPricingRulesRepository
    {
        bool AddPricingRule(PricingRuleDto pricingRule);
        bool UpdatePricingRule(PricingRuleDto pricingRule);
        bool DeletePricingRule(PricingRuleDto pricingRule);
        IEnumerable<PricingRuleDto> GetAllPricingRules();
        PricingRuleDto GetPricingRuleById(int id);
    }
}
