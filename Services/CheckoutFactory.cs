using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Services
{
    public class CheckoutFactory : ICheckoutFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Guid, (ICheckout checkout, DateTime lastModified, Guid guid)> _sessions;

        public CheckoutFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _sessions = new ConcurrentDictionary<Guid, (ICheckout checkout, DateTime lastModified, Guid guid)>();
        }

        public Guid CreateCheckout()
        {
            var guid = Guid.NewGuid();
            ICheckout checkout;
            using (var scope = _serviceProvider.CreateScope())
            {
                checkout = ActivatorUtilities.GetServiceOrCreateInstance<ICheckout>(scope.ServiceProvider);
            }
            var lastModified = DateTime.UtcNow;
            _sessions.TryAdd(guid, (checkout, lastModified, guid));

            return guid;
        }

        public ICheckout GetCheckout(Guid guid)
        {
            if (_sessions.TryGetValue(guid, out var tuple))
            {
                tuple.lastModified = DateTime.UtcNow;
                return tuple.checkout;
            }

            return null;
        }

        public ICheckout RemoveCheckout(Guid guid)
        {
            if (_sessions.TryRemove(guid, out var tuple))
            {
                return tuple.checkout;
            }
            return null;
        }

        public IEnumerable<(ICheckout checkout, DateTime lastModified, Guid guid)> GetAllCheckoutsWithData()
        {
            return _sessions.Values;
        }
    }
}
