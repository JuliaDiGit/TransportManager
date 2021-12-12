using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSenderEmulator.MessageSenders
{
    public class HttpSender
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<string> SendAsync(List<byte[]> byteArrays)
        {
            if (byteArrays == null) throw new ArgumentNullException(nameof(byteArrays));

            try
            {
                var json = JsonSerializer.Serialize(byteArrays);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await Client.PostAsync("http://localhost:6666/", data);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var binaryReader = new BinaryReader(stream))
                    {
                        var bytesResponseMessage = binaryReader.ReadBytes(54); //54 - размер сообщения от сервера
                        var responseMessage = Encoding.UTF8.GetString(bytesResponseMessage);
                        Console.WriteLine(responseMessage);
                        Console.WriteLine();
                    }
                }

                if ((int) response.StatusCode == 200) return Resources.SendingMessages_Success;
                
                throw new Exception(Resources.Error_StatusCode + (int) response.StatusCode);
            }
            catch (Exception e)
            { 
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
                
                return null;
            }
        }
    }
}