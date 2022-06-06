using AutoFixture;
using FluentAssertions;
using IntegrationTests.Api;
using IntegrationTests.Api.Models;
using IntegrationTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class AcceptanceTests
    {
        private readonly ICheckoutApi _checkoutApi;
        private readonly IItemsApi _itemApi;
        private readonly IPricingRulesApi _pricingRuleApi;

        public AcceptanceTests()
        {
            _checkoutApi = ApiHelper.CreateApiClient<ICheckoutApi>();
            _itemApi = ApiHelper.CreateApiClient<IItemsApi>();
            _pricingRuleApi = ApiHelper.CreateApiClient<IPricingRulesApi>();
        }

        [Theory, MemberData(nameof(GenerateDataForNoDealsApplied)), MemberData(nameof(GenerateDataForDealsApplied))]
        public async Task NoDealsApplied_ReturnsCorrectAmount(List<Item> items, List<PricingRule> pricingRules, List<string> skus, decimal expectedTotal)
        {
            Guid guid = Guid.Empty;
            try
            {
                //setup
                foreach (var item in items)
                {
                    await _itemApi.Create(item);
                }
                foreach (var pricingRule in pricingRules)
                {
                    await _pricingRuleApi.Create(pricingRule);
                }

                //execution
                guid = await _checkoutApi.Start();
                foreach (var sku in skus)
                {
                    await _checkoutApi.Scan(guid, sku);
                }
                var total = await _checkoutApi.EndSession(guid);
                //validation
                total.Should().Be(expectedTotal);
            }
            finally
            {
                //cleanup
                await CleanUp(items, pricingRules, guid);
            }
        }

        private async Task CleanUp(List<Item> items, List<PricingRule> pricingRules, Guid guid)
        {
            try
            {
                await _checkoutApi.EndSession(guid);
            }
            catch (Exception) { }
            foreach (var pricingRule in pricingRules)
            {
                try
                {
                    await _pricingRuleApi.Delete(pricingRule.Id);
                }
                catch (Exception) { }
            }
            foreach (var item in items)
            {
                try
                {
                    await _itemApi.Delete(item.Id);
                }
                catch (Exception) { }
            }
        }

        public static IEnumerable<object[]> GenerateDataForNoDealsApplied()
        {
            var fixture = new Fixture();
            var list = new List<object[]>();

            Item itemA, itemB, itemC, itemD;
            var items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            var pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku }, 60 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemB.Sku }, 90 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemC.Sku, itemD.Sku, itemB.Sku, itemA.Sku }, 145 });


            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku }, 120 });

            return list;
        }

        private static List<Item> CreateItems(Fixture fixture, out Item itemA, out Item itemB, out Item itemC, out Item itemD)
        {
            var r = new Random();
            itemA = fixture.Create<Item>();
            itemA.Id = r.Next(100, 1000000);
            itemA.UnitPrice = 60;

            itemB = fixture.Create<Item>();
            itemB.Id = r.Next(100, 1000000);
            itemB.UnitPrice = 30;

            itemC = fixture.Create<Item>();
            itemC.Id = r.Next(100, 1000000);
            itemC.UnitPrice = 30;

            itemD = fixture.Create<Item>();
            itemD.Id = r.Next(100, 1000000);
            itemD.UnitPrice = 25;

            return new List<Item> { itemA, itemB, itemC, itemD };
        }

        private static List<PricingRule> CreatePricingRules(Fixture fixture, Item itemA, Item itemB)
        {
            var r = new Random();
            var pricingRuleA = fixture.Create<PricingRule>();
            pricingRuleA.Id = r.Next(100, 1000000);
            pricingRuleA.ItemId = itemA.Id;
            pricingRuleA.Type = 0;
            pricingRuleA.Data = $"{{\"MultipleUnitsPrice\":\"150\",\"NumberOfUnitsForDiscount\":\"3\"}}";

            var pricingRuleB = fixture.Create<PricingRule>();
            pricingRuleB.Id = r.Next(100, 1000000);
            pricingRuleB.ItemId = itemB.Id;
            pricingRuleB.Type = 0;
            pricingRuleB.Data = $"{{\"MultipleUnitsPrice\":\"45\",\"NumberOfUnitsForDiscount\":\"2\"}}";

            var pricingRule = new List<PricingRule> { pricingRuleA, pricingRuleB };
            return pricingRule;
        }

        public static IEnumerable<object[]> GenerateDataForDealsApplied()
        {
            var fixture = new Fixture();
            var list = new List<object[]>();

            Item itemA, itemB, itemC, itemD;
            var items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            var pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku }, 150 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku, itemA.Sku }, 210 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku, itemA.Sku, itemA.Sku }, 270 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku, itemA.Sku, itemA.Sku, itemA.Sku }, 300 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku, itemB.Sku }, 180 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku, itemB.Sku, itemB.Sku }, 195 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemA.Sku, itemA.Sku, itemA.Sku, itemB.Sku, itemB.Sku, itemD.Sku }, 220 });

            items = CreateItems(fixture, out itemA, out itemB, out itemC, out itemD);
            pricingRule = CreatePricingRules(fixture, itemA, itemB);
            list.Add(new object[] { items, pricingRule, new List<string> { itemD.Sku, itemA.Sku, itemB.Sku, itemA.Sku, itemB.Sku, itemA.Sku }, 220 });

            return list;
        }
    }
}
