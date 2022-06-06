using Interfaces;
using Interfaces.Dtos;
using Interfaces.Enums;
using Interfaces.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Rules;
using System;
using System.Collections.Immutable;

namespace Services
{
    public class RulesFactory : IRulesFactory
    {
        private readonly IPricingRulesRepository _rulesRepository;
        private readonly IServiceProvider _serviceProvider;

        public RulesFactory(IPricingRulesRepository rulesRepository, IServiceProvider serviceProvider)
        {
            _rulesRepository = rulesRepository;
            _serviceProvider = serviceProvider;
        }

        public ImmutableDictionary<int, IPricingRule> GetImmutablePricingRulesForItems()
        {
            var rules = _rulesRepository.GetAllPricingRules();
            if (rules == null)
            {
                return ImmutableDictionary<int, IPricingRule>.Empty;
            }
            var builder = ImmutableDictionary.CreateBuilder<int, IPricingRule>();
            foreach (var rule in rules)
            {
                var pricingRule = CreatePricingRule(rule);
                PrioritizeRule(builder, rule, pricingRule);
            }

            return builder.ToImmutable();
        }

        private static void PrioritizeRule(ImmutableDictionary<int, IPricingRule>.Builder builder, PricingRuleDto rule, IPricingRule pricingRule)
        {
            if (pricingRule != null && !builder.ContainsKey(rule.ItemId))
            {
                builder.Add(rule.ItemId, pricingRule);
            }
        }

        private IPricingRule CreatePricingRule(PricingRuleDto pricingRuleDto)
        {
            switch (pricingRuleDto.Type)
            {
                case PricingRuleType.MultiBuyRule:
                    var data = JsonConvert.DeserializeObject<MultiBuyRuleData>(pricingRuleDto.Data);
                    return ActivatorUtilities.CreateInstance<MultiBuyRule>(_serviceProvider, data);
                default:
                    return null;
            }
        }
    }
}
