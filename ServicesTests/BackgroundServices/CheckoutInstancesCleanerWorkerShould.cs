using AutoFixture;
using Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Services.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ServicesTests.BackgroundServices
{
    public class CheckoutInstancesCleanerWorkerShould
    {
        private readonly Fixture _fixture;
        private readonly ICheckoutFactory _checkoutFactory;
        private readonly IConfiguration _configuration;
        private readonly CheckoutInstancesCleanerWorker _worker;

        public CheckoutInstancesCleanerWorkerShould()
        {
            _fixture = new Fixture();
            _checkoutFactory = Substitute.For<ICheckoutFactory>();
            _configuration = Substitute.For<IConfiguration>();
            _worker = new CheckoutInstancesCleanerWorker(_checkoutFactory, _configuration);
        }

        [Fact]
        public async Task StartAsync_WillDoWorkAfterOneSecond()
        {
            //setup
            _configuration[Constants.CheckoutKeepAliveSeconds] = "1";
            var list = _fixture.CreateMany<(ICheckout, DateTime, Guid)>(0);
            _checkoutFactory.GetAllCheckoutsWithData().Returns(list);
            //execution
            await _worker.StartAsync(CancellationToken.None);
            // this is waiting for timer to kick in by using an arbitrary number
            await Task.Delay(1010);
            await _worker.StopAsync(CancellationToken.None);
            //validation
            _checkoutFactory.Received(1).GetAllCheckoutsWithData();
            //cleanup
        }

        [Fact]
        public async Task StartAsync_DoesntDoWorkRightAway()
        {
            //setup
            _configuration[Constants.CheckoutKeepAliveSeconds] = "100";
            var list = _fixture.CreateMany<(ICheckout, DateTime, Guid)>(0);
            _checkoutFactory.GetAllCheckoutsWithData().Returns(list);
            //execution
            await _worker.StartAsync(CancellationToken.None);
            // this is waiting for timer to kick in by using an arbitrary number
            await Task.Delay(10);
            await _worker.StopAsync(CancellationToken.None);
            //validation
            _checkoutFactory.Received(0).GetAllCheckoutsWithData();
            //cleanup
        }

        [Fact]
        public async Task StartAsync_DoesWorkByRemovingCheckoutWhereTimeHasSufficientlyPassed()
        {
            //setup
            _configuration[Constants.CheckoutKeepAliveSeconds] = "1";
            var checkout1 = Substitute.For<ICheckout>();
            var checkout2 = Substitute.For<ICheckout>();
            var list = new List<(ICheckout, DateTime, Guid)>()
            {
                (checkout1, DateTime.UtcNow.AddMinutes(-1), _fixture.Create<Guid>()),
                (checkout2, DateTime.UtcNow.AddMinutes(1), _fixture.Create<Guid>())
            };
            _checkoutFactory.GetAllCheckoutsWithData().Returns(list);
            //execution
            await _worker.StartAsync(CancellationToken.None);
            // this is waiting for timer to kick in by using an arbitrary number
            await Task.Delay(1010);
            await _worker.StopAsync(CancellationToken.None);
            //validation
            _checkoutFactory.Received(1).GetAllCheckoutsWithData();
            _checkoutFactory.Received(1).RemoveCheckout(list[0].Item3);
            //cleanup
        }
    }
}
