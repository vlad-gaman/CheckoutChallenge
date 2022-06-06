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
    public class ItemsApiTests
    {
        private readonly Fixture _fixture;
        private readonly IItemsApi _itemApi;

        public ItemsApiTests()
        {
            _fixture = new Fixture();
            _itemApi = ApiHelper.CreateApiClient<IItemsApi>();
        }

        [Fact]
        public async Task Create_WorksAsExpected()
        {
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                //execution
                await _itemApi.Create(item);
                //validation
                var actualItem = await _itemApi.GetItemById(item.Id);
                actualItem.Should().BeEquivalentTo(item);
            }
            finally
            {
                //cleanup
                await _itemApi.Delete(item?.Id ?? 0);
            }
        }

        [Fact]
        public async Task Create_ConflictsWhenAddingTheSameItemAgain()
        {
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                await _itemApi.Create(item);
                //execution
                Func<Task> action = () => _itemApi.Create(item);
                //validation
                await action.Should().ThrowAsync<ValidationApiException>()
                    .Where(a => a.StatusCode == System.Net.HttpStatusCode.Conflict);
            }
            finally
            {
                //cleanup
                await _itemApi.Delete(item?.Id ?? 0);
            }
        }

        [Fact]
        public async Task GetItemById_WhenThereIsOne()
        {
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                await _itemApi.Create(item);
                //execution
                var actualItem = await _itemApi.GetItemById(item.Id);
                //validation
                actualItem.Should().BeEquivalentTo(item);
            }
            finally
            {
                //cleanup
                await _itemApi.Delete(item?.Id ?? 0);
            }
        }

        [Fact]
        public async Task GetItemById_ReturnsNotFound()
        {
            //setup
            var id = _fixture.Create<int>();
            //execution
            Func<Task> action = () => _itemApi.GetItemById(id);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task GetItemBySku_WhenThereIsOne()
        {
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                await _itemApi.Create(item);
                //execution
                var actualItem = await _itemApi.GetItemBySku(item.Sku);
                //validation
                actualItem.Should().BeEquivalentTo(item);
            }
            finally
            {
                //cleanup
                await _itemApi.Delete(item?.Id ?? 0);
            }
        }

        [Fact]
        public async Task GetItemBySku_ReturnsNotFound()
        {
            //setup
            var sku = _fixture.Create<string>();
            //execution
            Func<Task> action = () => _itemApi.GetItemBySku(sku);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task GetAllItems_WorksAsExpected()
        {
            List<Item> list = new List<Item>();
            try
            {
                //setup
                list = _fixture.CreateMany<Item>(2).ToList();
                foreach (var item in list)
                {
                    await _itemApi.Create(item);
                }
                //execution
                var actualList = await _itemApi.GetAllItems();
                //validation
                actualList.FirstOrDefault(i => i.Id == list[0].Id).Should().BeEquivalentTo(list[0]);
                actualList.FirstOrDefault(i => i.Id == list[1].Id).Should().BeEquivalentTo(list[1]);
            }
            finally
            {
                //cleanup
                foreach (var item in list)
                {
                    await _itemApi.Delete(item.Id);
                }
            }
        }

        [Fact]
        public async Task UpdateItem_UpdatesExistingItem()
        {
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                var newUnitPrice = _fixture.Create<decimal>();
                await _itemApi.Create(item);
                var expecteItem = item;
                expecteItem.UnitPrice = newUnitPrice;
                //execution
                await _itemApi.Update(expecteItem.Id, newUnitPrice);
                //validation
                var actualItem = await _itemApi.GetItemById(expecteItem.Id);
                actualItem.Should().BeEquivalentTo(expecteItem);
            }
            finally
            {
                //cleanup
                await _itemApi.Delete(item?.Id ?? 0);
            }
        }

        [Fact]
        public async Task UpdateItem_ReturnsNotFound()
        {
            //setup
            var itemId = _fixture.Create<int>();
            var newUnitPrice = _fixture.Create<decimal>();
            //execution
            Func<Task> action = () => _itemApi.Update(itemId, newUnitPrice);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task DeleteItem_DeletesExistingItem()
        {
            Item item = null;
            //setup
            item = _fixture.Create<Item>();
            await _itemApi.Create(item);
            //execution
            await _itemApi.Delete(item.Id);
            //validation
            Func<Task> action = () => _itemApi.GetItemById(item.Id);
            await action.Should().ThrowAsync<ValidationApiException>()
                        .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task DeleteItem_ReturnsNotFound()
        {
            //setup
            var itemId = _fixture.Create<int>();
            //execution
            Func<Task> action = () => _itemApi.Delete(itemId);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }
    }
}
