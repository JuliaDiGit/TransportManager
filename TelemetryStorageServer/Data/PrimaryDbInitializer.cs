using System.Threading.Tasks;

namespace Data
{
    public static class PrimaryDbInitializer
    {
        public static async Task InitializeTelemetryDbAsync()
        {
            await Task.Run(() => { 
                                    TelemetryDbContext context = new TelemetryDbContext(); 
                                    context.Database.Initialize(false);
                                 });
        }
    }
}