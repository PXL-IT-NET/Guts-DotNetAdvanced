using System;

namespace Exercise2
{
    public class CalculationEventArgs : EventArgs
    {
        public long Result { get; }
        public double ProgressPercentage { get; set; }

        public CalculationEventArgs(long result, double progress)
        {
            Result = result;
            ProgressPercentage = progress;
        }
    }
}