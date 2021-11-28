using System;
using System.Collections.Generic;
using TransportManager.Models;

namespace TransportManager.Generators
{
    /// <summary>
    ///     Класс VehicleGenerator используется для создания ТС при тестировании приложения
    /// </summary>
    public static class VehicleGenerator
    {
        private static readonly Random Random = new Random();
        
        //список различных моделей ТС
        private static readonly string[] Models = {"Audi TT", "Audi R8", "BMW M2", "BMW X5", "Honda Civic", "Honda CR-V", 
            "Hyundai Solaris", "Hyundai Tucson", "Kia Rio", "Kia Mohave", "Lada Vesta", "Lada Xray", "Nissan GT-R", 
            "Nissan 350Z", "Toyota Supra", "Toyota Highlander"};
        
        //в российских гос.номерах ТС используются только такие буквы
        private static readonly string[] Letters = {"А", "В", "Е", "К", "М", "Н", "О", "Р", "С", "Т", "У", "Х" };

        /// <summary>
        ///     метод CreateVehicles создает ТС
        /// </summary>
        /// <param name="count">количество ТС, которое необходимо создать</param>
        /// <param name="drivers">список водителей, которых можно привязать к этим ТС</param>
        /// <returns></returns>
        public static List<VehicleModel> CreateVehicles(int count, List<DriverModel> drivers)
        {
            var vehicles = new List<VehicleModel>();
            for (int i = 0; i < count; i++)
            {
                DriverModel driver = drivers[Random.Next(0, drivers.Count - 1)];
                vehicles.Add(new VehicleModel()
                {
                    //для охвата всех состояний ТС - каждое пятое ТС создаем без привязки к водителю
                    DriverId = i % 5 == 0 ? (int?)null : driver.Id,
                    CompanyId = driver.CompanyId,
                    Model = Models[Random.Next(Models.Length)],
                    GovernmentNumber = Letters[Random.Next(Letters.Length)] + 
                                       Random.Next(100, 999) + 
                                       Letters[Random.Next(Letters.Length)] + 
                                       Letters[Random.Next(Letters.Length)] + 
                                       Random.Next(10,999)
                });
            }

            return vehicles;
        }
    }
}