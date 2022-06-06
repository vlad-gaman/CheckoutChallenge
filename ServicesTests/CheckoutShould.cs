using AutoFixture;
using FluentAssertions;
using Interfaces;
using Interfaces.Dtos;
using Interfaces.Models;
using NSubstitute;
using Services;
using System;
using Xunit;

namespace ServicesTests
{
    public class CheckoutShould
    {
        private readonly Fixture _fixture;
        private readonly IPricingRules _pricingRules;
        private readonly IItemsRepository _itemsRepository;
        private readonly Checkout _service;

        public CheckoutShould()
        {
            _fixture = new Fixture();
            _pricingRules = Substitute.For<IPricingRules>();
            _itemsRepository = Substitute.For<IItemsRepository>();
            _service = new Checkout(_pricingRules, _itemsRepository);
        }

        [Fact]
        public void Scan_ByItemUpdatesTotalCorrectly()
        {
            //setup
            var item = _fixture.Create<Item>();
            var price = _fixture.Create<decimal>();
            _pricingRules.CalculatePrice(item).Returns(price);
            //execution
            _service.Scan(item);
            //validation
            var expected = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expected);
            //cleanup
        }

        [Fact]
        public void Scan_ByItemUpdatesTotalCorrectlyWithTwoDifferentItems()
        {
            //setup
            var item1 = _fixture.Create<Item>();
            var price1 = _fixture.Create<decimal>();
            var item2 = _fixture.Create<Item>();
            var price2 = _fixture.Create<decimal>();
            _pricingRules.CalculatePrice(item1).Returns(price1);
            _pricingRules.CalculatePrice(item2).Returns(price2);
            //execution
            _service.Scan(item1);
            _service.Scan(item2);
            //validation
            var expected = decimal.Round(price1 + price2, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expected);
            //cleanup
        }

        [Fact]
        public void Scan_ByItemUpdatesTotalCorrectlyWithSameItems()
        {
            //setup
            var item = _fixture.Create<Item>();
            var price = _fixture.Create<decimal>();
            _pricingRules.CalculatePrice(item).Returns(price);
            _pricingRules.CalculatePrice(item, 2).Returns(price * 2);
            //execution
            _service.Scan(item);
            _service.Scan(item);
            //validation
            var expected = decimal.Round(price * 2, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expected);
            //cleanup
        }

        [Theory]
        [InlineData(2.9999, 2.8888)]
        [InlineData(2.1111, 2.2222)]
        public void Scan_ByItemUpdatesRoundedTotalCorrectly(decimal price1, decimal price2)
        {
            //setup
            var item1 = _fixture.Create<Item>();
            var item2 = _fixture.Create<Item>();
            _pricingRules.CalculatePrice(item1).Returns(price1);
            _pricingRules.CalculatePrice(item2).Returns(price2);
            //execution
            _service.Scan(item1);
            var firstTotal = _service.Total;
            _service.Scan(item2);
            //validation
            var expectedFirstTotal = decimal.Round(price1, 2, MidpointRounding.AwayFromZero);
            firstTotal.Should().Be(expectedFirstTotal);
            var expectedSecondTotal = decimal.Round(price1 + price2, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expectedSecondTotal);
            //cleanup
        }

        [Fact]
        public void Scan_BySkuFails()
        {
            //setup
            var sku = _fixture.Create<string>();
            _itemsRepository.GetItemBySku(sku).Returns((ItemDto)null);
            //execution
            var result = _service.Scan(sku);
            //validation
            result.Should().BeFalse();
            //cleanup
        }

        [Fact]
        public void Scan_BySkuUpdatesTotalCorrectly()
        {
            //setup
            var sku = _fixture.Create<string>();
            var itemDto = _fixture.Create<ItemDto>();
            var price = _fixture.Create<decimal>();
            _itemsRepository.GetItemBySku(sku).Returns(itemDto);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto))).Returns(price);
            //execution
            var result = _service.Scan(sku);
            //validation
            result.Should().BeTrue();
            var expected = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expected);
            //cleanup
        }

        private static bool AreTheSame(Item i, ItemDto itemDto)
        {
            return i.Id == itemDto.Id && i.Sku == itemDto.Sku && i.UnitPrice == itemDto.UnitPrice;
        }

        [Fact]
        public void Scan_BySkuUpdatesTotalCorrectlyWithTwoDifferentItems()
        {
            //setup
            var sku1 = _fixture.Create<string>();
            var price1 = _fixture.Create<decimal>();
            var itemDto1 = _fixture.Create<ItemDto>();
            var sku2 = _fixture.Create<string>();
            var price2 = _fixture.Create<decimal>();
            var itemDto2 = _fixture.Create<ItemDto>();
            _itemsRepository.GetItemBySku(sku1).Returns(itemDto1);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto1))).Returns(price1);
            _itemsRepository.GetItemBySku(sku2).Returns(itemDto2);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto2))).Returns(price2);
            //execution
            var result1 = _service.Scan(sku1);
            var result2 = _service.Scan(sku2);
            //validation
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            var expected = decimal.Round(price1 + price2, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expected);
            //cleanup
        }

        [Fact]
        public void Scan_BySkuUpdatesTotalCorrectlyWithSameItems()
        {
            //setup
            var sku = _fixture.Create<string>();
            var price = _fixture.Create<decimal>();
            var itemDto = _fixture.Create<ItemDto>();
            _itemsRepository.GetItemBySku(sku).Returns(itemDto);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto))).Returns(price);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto)), 2).Returns(price * 2);
            //execution
            var result1 = _service.Scan(sku);
            var result2 = _service.Scan(sku);
            //validation
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            var expected = decimal.Round(price * 2, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expected);
            //cleanup
        }

        [Theory]
        [InlineData(2.9999, 2.8888)]
        [InlineData(2.1111, 2.2222)]
        public void Scan_BySkuUpdatesRoundedTotalCorrectly(decimal price1, decimal price2)
        {
            //setup
            var sku1 = _fixture.Create<string>();
            var sku2 = _fixture.Create<string>();
            var itemDto1 = _fixture.Create<ItemDto>();
            var itemDto2 = _fixture.Create<ItemDto>();
            _itemsRepository.GetItemBySku(sku1).Returns(itemDto1);
            _itemsRepository.GetItemBySku(sku2).Returns(itemDto2);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto1))).Returns(price1);
            _pricingRules.CalculatePrice(Arg.Is<Item>(item => AreTheSame(item, itemDto2))).Returns(price2);
            //execution
            var result1 = _service.Scan(sku1);
            var firstTotal = _service.Total;
            var result2 = _service.Scan(sku2);
            //validation
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            var expectedFirstTotal = decimal.Round(price1, 2, MidpointRounding.AwayFromZero);
            firstTotal.Should().Be(expectedFirstTotal);
            var expectedSecondTotal = decimal.Round(price1 + price2, 2, MidpointRounding.AwayFromZero);
            _service.Total.Should().Be(expectedSecondTotal);
            //cleanup
        }
    }
}
