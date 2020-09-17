using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FizzBuzz.Business
{
    public interface IFizzBuzzService
    {
        string GenerateFizzBuzzText(int fizzFactor, int buzzFactor, int lastNumber);
    }
}
