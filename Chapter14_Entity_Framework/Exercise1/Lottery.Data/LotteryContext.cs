using System.Configuration;
using Lottery.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Data
{
    public class LotteryContext : DbContext
    {
        public LotteryContext(){} //Constructor used by UI project

        public LotteryContext(DbContextOptions<LotteryContext> options) : base(options) { } //Constructor used by Test project

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) //only configure the connection if the parameterless contructor was used (no options where provided).
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
        }
    }
}
