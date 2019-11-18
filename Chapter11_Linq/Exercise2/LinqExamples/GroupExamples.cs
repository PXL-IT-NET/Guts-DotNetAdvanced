using System;
using System.Collections.Generic;
using System.Linq;
using LinqExamples.Models;

namespace LinqExamples
{
    public class GroupExamples
    {
        //Tip: use the "ToList" extension method to convert an IEnumerable to a List

        public IList<IGrouping<bool, int>> GroupSmallAndBigNumbers(int[] numbers)
        {
            throw new NotImplementedException();
           
        }

        public IList<PersonAgeGroup> GroupPersonsByAge(List<Person> persons)
        {
            throw new NotImplementedException();
        }
    }
}