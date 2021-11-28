using System.Threading.Tasks;
using TransportManager.DataEF.DbContext;

namespace TransportManager.DataEF
{
    public static class PrimaryDbInitializer
    {
        public static async Task InitializeMainDbAsync()
        {
            await Task.Run(() => {
                                    EfDbContext context = new EfDbContext();
                                    context.Database.Initialize(false);
                                 });
        }
        
        public static async Task InitializeTelemetryDbAsync()
        {
            await Task.Run(() => {
                                    TelemetryDbContext context = new TelemetryDbContext();
                                    context.Database.Initialize(false);
                                 });
        }
    }
}