namespace Bank.Data.DomainClasses
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string CellPhone { get; set; }
        public int ZipCode { get; set; }
    }
}