using Interfaces;
using Interfaces.Dtos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class PricingRulesFileRepository : IPricingRulesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IFileHandler<PricingRuleDto> _fileHandler;

        public PricingRulesFileRepository(IConfiguration configuration, IFileHandler<PricingRuleDto> fileHandler)
        {
            _configuration = configuration;
            _fileHandler = fileHandler;
        }

        public bool AddPricingRule(PricingRuleDto pricingRule)
        {
            var jsonFilePath = _configuration[Constants.PricingRulesJsonFile];
            return _fileHandler.AddObject(jsonFilePath, pricingRule);
        }

        public bool UpdatePricingRule(PricingRuleDto pricingRule)
        {
            var jsonFilePath = _configuration[Constants.PricingRulesJsonFile];
            return _fileHandler.UpdateObject(jsonFilePath, pricingRule);
        }

        public bool DeletePricingRule(PricingRuleDto pricingRule)
        {
            var jsonFilePath = _configuration[Constants.PricingRulesJsonFile];
            return _fileHandler.DeleteObject(jsonFilePath, pricingRule);
        }

        public IEnumerable<PricingRuleDto> GetAllPricingRules()
        {
            var jsonFilePath = _configuration[Constants.PricingRulesJsonFile];
            return _fileHandler.GetAll(jsonFilePath);
        }

        public PricingRuleDto GetPricingRuleById(int id)
        {
            return GetAllPricingRules().FirstOrDefault(pricingRule => pricingRule.Id == id);
        }
    }
}
