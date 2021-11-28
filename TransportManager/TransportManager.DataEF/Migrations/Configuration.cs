using System.Data.Entity.Migrations;
using TransportManager.DataEF.DbContext;

namespace TransportManager.DataEF.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EfDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}