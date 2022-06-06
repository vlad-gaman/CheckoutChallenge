using Interfaces;
using Interfaces.Models;

namespace Services.Rules
{
    public class MultiBuyRule : IPricingRule
    {
        public decimal MultipleUnitsPrice { get; }
        public int NumberOfUnitsForDiscount { get; }

        public MultiBuyRule(MultiBuyRuleData data)
        {
            MultipleUnitsPrice = data.MultipleUnitsPrice;
            NumberOfUnitsForDiscount = data.NumberOfUnitsForDiscount;
        }

        public decimal CalculatePrice(Item item, int numberOfEntries = 1)
        {
            if (numberOfEntries <= 0)
            {
                return 0;
            }

            var noOfNonAplicableItems = numberOfEntries % NumberOfUnitsForDiscount;
            var noOfBatchesOfApplicableItems = numberOfEntries / NumberOfUnitsForDiscount;

            return noOfBatchesOfApplicableItems * MultipleUnitsPrice + noOfNonAplicableItems * item.UnitPrice;
        }
    }
}