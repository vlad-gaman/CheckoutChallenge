namespace IntegrationTests.Api.Models
{
    public class PricingRule
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Type { get; set; }
        public string Data { get; set; }
    }
}
