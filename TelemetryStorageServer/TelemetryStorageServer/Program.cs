using System;
using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Bootstrapper bootstrapper = new Bootstrapper();

            try
            {
                bootstrapper.Initialize();
                await bootstrapper.RunAsync();
            }
            catch (Exception e)
            {
                MessagesPrinter.PrintColorMessage(e, ConsoleColor.Red);
                Console.ReadKey();
            }
        }
    }
}