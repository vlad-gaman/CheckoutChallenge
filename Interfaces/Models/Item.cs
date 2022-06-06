namespace Interfaces.Models
{
    public class Item
    {
        public int Id { get; }
        public string Sku { get; }
        public decimal UnitPrice { get; }

        public Item(int id, string sku, decimal unitPrice)
        {
            Id = id;
            Sku = sku;
            UnitPrice = unitPrice;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Item))
            {
                return false;
            }

            var other = obj as Item;
            return other.Id == Id;
        }
    }
}