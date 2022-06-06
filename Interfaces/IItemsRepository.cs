using Interfaces.Dtos;
using System.Collections.Generic;

namespace Interfaces
{
    public interface IItemsRepository
    {
        bool AddItem(ItemDto item);
        bool DeleteItem(ItemDto item);
        IEnumerable<ItemDto> GetAllItems();
        ItemDto GetItemById(int id);
        ItemDto GetItemBySku(string sku);
        bool UpdateItem(ItemDto item);
    }
}
