namespace Interfaces.Dtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemDto))
            {
                return false;
            }

            var other = obj as ItemDto;
            return other.Id == Id;
        }
    }
}
