using System;

namespace Exercise2
{
    public class MathOperationFactory : IMathOperationFactory
    {
        public Func<int, long> CreateCubicOperation() //3*x³ + 2*x² + x
        {
            throw new NotImplementedException();
        }

        public Func<int, long> CreateNthPrimeOperation()
        {
            throw new NotImplementedException();
        }

        private bool IsPrime(long number)
        {
            for (long i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}