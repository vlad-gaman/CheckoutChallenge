namespace ServicesTests.Helpers
{
    public class TestObjectClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TestObjectClass))
            {
                return false;
            }

            var other = obj as TestObjectClass;
            return other.Id == Id;
        }
    }
}
