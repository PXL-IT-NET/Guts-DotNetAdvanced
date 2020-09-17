namespace LinqExamples.Models
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Person other)) return false;

            return other.Name.Equals(Name) && other.Age.Equals(Age);
        }
    }
}
