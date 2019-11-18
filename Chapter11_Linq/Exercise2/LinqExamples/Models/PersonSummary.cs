namespace LinqExamples.Models
{
    public class PersonSummary
    {
        public string FullName { get; set; }
        public bool IsAdult { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is PersonSummary other)) return false;

            return other.FullName.Equals(FullName) && other.IsAdult.Equals(IsAdult);
        }
    }
}