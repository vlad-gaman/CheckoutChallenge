using Interfaces;
using Interfaces.Dtos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class ItemsFileRepository : IItemsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IFileHandler<ItemDto> _fileRepository;

        public ItemsFileRepository(IConfiguration configuration, IFileHandler<ItemDto> fileRepository)
        {
            _configuration = configuration;
            _fileRepository = fileRepository;
        }

        public bool AddItem(ItemDto item)
        {
            var jsonFilePath = _configuration[Constants.ItemsJsonFile];
            return _fileRepository.AddObject(jsonFilePath, item);
        }

        public bool UpdateItem(ItemDto item)
        {
            var jsonFilePath = _configuration[Constants.ItemsJsonFile];
            return _fileRepository.UpdateObject(jsonFilePath, item);
        }

        public bool DeleteItem(ItemDto item)
        {
            var jsonFilePath = _configuration[Constants.ItemsJsonFile];
            return _fileRepository.DeleteObject(jsonFilePath, item);
        }

        public IEnumerable<ItemDto> GetAllItems()
        {
            var jsonFilePath = _configuration[Constants.ItemsJsonFile];
            return _fileRepository.GetAll(jsonFilePath);
        }

        public ItemDto GetItemById(int id)
        {
            return GetAllItems().FirstOrDefault(item => item.Id == id);
        }

        public ItemDto GetItemBySku(string sku)
        {
            return GetAllItems().FirstOrDefault(item => item.Sku == sku);
        }
    }
}
