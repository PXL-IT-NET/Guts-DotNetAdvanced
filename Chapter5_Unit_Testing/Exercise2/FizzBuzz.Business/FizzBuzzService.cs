using System;
using System.Text;

namespace FizzBuzz.Business
{
    public class FizzBuzzService : IFizzBuzzService
    {
        public const int MinimumFactor = 2;
        public const int MaximumFactor = 10;
        public const int MinimumLastNumber = 1;
        public const int MaximumLastNumber = 250;

        public string GenerateFizzBuzzText(int fizzFactor, int buzzFactor, int lastNumber)
        {
            //TODO: correctly generate FizzBuzzText
            //Tip: use an instance of 'StringBuilder' to build the FizzBuzz text with good performance
            throw new NotImplementedException();
        }

        public void Validate(int fizzFactor, int buzzFactor, int lastNumber)
        {
            //TODO: throw FizzBuzzValidationException if input is invalid
            throw new NotImplementedException();
        }
    }
}