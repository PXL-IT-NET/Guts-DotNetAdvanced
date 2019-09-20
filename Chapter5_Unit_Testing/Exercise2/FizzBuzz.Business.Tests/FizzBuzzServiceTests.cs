using System;
using NUnit.Framework;

namespace FizzBuzz.Business.Tests
{
    public class FizzBuzzServiceTests
    {

        public void ReturnsCorrectFizzBuzzTextWhenParametersAreValid(int fizzFactor, int buzzFactor, int lastNumber, string expected)
        {
            Assert.Fail("Test not implemented yet");
        }

        public void ThrowsValidationExceptionWhenFizzFactorIsNotInRange(int fizzFactor)
        {
            Assert.Fail("Test not implemented yet");
        }

        public void ThrowsValidationExceptionWhenBuzzFactorIsNotInRange(int buzzFactor)
        {
            Assert.Fail("Test not implemented yet");
        }

        public void ThrowsValidationExceptionWhenLastNumberIsNotInRange(int lastNumber)
        {
            Assert.Fail("Test not implemented yet");
        }
    }
}
