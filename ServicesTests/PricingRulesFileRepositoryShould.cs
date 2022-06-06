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
    public class PricingRulesFileRepositoryShould
    {
        private readonly Fixture _fixture;
        private readonly IFileHandler<PricingRuleDto> _fileHandler;
        private readonly PricingRulesFileRepository _service;
        private readonly string _path;

        public PricingRulesFileRepositoryShould()
        {
            _fixture = new Fixture();
            var configuration = Substitute.For<IConfiguration>();
            _fileHandler = Substitute.For<IFileHandler<PricingRuleDto>>();
            _service = new PricingRulesFileRepository(configuration, _fileHandler);
            _path = _fixture.Create<string>();
            configuration[Constants.PricingRulesJsonFile].Returns(_path);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddPricingRule_ReturnsValue(bool value)
        {
            //setup
            var pricingRule = _fixture.Create<PricingRuleDto>();
            _fileHandler.AddObject(_path, pricingRule).Returns(value);
            //execution
            var result = _service.AddPricingRule(pricingRule);
            //validation
            result.Should().Be(value);
            //cleanup
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UpdatePricingRule_ReturnsValue(bool value)
        {
            //setup
            var pricingRule = _fixture.Create<PricingRuleDto>();
            _fileHandler.UpdateObject(_path, pricingRule).Returns(value);
            //execution
            var result = _service.UpdatePricingRule(pricingRule);
            //validation
            result.Should().Be(value);
            //cleanup
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeletePricingRule_ReturnsValue(bool value)
        {
            //setup
            var pricingRule = _fixture.Create<PricingRuleDto>();
            _fileHandler.DeleteObject(_path, pricingRule).Returns(value);
            //execution
            var result = _service.DeletePricingRule(pricingRule);
            //validation
            result.Should().Be(value);
            //cleanup
        }

        [Fact]
        public void GetAllPricingRules_ReturnsList()
        {
            //setup
            var pricingRules = _fixture.CreateMany<PricingRuleDto>();
            _fileHandler.GetAll(_path).Returns(pricingRules);
            //execution
            var result = _service.GetAllPricingRules();
            //validation
            result.Should().BeEquivalentTo(pricingRules);
            //cleanup
        }

        [Fact]
        public void GetPricingRuleById_ReturnsCorrectItemFromList()
        {
            //setup
            var pricingRules = _fixture.CreateMany<PricingRuleDto>(3).ToList();
            pricingRules[1].Id = pricingRules[2].Id;
            _fileHandler.GetAll(_path).Returns(pricingRules);
            //execution
            var result = _service.GetPricingRuleById(pricingRules[1].Id);
            //validation
            result.Should().BeSameAs(pricingRules[1]);
            //cleanup
        }
    }
}