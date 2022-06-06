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
    public class PricingRulesController : ControllerBase
    {
        private readonly IPricingRulesRepository _pricingRulesRepositry;

        public PricingRulesController(IPricingRulesRepository pricingRulesRepositry)
        {
            _pricingRulesRepositry = pricingRulesRepositry;
        }

        /// <summary>
        /// Retrieves a pricing rule by Id
        /// </summary>
        /// <param name="pricingRule">A new pricing rule</param>
        /// <returns>The pricing rule</returns>
        /// <response code="201">Pricing rule created</response>
        /// <response code="409">Pricing rule already exists with either the same id or sku</response>
        [HttpPost]
        [ProducesResponseType(typeof(PricingRuleDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public IActionResult CreateItem(PricingRuleDto pricingRule)
        {
            if (!_pricingRulesRepositry.AddPricingRule(pricingRule))
            {
                return Conflict();
            }

            return Created("", pricingRule);
        }

        /// <summary>
        /// Retrieves a pricing rule by id
        /// </summary>
        /// <param name="id">Pricing rule id</param>
        /// <returns>The pricing rule</returns>
        /// <response code="200">Pricing rule found</response>
        /// <response code="404">Pricing rule not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(PricingRuleDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetItemById(int id)
        {
            var pricingRule = _pricingRulesRepositry.GetPricingRuleById(id);
            if (pricingRule == null)
            {
                return NotFound();
            }

            return Ok(pricingRule);
        }

        /// <summary>
        /// Retrieves all pricing rules
        /// </summary>
        /// <returns>The pricing rules</returns>
        /// <response code="200">Pricing rules found</response>
        /// <response code="404">If inexistent list</response>
        [HttpGet("All")]
        [ProducesResponseType(typeof(List<PricingRuleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetAll()
        {
            var pricingRules = _pricingRulesRepositry.GetAllPricingRules();
            if (pricingRules == null)
            {
                return NotFound();
            }

            return Ok(pricingRules.ToList());
        }

        /// <summary>
        /// Updates a pricing rule data by id
        /// </summary>
        /// <param name="id">Pricing rule id</param>
        /// <param name="data">New data</param>
        /// <response code="200">Update successful</response>
        /// <response code="404">Pricing rule not found</response>
        [HttpPatch]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult UpdateItem(int id, string data)
        {
            var pricingRule = _pricingRulesRepositry.GetPricingRuleById(id);
            if (pricingRule == null)
            {
                return NotFound();
            }

            pricingRule.Data = data;
            _pricingRulesRepositry.UpdatePricingRule(pricingRule);
            return Ok();
        }

        /// <summary>
        /// Deletes a Pricing rule by id
        /// </summary>
        /// <param name="id">Pricing rule id</param>
        /// <response code="200">Update successful</response>
        /// <response code="404">Pricing rule not found</response>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult DeleteItem(int id)
        {
            var pricingRule = _pricingRulesRepositry.GetPricingRuleById(id);
            if (pricingRule == null)
            {
                return NotFound();
            }

            _pricingRulesRepositry.DeletePricingRule(pricingRule);
            return Ok();
        }
    }
}
