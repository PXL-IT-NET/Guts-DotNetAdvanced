using System.Collections.Generic;

namespace LinqExamples.Models
{
    public class AnimalLoverCollection
    {
        public string Animal { get; set; }
        public IEnumerable<Person> Persons { get; set; }
    }
}