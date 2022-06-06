using AutoFixture;
using FluentAssertions;
using Interfaces;
using Interfaces.Models;
using NSubstitute;
using Services;
using System.Collections.Immutable;
using Xunit;

namespace ServicesTests
{
    public class PricingRulesShould
    {
        private readonly Fixture _fixture;

        public PricingRulesShould()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CalculatePrice_WithoutRules(int numberOfItems)
        {
            //setup
            var builder = ImmutableDictionary.CreateBuilder<int, IPricingRule>();
            var pricingRules = new PricingRules(builder.ToImmutable());
            var item = _fixture.Create<Item>();
            //execution
            var actual = pricingRules.CalculatePrice(item, numberOfItems);
            //validation
            actual.Should().Be(item.UnitPrice * numberOfItems);
            //cleanup
        }

        [Theory]
        [InlineData(2.9999, 1)]
        [InlineData(2.1111, 2)]
        public void CalculatePrice_WithRulesButItDoesntApply(decimal price, int numberOfItems)
        {
            //setup
            var builder = ImmutableDictionary.CreateBuilder<int, IPricingRule>();
            var rule = Substitute.For<IPricingRule>();
            var item = new Item(_fixture.Create<int>(), _fixture.Create<string>(), price);
            builder.Add(item.Id + 1, rule);
            var pricingRules = new PricingRules(builder.ToImmutable());
            //execution
            var actual = pricingRules.CalculatePrice(item, numberOfItems);
            //validation
            actual.Should().Be(item.UnitPrice * numberOfItems);
            //cleanup
        }

        [Fact]
        public void CalculatePrice_WithRulesButItApplies()
        {
            //setup
            var builder = ImmutableDictionary.CreateBuilder<int, IPricingRule>();
            var rule = Substitute.For<IPricingRule>();
            var item = _fixture.Create<Item>();
            var numberOfEntries = _fixture.Create<int>();
            builder.Add(item.Id, rule);
            var pricingRules = new PricingRules(builder.ToImmutable());
            var rulePrice = _fixture.Create<decimal>();
            rule.CalculatePrice(item, numberOfEntries).Returns(rulePrice);
            //execution
            var actual = pricingRules.CalculatePrice(item, numberOfEntries);
            //validation
            actual.Should().Be(rulePrice);
            //cleanup
        }
    }
}
