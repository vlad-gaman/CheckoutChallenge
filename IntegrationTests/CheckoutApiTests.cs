using AutoFixture;
using FluentAssertions;
using IntegrationTests.Api;
using IntegrationTests.Api.Models;
using IntegrationTests.Utilities;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class CheckoutApiTests
    {
        private readonly Fixture _fixture;
        private readonly ICheckoutApi _checkoutApi;
        private readonly IItemsApi _itemApi;

        public CheckoutApiTests()
        {
            _fixture = new Fixture();
            _checkoutApi = ApiHelper.CreateApiClient<ICheckoutApi>();
            _itemApi = ApiHelper.CreateApiClient<IItemsApi>();
        }

        [Fact]
        public async Task Start_WorksAsExpected()
        {
            Guid guid = Guid.Empty;
            try
            {
                //setup
                //execution
                guid = await _checkoutApi.Start();
                //validation
                Func<Task> action = () => _checkoutApi.CheckIfSessionAlive(guid);
                await action.Should().NotThrowAsync<ValidationApiException>();
            }
            finally
            {
                //cleanup
                await _checkoutApi.EndSession(guid);
            }
        }

        [Fact]
        public async Task CheckIfSessionAlive_ReturnNotFound()
        {
            //setup
            var guid = _fixture.Create<Guid>();
            //execution
            Func<Task> action = () => _checkoutApi.CheckIfSessionAlive(guid);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task CheckIfSessionAlive_WorksWhenThereIsASession()
        {
            Guid guid = Guid.Empty;
            try
            {
                //setup
                guid = await _checkoutApi.Start();
                //execution
                Func<Task> action = () => _checkoutApi.CheckIfSessionAlive(guid);
                //validation
                await action.Should().NotThrowAsync<ValidationApiException>();
            }
            finally
            {
                //cleanup
                await _checkoutApi.EndSession(guid);
            }
        }

        [Fact]
        public async Task EndSession_WorksAsExpected()
        {
            //setup
            var guid = await _checkoutApi.Start();
            //execution
            var result = await _checkoutApi.EndSession(guid);
            //validation
            result.Should().Be(0);
            //cleanup
        }

        [Fact]
        public async Task EndSession_ReturnsNotFoundWhenThereIsNoSession()
        {
            //setup
            var guid = _fixture.Create<Guid>();
            //execution
            Func<Task> action = () => _checkoutApi.EndSession(guid);
            //validation
            await action.Should().ThrowAsync<ValidationApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task EndSession_ReturnsCorrectData()
        {
            Guid guid = Guid.Empty;
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                await _itemApi.Create(item);
                guid = await _checkoutApi.Start();
                await _checkoutApi.Scan(guid, item.Sku);
                await _checkoutApi.Scan(guid, item.Sku);
                //execution
                var total = await _checkoutApi.EndSession(guid);
                //validation
                var expectedPrice = decimal.Round(item.UnitPrice * 2, 2, MidpointRounding.AwayFromZero);
                total.Should().Be(expectedPrice);
            }
            finally
            {
                //cleanup
                await _itemApi.Delete(item.Id);
            }
        }

        [Fact]
        public async Task Scan_ReturnsNotFoundBecauseThereIsNoSession()
        {
            //setup
            var guid = _fixture.Create<Guid>();
            var sku = _fixture.Create<string>();
            //execution
            Func<Task> action = () => _checkoutApi.Scan(guid, sku);
            //validation
            await action.Should().ThrowAsync<ApiException>()
                .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            //cleanup
        }

        [Fact]
        public async Task Scan_ReturnsNotFoundBecauseThereIsNoSku()
        {
            Guid guid = Guid.Empty;
            try
            {
                //setup
                var sku = _fixture.Create<string>();
                guid = await _checkoutApi.Start();
                //execution
                Func<Task> action = () => _checkoutApi.Scan(guid, sku);
                //validation
                await action.Should().ThrowAsync<ApiException>()
                    .Where(a => a.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
            finally
            {
                //cleanup
                await _checkoutApi.EndSession(guid);
            }
        }

        [Fact]
        public async Task Scan_ReturnsCorrectData()
        {
            Guid guid = Guid.Empty;
            Item item = null;
            try
            {
                //setup
                item = _fixture.Create<Item>();
                await _itemApi.Create(item);
                guid = await _checkoutApi.Start();
                //execution
                var total = await _checkoutApi.Scan(guid, item.Sku);
                //validation
                total.Should().Be(item.UnitPrice);
            }
            finally
            {
                //cleanup
                await _checkoutApi.EndSession(guid);
                await _itemApi.Delete(item.Id);
            }
        }
    }
}
