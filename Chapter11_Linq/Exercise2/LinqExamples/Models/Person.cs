namespace LinqExamples.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Firstname { get; set; }

        public string Lastname { get; set; }
        public int Age { get; set; }

        public string FavoriteAnimal { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Person other)) return false;

            return other.Id.Equals(Id);
        }
    }
}
