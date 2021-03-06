using System;
using System.Collections.Generic;
using TransportManager.Domain;

namespace TransportManager.Statistics
{
    /// <summary>
    ///     класс TelemetryPacketSplitter используется для разделения списка TelemetryPacket на меньшие списки
    /// </summary>
    public static class TelemetryPacketSplitter
    {
        /// <summary>
        ///     метод SplitToChunks используется для разделения списка TelemetryPacket на куски
        /// </summary>
        /// <param name="telemetryPackets">список TelemetryPacket</param>
        /// <param name="chunksCount">количество частей, на которое необходимо разделить список</param>
        /// <returns>возвращаем список списков TelemetryPacket</returns>
        public static List<List<TelemetryPacket>> SplitToChunks(List<TelemetryPacket> telemetryPackets, int chunksCount)  
        {        
            List<List<TelemetryPacket>> chunks = new List<List<TelemetryPacket>>();

            // если кол-во элементов в списке не больше, чем chunksCount, то ставим размер куска = 1
            int chunkSize = telemetryPackets.Count > chunksCount ? telemetryPackets.Count / chunksCount : 1;

            for (int i = 0; i < telemetryPackets.Count; i += chunkSize) 
            {
                // проверяем что меньше - желаемый размер чанка, или остаток элементов до конца списка
                int newChunkSize = Math.Min(chunkSize, telemetryPackets.Count - i);
                // выделяем новый чанк - от элемента с индексом i в количестве newChunkSize
                var chunk = telemetryPackets.GetRange(i, newChunkSize);
                chunks.Add(chunk);
            }

            // Последующий блок кода необходим для регулирования размера последнего чанка
            // Если его размер получился более, чем в 2 раза меньше планируемого (chunkSize/2),
            // то копируем все его элементы в предыдущий список, и удаляем его (последний чанк)

            // Для выбора последнего списка используем конструкцию chunks[chunks.Count-1],
            // где chunks.Count - кол-во элементов в списке,
            // а "-1" необходимо для вычисления индекса элемента, т.к отсчет индекса начинается с 0, а не с 1, как кол-во
            // Аналогично с конструкцией chunks[chunks.Count-2] - "-2" необходимо для вычисления индекса предпоследнего элемента
            if (chunks[chunks.Count-1].Count < (double)chunkSize/2)
            {
                chunks[chunks.Count-1].ForEach(value => chunks[chunks.Count-2]
                                      .Add(value));
                
                chunks.RemoveAt(chunks.Count-1);
            }

            return chunks; 
        } 
    }
}