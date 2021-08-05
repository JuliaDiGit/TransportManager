using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class HttpMessageReceiver
    {
        public async Task<List<byte[]>> Listen(HttpListener listener)
        {
            try
            {
                HttpListenerContext context = await listener.GetContextAsync(); // Ожидание входящего запроса
                HttpListenerRequest request = context.Request; // Объект запроса
                HttpListenerResponse response = context.Response; // Объект ответа

                // Создаем ответ
                Stream inputStream = request.InputStream; // Поток, содержащий данные основного текста, отправленные клиентом
                Encoding encoding = request.ContentEncoding; // Кодировка, использованная в запросе
                StreamReader reader = new StreamReader(inputStream, encoding); 
                var requestBody = await reader.ReadToEndAsync(); // Считываем всё, что есть в потоке
                var result = JsonSerializer.Deserialize<List<byte[]>>(requestBody); // Преобразуем JSON строку в List<byte[]>

                // Возвращаем ответ
                using (Stream stream = response.OutputStream)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(Resources.ServerConnection_Success);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}