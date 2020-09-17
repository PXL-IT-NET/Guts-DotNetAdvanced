using System.Collections.Generic;

namespace LinqExamples.Models
{
    public class PersonAgeGroup
    {
        public int Age { get; set; }
        public IEnumerable<Person> Persons { get; set; }
    }
}