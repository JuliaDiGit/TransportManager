using System;
using System.Threading.Tasks;
using Data;
using Services;

namespace TelemetryStorageServer
{
    public class Bootstrapper
    {
        private Messenger _messenger;
            
        public void Initialize()
        {
            try
            {
                InputParamsValidator.Validate(out var receivingMethod);
                
                DbInitialization.Start();
                
                ReceiverDeterminer.Determine(receivingMethod, 
                                             out var receiverHttp, 
                                             out var listener, 
                                             out var receiverTransact);

                var service = new TelemetryPacketsService(new TelemetryPacketsRepository());
                
                _messenger = new Messenger(receivingMethod, receiverHttp, listener, receiverTransact, service);
            }
            catch (Exception)
            {
                Console.WriteLine();
                MessagesPrinter.PrintColorMessage("Bootstrapper: Ошибка иницализации!", ConsoleColor.Red);
                throw;
            }
        }

        public async Task RunAsync()
        {
            if (_messenger == null) throw new Exception("Bootstrapper должен быть иницализирован до запуска!");
            
            await _messenger.StartAsync();
        }
    }
}