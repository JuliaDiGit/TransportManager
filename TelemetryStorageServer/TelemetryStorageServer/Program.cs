using System;
using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {

            try
            {
                InputParamsValidator.Validate();
                
                DbInitialization.Start();
                
                var bootstrapper = new Bootstrapper();
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