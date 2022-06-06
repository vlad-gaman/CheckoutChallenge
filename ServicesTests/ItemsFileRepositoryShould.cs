using AutoFixture;
using FluentAssertions;
using Interfaces;
using Interfaces.Dtos;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Services;
using System.Linq;
using Xunit;

namespace ServicesTests
{
    public class ItemsFileRepositoryShould
    {
        private readonly Fixture _fixture;
        private readonly IFileHandler<ItemDto> _fileHandler;
        private readonly ItemsFileRepository _service;
        private readonly string _path;

        public ItemsFileRepositoryShould()
        {
            _fixture = new Fixture();
            var configuration = Substitute.For<IConfiguration>();
            _fileHandler = Substitute.For<IFileHandler<ItemDto>>();
            _service = new ItemsFileRepository(configuration, _fileHandler);
            _path = _fixture.Create<string>();
            configuration[Constants.ItemsJsonFile].Returns(_path);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddItem_ReturnsValue(bool value)
        {
            //setup
            var itemDto = _fixture.Create<ItemDto>();
            _fileHandler.AddObject(_path, itemDto).Returns(value);
            //execution
            var result = _service.AddItem(itemDto);
            //validation
            result.Should().Be(value);
            //cleanup
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UpdateItem_ReturnsValue(bool value)
        {
            //setup
            var itemDto = _fixture.Create<ItemDto>();
            _fileHandler.UpdateObject(_path, itemDto).Returns(value);
            //execution
            var result = _service.UpdateItem(itemDto);
            //validation
            result.Should().Be(value);
            //cleanup
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteItem_ReturnsValue(bool value)
        {
            //setup
            var itemDto = _fixture.Create<ItemDto>();
            _fileHandler.DeleteObject(_path, itemDto).Returns(value);
            //execution
            var result = _service.DeleteItem(itemDto);
            //validation
            result.Should().Be(value);
            //cleanup
        }

        [Fact]
        public void GetAllItems_ReturnsList()
        {
            //setup
            var itemDtos = _fixture.CreateMany<ItemDto>();
            _fileHandler.GetAll(_path).Returns(itemDtos);
            //execution
            var result = _service.GetAllItems();
            //validation
            result.Should().BeEquivalentTo(itemDtos);
            //cleanup
        }

        [Fact]
        public void GetItemById_ReturnsCorrectItemFromList()
        {
            //setup
            var itemDtos = _fixture.CreateMany<ItemDto>(3).ToList();
            itemDtos[1].Id = itemDtos[2].Id;
            _fileHandler.GetAll(_path).Returns(itemDtos);
            //execution
            var result = _service.GetItemById(itemDtos[1].Id);
            //validation
            result.Should().BeSameAs(itemDtos[1]);
            //cleanup
        }

        [Fact]
        public void GetItemBySku_ReturnsCorrectItemFromList()
        {
            //setup
            var itemDtos = _fixture.CreateMany<ItemDto>(3).ToList();
            itemDtos[1].Sku = itemDtos[2].Sku;
            _fileHandler.GetAll(_path).Returns(itemDtos);
            //execution
            var result = _service.GetItemBySku(itemDtos[1].Sku);
            //validation
            result.Should().BeSameAs(itemDtos[1]);
            //cleanup
        }
    }
}