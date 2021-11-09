using System;
using System.Collections.Generic;
using System.Linq;
using LinqExamples.Models;

namespace LinqExamples
{
    public class GroupExamples
    {
        //Tip: use the "ToList" extension method to convert an IEnumerable to a List

        public IList<IGrouping<bool, int>> GroupNegativeAndPositiveNumbers(int[] numbers)
        {
            //Tip: Zero can be interpreted as a positive number
            throw new NotImplementedException();
        }

        public IList<AnimalLoverCollection> GroupPersonsByFavoriteAnimal(List<Person> persons)
        {
            throw new NotImplementedException();
        }
    }
}