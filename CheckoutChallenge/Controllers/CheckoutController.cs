using Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace CheckoutChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutFactory _checkoutFactory;

        public CheckoutController(ICheckoutFactory checkoutFactory)
        {
            _checkoutFactory = checkoutFactory;
        }

        /// <summary>
        /// Used as keep alive of the checkout session
        /// </summary>
        /// <param name="guid">Checkout identifier</param>
        /// <response code="200">Checkout session is alive</response>
        /// <response code="404">Checkout session is dead</response>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Get(Guid guid)
        {
            if (_checkoutFactory.GetCheckout(guid) == null)
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Starts a checkout session
        /// </summary>
        /// <response code="201">Checkout session Guid that is used as the identifier of the checkout</response>
        [HttpGet("Start")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        public IActionResult Start()
        {
            var guid = _checkoutFactory.CreateCheckout();
            return Created("", guid);
        }

        /// <summary>
        /// Scan a SKU in a checkout session
        /// </summary>
        /// <param name="guid">Checkout identifier</param>
        /// <param name="sku">A SKU</param>
        /// <returns>The total</returns>
        /// <response code="200">Scan was successful and return current total</response>
        /// <response code="404">Checkout session is dead</response>
        /// <response code="404">There is no such SKU, but returns current total</response>        
        [HttpPatch("Scan")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.NotFound)]
        public IActionResult Scan(Guid guid, string sku)
        {
            var checkout = _checkoutFactory.GetCheckout(guid);
            if (checkout == null)
            {
                return NotFound("No checkout session");
            }

            if (!checkout.Scan(sku))
            {
                return NotFound(checkout.Total);
            }

            return Ok(checkout.Total);
        }

        /// <summary>
        /// Ends checkout session
        /// </summary>
        /// <param name="guid">Checkout identifier</param>
        /// <returns>The total</returns>
        /// <response code="200">Ending checkout session was successful and returns total</response>
        /// <response code="404">Checkout session is dead</response>
        /// <response code="404">There is no such SKU, but returns current total</response>        
        [HttpDelete]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult EndSession(Guid guid)
        {
            var checkout = _checkoutFactory.RemoveCheckout(guid);
            if (checkout == null)
            {
                return NotFound();
            }

            return Ok(checkout.Total);
        }
    }
}
