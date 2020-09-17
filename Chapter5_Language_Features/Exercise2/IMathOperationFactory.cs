using System;

namespace Exercise2
{
    public interface IMathOperationFactory
    {
        Func<int, long> CreateCubicOperation();
        Func<int, long> CreateNthPrimeOperation();
    }
}