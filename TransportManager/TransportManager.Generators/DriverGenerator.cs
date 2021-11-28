using System;
using System.Collections.Generic;
using TransportManager.Models;

namespace TransportManager.Generators
{
    /// <summary>
    ///     класс DriverGenerator используется для создания водителей при тестировании приложения
    /// </summary>
    public static class DriverGenerator
    {
        private static readonly Random Random = new Random();
        
        //список различных имен водителей
        private static readonly string[] Names = {"Joel", "Tommy", "Sam", "Ellie", "Riley", "Tess", "Deacon", "William", "Jack", 
            "Sarah", "Rikki", "Addy", "Rose", "Marta", "Amelia", "Rory", "Melody", "Richard", "Cara", "Dean", 
            "Sam", "Bobby", "Mary", "John", "Rick", "Morty", "Summer", "April"};

        /// <summary>
        ///     метод CreateDrivers создает водителей
        /// </summary>
        /// <param name="count">количество водителей, которое необходимо создать</param>
        /// <param name="companies">список компаний, к которым привязываются водители</param>
        /// <returns></returns>
        public static List<DriverModel> CreateDrivers(int count, List<CompanyModel> companies)
        {
            var drivers = new List<DriverModel>();
            for (int i = 0; i < count; i++)
            {
                drivers.Add(new DriverModel()
                {
                    Name = Names[Random.Next(Names.Length)],
                    CompanyId = companies[Random.Next(companies.Count)].CompanyId
                });
            }

            return drivers;
        }
    }
}