namespace LinqExamples.Models
{
    public class AngleInfo
    {
        public double Angle { get; set; }
        public double Cosinus { get; set; }
        public double Sinus { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is AngleInfo other)) return false;

            return other.Angle.Equals(Angle) && other.Cosinus.Equals(Cosinus) && other.Sinus.Equals(Sinus);
        }
    }
}
