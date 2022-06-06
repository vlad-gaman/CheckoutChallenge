using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Services.BackgroundServices
{
    public class CheckoutInstancesCleanerWorker : IHostedService, IDisposable
    {
        private readonly ICheckoutFactory _checkoutFactory;
        private readonly IConfiguration _configuration;
        private System.Timers.Timer _timer;

        public CheckoutInstancesCleanerWorker(ICheckoutFactory checkoutFactory, IConfiguration configuration)
        {
            _checkoutFactory = checkoutFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var seconds = GetSeconds();
            _timer = new System.Timers.Timer(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
            _timer.Elapsed += DoWork;
            _timer.Start();
            return Task.CompletedTask;
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            var seconds = GetSeconds();
            var now = DateTime.UtcNow.AddSeconds(seconds * -1);
            foreach (var tuple in _checkoutFactory.GetAllCheckoutsWithData())
            {
                if (tuple.lastModified.CompareTo(now) == -1)
                {
                    _checkoutFactory.RemoveCheckout(tuple.guid);
                }
            }
        }

        private int GetSeconds()
        {
            return int.Parse(_configuration[Constants.CheckoutKeepAliveSeconds]);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _timer?.Dispose();
        }
    }
}
