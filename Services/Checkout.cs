using Interfaces;
using Interfaces.Models;
using System;
using System.Collections.Concurrent;

namespace Services
{
    public class Checkout : ICheckout
    {
        private readonly IPricingRules _pricingRules;
        private readonly IItemsRepository _itemsRepository;
        private readonly ConcurrentDictionary<Item, int> _basket;

        public Checkout(IPricingRules pricingRules, IItemsRepository itemsRepository)
        {
            _pricingRules = pricingRules;
            _itemsRepository = itemsRepository;
            _basket = new ConcurrentDictionary<Item, int>();
        }

        public decimal Total { get; private set; }

        public bool Scan(string sku)
        {
            var item = _itemsRepository.GetItemBySku(sku);
            if (item == null)
            {
                return false;
            }

            Scan(new Item(item.Id, item.Sku, item.UnitPrice));
            return true;
        }

        public void Scan(Item item)
        {
            _basket.AddOrUpdate(item, item => 1, (item, old) => old + 1);
            decimal currentTotal = 0;
            foreach (var existingItem in _basket.Keys)
            {
                _basket.TryGetValue(existingItem, out var noOfEntries);
                currentTotal += _pricingRules.CalculatePrice(existingItem, noOfEntries);
            }
            Total = decimal.Round(currentTotal, 2, MidpointRounding.AwayFromZero);
        }
    }
}
