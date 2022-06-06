using Interfaces.Enums;

namespace Interfaces.Dtos
{
    public class PricingRuleDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public PricingRuleType Type { get; set; }
        public string Data { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PricingRuleDto))
            {
                return false;
            }

            var other = obj as PricingRuleDto;
            return other.Id == Id;
        }
    }
}
