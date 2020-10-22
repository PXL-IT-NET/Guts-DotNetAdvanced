using System;
using Microsoft.EntityFrameworkCore;

namespace Bank.Data
{
    internal class BankContext : DbContext
    {
        public BankContext() { } //Constructor used by UI project

        public BankContext(DbContextOptions<BankContext> options) : base(options) { } //Constructor used by Test project

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) //only configure the connection if the parameter-less constructor was used (no options where provided).
            {
                //TODO: tell EF (Entity Framework) that is going to operate against a SQL Server database using the connection string in the app.config of the UI project
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void CreateOrUpdateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
