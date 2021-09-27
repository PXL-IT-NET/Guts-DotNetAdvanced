namespace Exercise3.OrderAggregate
{
    public interface IOrder
    {
        OrderNumber Number { get; set; }
        int NumberOfBurgers { get; }
        bool IsStarted { get; set; }
        bool IsCompleted { get; set; }
    }
}