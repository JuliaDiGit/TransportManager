using System;
using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();

            try
            {
                bootstrapper.Initialize();
                await bootstrapper.RunAsync();
            }
            catch (Exception e)
            {
                MessagesPrinter.PrintColorMessage(e.Message, ConsoleColor.Red);
                Console.ReadKey();
            }
        }
    }
}