using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using TransportManager.Entities;
using TransportManager.Common.Enums;
using TransportManager.DataEF.DbContext;

namespace TransportManager.DataEF
{
    public class EfDbInitializer : CreateDatabaseIfNotExists<EfDbContext>
    {
        protected override void Seed(EfDbContext context)
        {
            var users = new List<UserEntity>
            {
                new UserEntity()
                {
                    Login = "Admin",
                    Password = "A1",
                    Role = Role.Admin,
                    IsDeleted = false
                },
                
                new UserEntity()
                {
                    Login = "Employee", 
                    Password = "E1", 
                    Role = Role.Employee, 
                    IsDeleted = false
                }
            };
            
            users.ForEach(u => context.Users.AddOrUpdate(u));

            var companies = new List<CompanyEntity>
            {
                new CompanyEntity()
                {
                    CompanyId = 101,
                    CompanyName = "Yandex",
                    IsDeleted = false
                },
                
                new CompanyEntity()
                {
                    CompanyId = 102,
                    CompanyName = "Uber",
                    IsDeleted = false
                },
                
                new CompanyEntity()
                {
                    CompanyId = 103,
                    CompanyName = "YouDrive",
                    IsDeleted = false
                },
                
                new CompanyEntity()
                {
                    CompanyId = 104,
                    CompanyName = "Делимобиль",
                    IsDeleted = false
                },
                
                new CompanyEntity()
                {
                    CompanyId = 105,
                    CompanyName = "MyTaxi",
                    IsDeleted = false
                }
            };
            
            companies.ForEach(c => context.Companies.AddOrUpdate(c));
            
            var drivers = new List<DriverEntity>
            {
                new DriverEntity()
                {
                    Name = "William",
                    CompanyId = 101,
                    IsDeleted = false
                },
                
                new DriverEntity()
                {
                    Name = "Mary",
                    CompanyId = 105,
                    IsDeleted = false
                },
                
                new DriverEntity()
                {
                    Name = "Marta",
                    CompanyId = 102,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Bobby",
                    CompanyId = 103,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Jack",
                    CompanyId = 103,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Summer",
                    CompanyId = 101,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Rose",
                    CompanyId = 104,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Deacon",
                    CompanyId = 103,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Rory",
                    CompanyId = 101,
                    IsDeleted = false
                },

                new DriverEntity()
                {
                    Name = "Cara",
                    CompanyId = 102,
                    IsDeleted = false
                }
            };
            
            drivers.ForEach(d => context.Drivers.AddOrUpdate(d));
            
            var vehicles = new List<VehicleEntity>
            {
                new VehicleEntity()
                {
                    Model = "Honda Civic",
                    GovernmentNumber = "О561ВС178",
                    DriverId = 1,
                    CompanyId = 101,
                    IsDeleted = false
                },
                
                new VehicleEntity()
                {
                    Model = "Nissan GT-R",
                    GovernmentNumber = "Х260СР178",
                    DriverId = 2,
                    CompanyId = 105,
                    IsDeleted = false
                },
                new VehicleEntity()
                {
                    Model = "Hyundai Tucson",
                    GovernmentNumber = "К327ТУ98",
                    DriverId = 3,
                    CompanyId = 102,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Honda CR-V",
                    GovernmentNumber = "О578ОР198",
                    DriverId = 4,
                    CompanyId = 103,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Lada Xray",
                    GovernmentNumber = "В666РМ198",
                    DriverId = 5,
                    CompanyId = 103,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Toyota Supra",
                    GovernmentNumber = "В452НМ47",
                    DriverId = 6,
                    CompanyId = 101,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Audi TT",
                    GovernmentNumber = "Х500АК78",
                    DriverId = 7,
                    CompanyId = 104,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Nissan 350Z",
                    GovernmentNumber = "В202УК47",
                    DriverId = 8,
                    CompanyId = 103,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Kia Mohave",
                    GovernmentNumber = "К597РС147",
                    DriverId = 9,
                    CompanyId = 101,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "BMW M2",
                    GovernmentNumber = "Н267РЕ47",
                    CompanyId = 102,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Hyundai Solaris",
                    GovernmentNumber = "К763НК55",
                    DriverId = 7,
                    CompanyId = 104,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Kia Rio",
                    GovernmentNumber = "Р811СР63",
                    DriverId = 7,
                    CompanyId = 104,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Audi R8",
                    GovernmentNumber = "Р568РВ90",
                    DriverId = 2,
                    CompanyId = 102,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "BMW X5",
                    GovernmentNumber = "У540ВУ46",
                    CompanyId = 105,
                    IsDeleted = false
                },

                new VehicleEntity()
                {
                    Model = "Lada Vesta",
                    GovernmentNumber = "С426ОА17",
                    DriverId = 8,
                    CompanyId = 104,
                    IsDeleted = false
                }
            };
            
            vehicles.ForEach(v => context.Vehicles.AddOrUpdate(v));

            context.SaveChanges();
        }
    }
}