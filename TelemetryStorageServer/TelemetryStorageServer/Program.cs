using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await ProgramStarter.StartAsync();
        }
    }
}