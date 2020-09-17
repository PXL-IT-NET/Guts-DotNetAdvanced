using System;

namespace FizzBuzz.Business
{
    public class FizzBuzzValidationException : ApplicationException
    {
        public FizzBuzzValidationException(string message) : base(message)
        {
        }
    }
}