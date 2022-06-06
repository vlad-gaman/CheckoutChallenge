using AutoFixture;
using FluentAssertions;
using IntegrationTests.Api;
using IntegrationTests.Api.Models;
using IntegrationTests.Utilities;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class PricingRuleApiTests
    {
        private readonly Fixture _fixture;
        private readonly IPricingRulesApi _pricingRuleApi;

        public PricingRuleApiTests()
        {
            _fixture = new Fixture();
            _pricingRuleApi = ApiHelper.CreateApiClient<IPricingRulesApi>();
        }

        [Fact]
        public async Task Create_WorksAsExpected()
        {
            PricingRule pricingRule = null;
            try
            {
                //setup
                pricingRule = _fixture.Create<PricingRule>();
                //execution
                await _pricingRuleApi.Create(pricingRule);
                //validation
                var actualPricingRule = await _pricingRuleApi.GetPricingRuleById(pricingRule.Id);
                actualPricingRule.Should().BeEquivalentTo(pricingRule);
            }
            finally
            {
                //cleanup
                await _pricingRuleApi.Delete(pricingRule?.Id ?? 0);
            }
        }

        [Fact]
        public async Task Create_ConflictsWhenAddingTheSamePricingRuleAgain()
        {
            PricingRule pricingRule = null;
            try
            {
                //setup
                pricingRule = _fixture.Create<PricingRule>();
                await _pricingRuleApi.Create(pricingRule);
                //execution
                Func<Task> action = () => _pricingRuleApi.Create(pricingRule);
                //validation
                await action.Should().ThrowAsync<ValidationApiException>()
                    .Where(a => a.StatusCode == System.Net.HttpStatusCode.Conflict);
            }
            finally
            {
                //cleanup
                await _pricingRuleApi.Delete(pricingRule?.Id ?? 0);
            }
        }

        [Fact]
        public async Task GetPricingRulesById_WhenThereIsOne()
        {
            PricingRule pricingRule = null;
            try
            {
                //setup
                pricingRule = _fixture.Create<PricingRule>();
                await _pricingRuleApi.Create(pricingRule);
                //execution
                var actualPricingRule = await _pricingRuleApi.GetPricingRuleById(pricingRule.Id);
                //validation
                actualPricingRule.Should().BeEquivalentTo(pricingRule);
            }
            finally
            {
                //cleanup
                await _pricingRuleApi.Delete(pricingRule?.Id ?? 0);
            }
        }

        [Fact]
        public async Task GetPricingRuleById_ReturnsNotFound()
        {
            //setup
            var id = _fixture.Create<int>();
            //execution
            Func<Task> action = () => _pricingRuleApi.GetPricingRuleById(id);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task GetAllItems_WorksAsExpected()
        {
            List<PricingRule> list = new List<PricingRule>();
            try
            {
                //setup
                list = _fixture.CreateMany<PricingRule>(2).ToList();
                foreach (var pricingRule in list)
                {
                    await _pricingRuleApi.Create(pricingRule);
                }
                //execution
                var actualList = await _pricingRuleApi.GetAllPricingRules();
                //validation
                actualList.FirstOrDefault(i => i.Id == list[0].Id).Should().BeEquivalentTo(list[0]);
                actualList.FirstOrDefault(i => i.Id == list[1].Id).Should().BeEquivalentTo(list[1]);
            }
            finally
            {
                //cleanup
                foreach (var pricingRule in list)
                {
                    await _pricingRuleApi.Delete(pricingRule.Id);
                }
            }
        }

        [Fact]
        public async Task UpdateItem_UpdatesExistingItem()
        {
            PricingRule pricingRule = null;
            try
            {
                //setup
                pricingRule = _fixture.Create<PricingRule>();
                var newData = _fixture.Create<string>();
                await _pricingRuleApi.Create(pricingRule);
                var expectePricingRule = pricingRule;
                expectePricingRule.Data = newData;
                //execution
                await _pricingRuleApi.Update(expectePricingRule.Id, newData);
                //validation
                var actualPricingRule = await _pricingRuleApi.GetPricingRuleById(expectePricingRule.Id);
                actualPricingRule.Should().BeEquivalentTo(expectePricingRule);
            }
            finally
            {
                //cleanup
                await _pricingRuleApi.Delete(pricingRule?.Id ?? 0);
            }
        }

        [Fact]
        public async Task UpdateItem_ReturnsNotFound()
        {
            //setup
            var pricingRuleId = _fixture.Create<int>();
            var newData = _fixture.Create<string>();
            //execution
            Func<Task> action = () => _pricingRuleApi.Update(pricingRuleId, newData);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task DeleteItem_DeletesExistingItem()
        {
            PricingRule pricingRule = null;
            //setup
            pricingRule = _fixture.Create<PricingRule>();
            await _pricingRuleApi.Create(pricingRule);
            //execution
            await _pricingRuleApi.Delete(pricingRule.Id);
            //validation
            Func<Task> action = () => _pricingRuleApi.GetPricingRuleById(pricingRule.Id);
            await action.Should().ThrowAsync<ValidationApiException>()
                        .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task DeleteItem_ReturnsNotFound()
        {
            //setup
            var pricingRuleId = _fixture.Create<int>();
            //execution
            Func<Task> action = () => _pricingRuleApi.Delete(pricingRuleId);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }
    }
}
