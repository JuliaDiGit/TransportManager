using System;
using System.Collections.Generic;
using Domain;

namespace Services
{
    public static class TelemetryPacketsGenerator
    {
        /// <summary>
        ///     Метод Generate создает список из TelemetryPacket
        /// <para>Создаетcя не более 3х точек в пределах каледарного дня,
        /// для изменения итогового количества записей можно менять входные параметры</para>
        /// </summary>
        /// <param name="vehicles">количество ТС для которых генерируем TelemetryPacket</param>
        /// <param name="days">количество дней на которые генерируем TelemetryPacket</param>
        /// <returns></returns>
        
        public static List<TelemetryPacket> Generate(int vehicles = 15, int days = 3)
        {
            if (vehicles == 0 || days == 0) return null;
            
            Random random = new Random();

            List<TelemetryPacket> telemetryPackets = new List<TelemetryPacket>();
            
            //ТС
            for (int i = 1; i <= vehicles; i++)
            {
                int vehicleId = i;
                //дни
                for (int j = 1; j <= days; j++)
                {
                    DateTime date = DateTime.Now.AddDays(-j);
                    
                    //не более 3х точек в день
                    int point = random.Next(0, 4);
                    for (int k = 0; k < point; k++)
                    {
                        int hours = random.Next(0, 4);
                        int minutes = random.Next(0, 60);
                        int seconds = random.Next(0, 60);
                        
                        date = date.AddHours(hours);
                        date = date.AddMinutes(minutes);
                        date = date.AddSeconds(seconds);
                        
                        int timeInSec = seconds + minutes * 60 + hours * 3600;

                        double distance = 0.9 * random.Next(60, 120) * timeInSec/3600;
                        double fuelConsumption = 0.9 * random.Next(4, 21) * distance / 100;
                        
                        telemetryPackets.Add(new TelemetryPacket()
                        {
                            VehicleId = vehicleId,
                            Date = date,
                            Distance = distance,
                            FuelConsumption = fuelConsumption,
                            TravelTime = new TimeSpan(hours, minutes, seconds)
                        });
                    }
                }
            }

            return telemetryPackets;
        }
    }
}