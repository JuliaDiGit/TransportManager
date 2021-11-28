using System;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.Common.Enums;
using TransportManager.DataEF.Repositories.Abstract;
using TransportManager.Domain;
using TransportManager.Mappers;
using TransportManager.Services.Abstract;
using TransportManager.Statistics;

namespace TransportManager.Services
{
    public class TelemetryPacketsService : ITelemetryPacketsService
    {
        private readonly ITelemetryRepository _telemetryPacketsRepository;

        public TelemetryPacketsService(ITelemetryRepository telemetryPacketsRepository)
        {
            _telemetryPacketsRepository = telemetryPacketsRepository;
        }
        
        public async Task<TelemetryPacket> BuildStatisticAsync(DateTime fromDate, 
                                                               DateTime toDate,
                                                               StatisticsType type, 
                                                               int vehicleId = 0)
        {
            var allTelemetryPackets = (await _telemetryPacketsRepository.GetTelemetryAsync())?
                                                                        .Select(s => s.ToDomain())
                                                                        .ToList();

            if (allTelemetryPackets == null || allTelemetryPackets.Count == 0) return null;

            // если не указан ид ТС, то отчет по всем ТС
            // для отчетов по всем ТС используем 10 потоков
            // поэтому список с TelemetryPacket делим на 10 частей и каждую часть рассчитываем в новом потоке
            // для отчетов по одному ТС - 6 потоков (6 частей)
            int threadsToUse = vehicleId == 0 ? 10 : 6;
            var splitStatistics = TelemetryPacketSplitter.SplitToChunks(allTelemetryPackets, threadsToUse);

            var tasks = splitStatistics.Select(statsList => StatisticsBuilder.BuildStatisticAsync(statsList, 
                                                                                                 fromDate,
                                                                                                 toDate, 
                                                                                                 type, 
                                                                                                 vehicleId)).ToList();
            
            TelemetryPacket[] chunksOfStatistics;
            
            try
            {
                chunksOfStatistics = await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
                return null;
            }

            TelemetryPacket totalStatistic = new TelemetryPacket();
            foreach (var statistic in chunksOfStatistics)
            {
                totalStatistic.Distance += statistic.Distance;
                totalStatistic.FuelConsumption += statistic.FuelConsumption;
                totalStatistic.TravelTime += statistic.TravelTime;
            }

            return totalStatistic;
        }
    }
}