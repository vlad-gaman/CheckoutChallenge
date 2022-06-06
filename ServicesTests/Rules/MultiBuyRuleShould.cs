using AutoFixture;
using FluentAssertions;
using Interfaces.Models;
using Services.Rules;
using ServicesTests.Helpers;
using Xunit;

namespace ServicesTests.Rules
{
    public class MultiBuyRuleShould
    {
        private readonly Fixture _fixture;

        public MultiBuyRuleShould()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CalculatePrice_WithRuleButItDoesntApply(int numberOfItems)
        {
            //setup
            var item = _fixture.Create<Item>();
            var data = _fixture.Create<MultiBuyRuleData>();
            data.NumberOfUnitsForDiscount = _fixture.CreateInt(3, 10);
            var rule = new MultiBuyRule(data);
            //execution
            var actual = rule.CalculatePrice(item, numberOfItems);
            //validation
            actual.Should().Be(item.UnitPrice * numberOfItems);
            //cleanup
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CalculatePrice_WithRuleButItApplies(int multiplier)
        {
            //setup
            var item = _fixture.Create<Item>();
            var data = _fixture.Create<MultiBuyRuleData>();
            data.NumberOfUnitsForDiscount = _fixture.CreateInt(3, 10);
            var rule = new MultiBuyRule(data);
            var numberOfItems = rule.NumberOfUnitsForDiscount * multiplier;
            //execution
            var actual = rule.CalculatePrice(item, numberOfItems);
            //validation
            actual.Should().Be(rule.MultipleUnitsPrice * multiplier);
            //cleanup
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CalculatePrice_WithRuleButItAppliesPartially(int multiplier)
        {
            //setup
            var item = _fixture.Create<Item>();
            var data = _fixture.Create<MultiBuyRuleData>();
            data.NumberOfUnitsForDiscount = _fixture.CreateInt(3, 10);
            var rule = new MultiBuyRule(data);
            var numberOfItems = rule.NumberOfUnitsForDiscount * multiplier + 1;
            //execution
            var actual = rule.CalculatePrice(item, numberOfItems);
            //validation
            actual.Should().Be(rule.MultipleUnitsPrice * multiplier + item.UnitPrice);
            //cleanup
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CalculatePrice_WithZeroOrNegativeEntriesThatShouldReturnZero(int numberOfEntries)
        {
            //setup
            var item = _fixture.Create<Item>();
            var data = _fixture.Create<MultiBuyRuleData>();
            data.NumberOfUnitsForDiscount = _fixture.CreateInt(3, 10);
            var rule = new MultiBuyRule(data);
            //execution
            var actual = rule.CalculatePrice(item, numberOfEntries);
            //validation
            actual.Should().Be(0);
            //cleanup
        }
    }
}
