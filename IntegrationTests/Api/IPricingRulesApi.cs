using IntegrationTests.Api.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests.Api
{
    public interface IPricingRulesApi
    {
        [Get("/api/PricingRules")]
        Task<PricingRule> GetPricingRuleById(int id);
        [Get("/api/PricingRules/All")]
        Task<List<PricingRule>> GetAllPricingRules();
        [Post("/api/PricingRules")]
        Task<PricingRule> Create([Body] PricingRule pricingRule);
        [Patch("/api/PricingRules")]
        Task Update(int id, string data);
        [Delete("/api/PricingRules")]
        Task Delete(int id);
    }
}
