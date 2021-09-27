using System;

namespace Exercise3.OrderAggregate
{
    public class OrderEventArgs : EventArgs
    {
        public IOrder Order { get; }

        public OrderEventArgs(IOrder order)
        {
            Order = order;
        }
    }
}