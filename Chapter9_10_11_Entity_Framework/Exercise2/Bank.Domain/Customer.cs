using System;
using System.Collections.Generic;

namespace Bank.Domain
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string Address { get; set; }

        public int ZipCode { get; set; }

        public Result Validate(IReadOnlyList<City> validCities)
        {
            throw new NotImplementedException("The 'Validate' method of 'Customer' is not implemented correctly.");
        }
    }
}