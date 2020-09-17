using System;
using System.Threading.Tasks;

namespace Exercise2
{
    public class CalculationWorker
    {
        public void DoWork(int[] inputs, Func<int, long> mathOperation)
        {
            //Task.Factory.StartNew invokes an action in a new thread so that the calculation does not block the UI thread (otherwise the UI would hang)
            Task.Factory.StartNew(() =>
            {
                //TODO: do the calculations here and notify subscribers about each finished calculation
            });
        }
    }
}
