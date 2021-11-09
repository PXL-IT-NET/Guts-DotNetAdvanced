using System;

namespace LinqExamples.Models
{
    public class PersonSummary
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        /// <summary>
        /// True when under 18
        /// </summary>
        public bool IsChild { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is PersonSummary other)) return false;

            return other.Id.Equals(Id);
        }
    }
}