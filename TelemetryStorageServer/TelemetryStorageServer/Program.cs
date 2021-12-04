using System;
using System.Configuration;
using System.Threading.Tasks;

namespace TelemetryStorageServer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                // если входные параметры заданы, то устанавливаем значение ReceivingMethod равное первому аргументу
                if (args.Length != 0) ConfigurationManager.AppSettings.Set("ReceivingMethod", args[0]);

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