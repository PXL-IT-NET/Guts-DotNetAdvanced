using System;
using System.Collections.Generic;
using System.Linq;
using LinqExamples.Models;

namespace LinqExamples
{
    public class GroupExamples
    {
        public IList<IGrouping<int, int>> GroupEvenAndOddNumbers(int[] numbers)
        {
            throw new NotImplementedException();
            //Tip: use the "ToList" extension method to convert an IEnumerable to a List
        }

        public IList<PersonAgeGroup> GroupPersonsByAge(List<Person> persons)
        {
            throw new NotImplementedException();
            //Tip: use the "ToList" extension method to convert an IEnumerable to a List
        }
    }
}