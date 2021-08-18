using System;
using System.Threading.Tasks;
using Data;
using Services;

namespace TelemetryStorageServer
{
    public class Bootstrapper
    {
        private Messenger _messenger;
        
        public Bootstrapper()
        {
            try
            {
                var messageReceiver = new MessageReceiver();

                var service = new TelemetryPacketsService(new TelemetryPacketsRepository());
                
                _messenger = new Messenger(messageReceiver, service);
            }
            catch (Exception)
            {
                Console.WriteLine();
                MessagesPrinter.PrintColorMessage("Bootstrapper: Ошибка иницализации!", ConsoleColor.Red);
                throw;
            }
        }

        /// <summary>
        ///     метод RunAsync запускает работу необходимых сервисов
        /// </summary>
        public async Task RunAsync()
        {
            if (_messenger == null) throw new Exception("Bootstrapper должен быть иницализирован до запуска!");

            try
            {
                await _messenger.StartAsync();
            }
            catch (Exception)
            {
                Console.WriteLine();
                MessagesPrinter.PrintColorMessage("Bootstrapper: Ошибка запуска Messenger!", ConsoleColor.Red);
                throw;
            }
        }
    }
}