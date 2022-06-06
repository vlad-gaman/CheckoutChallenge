using IntegrationTests.Api.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests.Api
{
    public interface IItemsApi
    {
        [Get("/api/Items/ById")]
        Task<Item> GetItemById(int id);
        [Get("/api/Items/BySku")]
        Task<Item> GetItemBySku(string sku);
        [Get("/api/Items/All")]
        Task<List<Item>> GetAllItems();
        [Post("/api/Items")]
        Task<Item> Create([Body] Item item);
        [Patch("/api/Items")]
        Task Update(int id, decimal unitPrice);
        [Delete("/api/Items")]
        Task Delete(int id);
    }
}
