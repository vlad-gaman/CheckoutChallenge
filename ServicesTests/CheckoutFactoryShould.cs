using AutoFixture;
using FluentAssertions;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Services;
using System;
using System.Linq;
using Xunit;

namespace ServicesTests
{
    public class CheckoutFactoryShould
    {
        private readonly Fixture _fixture;
        private readonly IServiceProvider _serviceProvider;
        private readonly CheckoutFactory _service;

        public CheckoutFactoryShould()
        {
            _fixture = new Fixture();
            _serviceProvider = Substitute.For<IServiceProvider>();
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            _serviceProvider.GetService<IServiceScopeFactory>().Returns(serviceScopeFactory);
            _service = new CheckoutFactory(_serviceProvider);
        }

        [Fact]
        public void CreateCheckout_WorksAsExpected()
        {
            //setup
            var scope = Substitute.For<IServiceScope>();            
            _serviceProvider.CreateScope().Returns(scope);
            var checkout = Substitute.For<ICheckout>();
            scope.ServiceProvider.GetService(typeof(ICheckout)).Returns(checkout);
            //execution
            var result = _service.CreateCheckout();
            //validation
            result.Should().NotBeEmpty();
            _service.GetCheckout(result).Should().Be(checkout);
            //cleanup
        }

        [Fact]
        public void GetCheckout_ReturnsNullWhenTheGuidIsNotValid()
        {
            //setup
            var guid = _fixture.Create<Guid>();
            //execution
            var result = _service.GetCheckout(guid);
            //validation
            result.Should().BeNull();
            //cleanup
        }

        [Fact]
        public void GetCheckout_ReturnsValue()
        {
            //setup
            var scope = Substitute.For<IServiceScope>();
            _serviceProvider.CreateScope().Returns(scope);
            var checkout = Substitute.For<ICheckout>();
            scope.ServiceProvider.GetService(typeof(ICheckout)).Returns(checkout);
            var guid = _service.CreateCheckout();
            //execution
            var result = _service.GetCheckout(guid);
            //validation
            result.Should().NotBeNull();
            result.Should().Be(checkout);
            //cleanup
        }

        [Fact]
        public void RemoveCheckout_ReturnsNullWhenTheGuidIsNotValid()
        {
            //setup
            var guid = _fixture.Create<Guid>();
            //execution
            var result = _service.RemoveCheckout(guid);
            //validation
            result.Should().BeNull();
            //cleanup
        }

        [Fact]
        public void RemoveCheckout_ReturnsValue()
        {
            //setup
            var scope = Substitute.For<IServiceScope>();
            _serviceProvider.CreateScope().Returns(scope);
            var checkout = Substitute.For<ICheckout>();
            scope.ServiceProvider.GetService(typeof(ICheckout)).Returns(checkout);
            var guid = _service.CreateCheckout();
            //execution
            var result = _service.RemoveCheckout(guid);
            //validation
            result.Should().NotBeNull();
            result.Should().Be(checkout);
            //cleanup
        }

        [Fact]
        public void GetAllCheckoutsWithLastModified()
        {
            //setup
            var scope = Substitute.For<IServiceScope>();
            _serviceProvider.CreateScope().Returns(scope);
            var checkout1 = Substitute.For<ICheckout>();
            var checkout2 = Substitute.For<ICheckout>();
            scope.ServiceProvider.GetService(typeof(ICheckout)).Returns(checkout1, checkout2);
            var guid1 = _service.CreateCheckout();
            var guid2 = _service.CreateCheckout();
            //execution
            var result = _service.GetAllCheckoutsWithData();
            //validation
            result.Should().HaveCount(2);
            var tuple1 = result.ToList()[0];
            var tuple2 = result.ToList()[1];

            tuple1.checkout.Should().Be(checkout1);
            tuple1.guid.Should().Be(guid1);

            tuple2.checkout.Should().Be(checkout2);
            tuple2.guid.Should().Be(guid2);
            //cleanup
        }
    }
}
