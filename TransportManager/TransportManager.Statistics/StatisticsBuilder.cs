using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.Common.Enums;
using TransportManager.Domain;

namespace TransportManager.Statistics
{
    public static class StatisticsBuilder
    {
        /// <summary>
        ///     метод BuildReportAsync строит отчеты по ТС
        /// </summary>
        /// <param name="allVehiclesTelemetry">список телеметрии</param>
        /// <param name="fromDate">расчёт с даты включительно</param>
        /// <param name="toDate">расчёт по дату включительно</param>
        /// <param name="statisticsType">тип статистики</param>
        /// <param name="vehicleId">ид ТС, указывается при отчете по конкретному ТС</param>
        /// <returns></returns>
        
        public static async Task<TelemetryPacket> BuildStatisticAsync(List<TelemetryPacket> allVehiclesTelemetry, 
                                                                DateTime fromDate, 
                                                                DateTime toDate, 
                                                                StatisticsType statisticsType, 
                                                                int vehicleId = 0)
        {
            return await Task.Run(() =>
            {
                TelemetryPacket totalTelemetriesPacket = new TelemetryPacket();

                var telemetryForDate = allVehiclesTelemetry.Where(s => 
                                                                                      s.Date.Date >= fromDate 
                                                                                      && s.Date.Date <= toDate);
                
                foreach (var telemetry in telemetryForDate)
                {
                    if (vehicleId == 0)
                    {
                        totalTelemetriesPacket = SumStats(totalTelemetriesPacket, telemetry);
                    }
                    else if (telemetry.VehicleId == vehicleId)
                    {
                        totalTelemetriesPacket = SumStats(totalTelemetriesPacket, telemetry);
                    }
                }

                if (statisticsType == StatisticsType.Average)
                {
                    //добавляем +1 день, т.к при вычитании не учитывается, что последняя дата нужна "включительно"
                    int totalDays = (int) (toDate.Date - fromDate.Date).TotalDays + 1;

                    totalTelemetriesPacket.Distance /= totalDays;
                    totalTelemetriesPacket.FuelConsumption /= totalDays;
                    totalTelemetriesPacket.TravelTime = TimeSpan.FromMinutes(totalTelemetriesPacket.TravelTime.TotalMinutes / totalDays);
                }

                return totalTelemetriesPacket;
            });
        }
        
        private static TelemetryPacket SumStats(TelemetryPacket stats1, TelemetryPacket stats2)
        {
            return new TelemetryPacket
            {
                Distance = stats1.Distance + stats2.Distance,
                FuelConsumption = stats1.FuelConsumption + stats2.FuelConsumption,
                TravelTime = stats1.TravelTime + stats2.TravelTime
            };
        }
    }
}