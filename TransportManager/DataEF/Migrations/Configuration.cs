using System.Data.Entity.Migrations;
using DataEF.DbContext;

namespace DataEF.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EfDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}