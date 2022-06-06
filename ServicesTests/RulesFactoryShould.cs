using AutoFixture;
using FluentAssertions;
using Interfaces;
using Interfaces.Dtos;
using Interfaces.Enums;
using Interfaces.Models;
using Newtonsoft.Json;
using NSubstitute;
using Services;
using Services.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ServicesTests
{
    public class RulesFactoryShould
    {
        private readonly Fixture _fixture;
        private readonly IPricingRulesRepository _repository;
        private readonly IServiceProvider _serviceProvider;
        private readonly RulesFactory _factory;

        public RulesFactoryShould()
        {
            _fixture = new Fixture();
            _repository = Substitute.For<IPricingRulesRepository>();
            _serviceProvider = Substitute.For<IServiceProvider>();
            _factory = new RulesFactory(_repository, _serviceProvider);
        }

        [Theory, MemberData(nameof(EmptyListOfPricingRuleDtos))]
        public void GetImmutablePricingRulesForItems_ReturnsEmptyDictionaryWhenThereAreNoPricingRules(IEnumerable<PricingRuleDto> pricingRules)
        {
            //setup
            _repository.GetAllPricingRules().Returns(pricingRules);
            //execution
            var dictionary = _factory.GetImmutablePricingRulesForItems();
            //validation
            dictionary.Should().BeEmpty();
            //cleanup
        }
        public static IEnumerable<object[]> EmptyListOfPricingRuleDtos()
        {
            return new List<object[]>
            {
                new object[] { null },
                new object[] { new List<PricingRuleDto>() }
            };
        }

        [Fact]
        public void GetImmutablePricingRulesForItems_ReturnsEmptyDictionaryWhenPricingRuleTypeDoesntExist()
        {
            //setup
            var rules = _fixture.CreateMany<PricingRuleDto>(1);
            rules.First().Type = (PricingRuleType)(-1);
            _repository.GetAllPricingRules().Returns(rules);
            //execution
            var dictionary = _factory.GetImmutablePricingRulesForItems();
            //validation
            dictionary.Should().BeEmpty();
            //cleanup
        }

        [Theory, MemberData(nameof(ListOfPricingRuleDtos))]
        public void GetImmutablePricingRulesForItems_ReturnsDictionaryWithCorrectValues(IEnumerable<PricingRuleDto> pricingRules, Type expectedType, Action<IPricingRule> validatePricingRuleData)
        {
            //setup
            _repository.GetAllPricingRules().Returns(pricingRules);
            //execution
            var dictionary = _factory.GetImmutablePricingRulesForItems();
            //validation
            dictionary.TryGetValue(pricingRules.First().ItemId, out var pricingRule).Should().BeTrue();
            pricingRule.Should().BeOfType(expectedType);
            validatePricingRuleData(pricingRule);
            //cleanup
        }

        public static IEnumerable<object[]> ListOfPricingRuleDtos()
        {
            var listOfParams = new List<object[]>();

            var fixture = new Fixture();
            var multiBuyRules = fixture.CreateMany<PricingRuleDto>(1).ToList();
            multiBuyRules[0].Type = PricingRuleType.MultiBuyRule;
            var multiBuyRuleData = fixture.Create<MultiBuyRuleData>();
            multiBuyRules[0].Data = JsonConvert.SerializeObject(multiBuyRuleData);
            listOfParams.Add(new object[] { multiBuyRules, typeof(MultiBuyRule),
                                            new Action<IPricingRule>((IPricingRule rule) =>
                                            {
                                                var multiBuyRule = rule as MultiBuyRule;
                                                multiBuyRule.MultipleUnitsPrice.Should().Be(multiBuyRuleData.MultipleUnitsPrice);
                                                multiBuyRule.NumberOfUnitsForDiscount.Should().Be(multiBuyRuleData.NumberOfUnitsForDiscount);
                                            }) });

            return listOfParams;
        }

        [Fact]
        public void GetImmutablePricingRulesForItems_ReturnsDictionaryCorrectlyByPrioritization()
        {
            //setup
            var multiBuyRuleData = _fixture.Create<MultiBuyRuleData>();
            var pricingRuleDto = new PricingRuleDto()
            {
                Id = _fixture.Create<int>(),
                ItemId = _fixture.Create<int>(),
                Type = PricingRuleType.MultiBuyRule,
                Data = JsonConvert.SerializeObject(multiBuyRuleData)
            };
            var pricingRules = new List<PricingRuleDto>() { pricingRuleDto, pricingRuleDto };            
            _repository.GetAllPricingRules().Returns(pricingRules);
            //execution
            var dictionary = _factory.GetImmutablePricingRulesForItems();
            //validation
            dictionary.Should().HaveCount(1);
            dictionary.TryGetValue(pricingRuleDto.ItemId, out var pricingRule).Should().BeTrue();
            pricingRule.Should().BeOfType<MultiBuyRule>();
            var multiBuyRule = pricingRule as MultiBuyRule;
            multiBuyRule.MultipleUnitsPrice.Should().Be(multiBuyRuleData.MultipleUnitsPrice);
            multiBuyRule.NumberOfUnitsForDiscount.Should().Be(multiBuyRuleData.NumberOfUnitsForDiscount);
            //cleanup
        }
    }
}
