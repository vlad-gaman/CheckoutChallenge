using Interfaces;
using Interfaces.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CheckoutChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _itemsRepository;

        public ItemsController(IItemsRepository itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        /// <summary>
        /// Creates an item
        /// </summary>
        /// <param name="item">A new item</param>
        /// <returns>The item</returns>
        /// <response code="201">Item created</response>
        /// <response code="409">Item already exists with either the same id or sku</response>
        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public IActionResult CreateItem(ItemDto item)
        {
            if (!_itemsRepository.AddItem(item))
            {
                return Conflict();
            }

            return Created("", item);
        }

        /// <summary>
        /// Retrieves an item by id
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns>The item</returns>
        /// <response code="200">Item found</response>
        /// <response code="404">Item not found</response>
        [HttpGet("ById")]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetItemById(int id)
        {
            var item = _itemsRepository.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Retrieves an item by sku
        /// </summary>
        /// <param name="sku">Item sku</param>
        /// <returns>The item</returns>
        /// <response code="200">Item found</response>
        /// <response code="404">Item not found</response>
        [HttpGet("BySku")]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetItemBySku(string sku)
        {
            var item = _itemsRepository.GetItemBySku(sku);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Retrieves all items
        /// </summary>
        /// <returns>The items</returns>
        /// <response code="200">Items found</response>
        /// <response code="404">If inexistent list</response>
        [HttpGet("All")]
        [ProducesResponseType(typeof(List<ItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetAll()
        {
            var items = _itemsRepository.GetAllItems();
            if (items == null)
            {
                return NotFound();
            }

            return Ok(items.ToList());
        }

        /// <summary>
        /// Updates an items price by id
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="unitPrice">New unit price</param>
        /// <response code="200">Update successful</response>
        /// <response code="404">Item not found</response>
        [HttpPatch]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult UpdateItem(int id, decimal unitPrice)
        {
            var item = _itemsRepository.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }

            item.UnitPrice = unitPrice;
            _itemsRepository.UpdateItem(item);
            return Ok();
        }

        /// <summary>
        /// Deletes an item by id
        /// </summary>
        /// <param name="id">Item id</param>
        /// <response code="200">Update successful</response>
        /// <response code="404">Item not found</response>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult DeleteItem(int id)
        {
            var item = _itemsRepository.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }

            _itemsRepository.DeleteItem(item);
            return Ok();
        }
    }
}
