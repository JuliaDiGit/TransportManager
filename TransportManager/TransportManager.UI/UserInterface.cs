using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransportManager.API.Controllers;
using TransportManager.Common.Enums;
using TransportManager.DataEF.Repositories;
using TransportManager.Generators;
using TransportManager.Logger.Abstract;
using TransportManager.Models;
using TransportManager.Services;
using TransportManager.Services.Decorators.CompaniesServiceDecorators;
using TransportManager.Services.Decorators.DriversServiceDecorators;
using TransportManager.Services.Decorators.VehiclesServiceDecorators;
using TransportManager.UI.Properties;

namespace TransportManager.UI
{
    public class UserInterface
    {
        private readonly ILogger _logger;
        private readonly UserModel _user;

        public UserInterface(UserModel user, ILogger logger)
        {
            _user = user;
            _logger = logger;
        }

        public void Start()
        {
            bool isExit = true;

            while (isExit)
            {
                Menu();

                PrintMagentaMessage(Resources.Message_EnterCommandNumber);
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        Companies();
                        break;
                    case "2":
                        Drivers();
                        break;
                    case "3":
                        Vehicles();
                        break;
                    case "4":
                        Reports();
                        break;
                    case "0":
                        isExit = false;
                        break;
                    default:
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                        break;
                }
            }

            void Menu()
            {
                PrintCyanMessage("\nКакую категорию выбрать?");
                Console.WriteLine("1. Компании");
                Console.WriteLine("2. Водители");
                Console.WriteLine("3. Транспортые средства");
                Console.WriteLine("4. Отчёты по статистике ТС");
                Console.WriteLine("\n0. Выход из учётной записи\n");
            }
        }

        //Компании
        void Companies()
        {
            bool isExit = true;

            while (isExit)
            {
                CompaniesMenu();

                PrintMagentaMessage(Resources.Message_EnterCommandNumber);
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        Command1();
                        break;
                    case "2":
                        Command2();
                        break;
                    case "3":
                        Command3();
                        break;
                    case "4":
                        Command4();
                        break;
                    case "5":
                        Command5();
                        break;
                    case "6":
                        Command6();
                        break;
                    case "7":
                        Command7();
                        break;
                    case "0":
                        isExit = false;
                        break;
                    default:
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                        break;
                }

                void CompaniesMenu()
                {
                    PrintCyanMessage("\nКакую команду выполнить?");
                    Console.WriteLine("1. Показать все компании");
                    Console.WriteLine("2. Показать данные компании");
                    Console.WriteLine("3. Показать всех водителей компании");
                    Console.WriteLine("4. Показать все ТС компании");
                    Console.WriteLine("5. Добавить компанию");
                    Console.WriteLine("6. Обновить данные компании");
                    Console.WriteLine("7. Удалить компанию");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");
                }

                //1. Показать все компании
                void Command1()
                {
                    PrintGrayMessage("Показать все компании\n");

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger, 
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        List<CompanyModel> companies = companiesController.GetAllCompaniesAsync()
                                                                          .GetAwaiter()
                                                                          .GetResult();

                        if (companies.Count > 0)
                        {
                            PrintYellowMessage("\nСписок всех компаний");
                            companies.ForEach(company =>
                            {
                                string info = $"id: {company.CompanyId}, " +
                                              $"название: {company.CompanyName}";

                                if (!company.IsDeleted) Console.WriteLine(info);
                                else PrintRedMessage(info + " (удалено!)");
                            });
                        }
                        else PrintYellowMessage("В данный момент список компаний пуст");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //2. Показать данные компании
                void Command2()
                {
                    PrintGrayMessage("Показать данные компании\n");

                    while (true)
                    {
                        PrintGrayMessage("Введите id компании (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out int companyId))
                        {
                            try
                            {
                                var companiesService = new CompaniesService(new CompaniesRepository());
                                var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                                 _logger, 
                                                                                                 companiesService);
                                var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                                var companiesController = new CompaniesController(_user, companiesServiceEvent);

                                CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                          .GetAwaiter()
                                                                          .GetResult();

                                if (company == null || company.IsDeleted)
                                {
                                    PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                                    break;
                                }

                                var notRemoteDrivers = company.Drivers.Where(d => !d.IsDeleted).ToList();
                                var notRemoteVehicles = company.Vehicles.Where(v => !v.IsDeleted).ToList();

                                PrintYellowMessage($"\nДанные компании id {companyId}");
                                Console.WriteLine($"Название: {company.CompanyName}" +
                                                  $"\nКол-во водителей: {notRemoteDrivers.Count}" +
                                                  $"\nКол-во ТС: {notRemoteVehicles.Count}" +
                                                  $"\nДата создания (в базе): {company.CreatedDate}");

                                break;
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                            }
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }
                }

                //3. Показать всех водителей компании
                void Command3()
                {
                    PrintGrayMessage("Показать всех водителей компании\n");

                    int companyId;
                    while (true)
                    {
                        PrintGrayMessage("Введите id компании (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger, 
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                  .GetAwaiter()
                                                                  .GetResult();

                        if (company == null || company.IsDeleted)
                        {
                            PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                            return;
                        }

                        var notRemoteDrivers = company.Drivers.Where(d => !d.IsDeleted).ToList();

                        if (notRemoteDrivers.Count > 0)
                        {
                            PrintYellowMessage("\nСписок водителей компании " +
                                               $"{company.CompanyName} " +
                                               $"(id {companyId})");

                            foreach (var driver in notRemoteDrivers)
                            {
                                Console.WriteLine($"id водителя: {driver.Id}, " +
                                                  $"имя: {driver.Name}");
                            }
                        }
                        else PrintYellowMessage("\nВ данный момент у компании " +
                                                $"{company.CompanyName} (id {companyId}) " +
                                                "ещё нет водителей");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //4. Показать все ТС компании
                void Command4()
                {
                    PrintGrayMessage("Показать все ТС компании\n");

                    int companyId;
                    while (true)
                    {
                        PrintGrayMessage("Введите id компании (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger, 
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                  .GetAwaiter()
                                                                  .GetResult();

                        if (company == null || company.IsDeleted)
                        {
                            PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                            return;
                        }

                        var notRemoteVehicles = company.Vehicles.Where(v => !v.IsDeleted).ToList();

                        if (notRemoteVehicles.Count > 0)
                        {
                            PrintYellowMessage($"\nСписок ТС компании {company.CompanyName} " +
                                               $"(id {companyId})");

                            foreach (var vehicle in notRemoteVehicles)
                            {
                                Console.WriteLine($"id ТС: {vehicle.Id}, " +
                                                  $"модель: {vehicle.Model}, " +
                                                  $"гос.номер: {vehicle.GovernmentNumber}, " +
                                                  $"дата создания (в базе): {vehicle.CreatedDate}");
                            }
                        }
                        else PrintYellowMessage("\nВ данный момент у компании " +
                                                $"{company.CompanyName} (id {companyId})" +
                                                " ещё нет ТС");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //5. Добавить компанию
                void Command5()
                {
                    PrintGrayMessage("Добавить компанию\n");

                    string name;
                    while (true)
                    {
                        PrintGrayMessage("Введите название компании: ");
                        name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(name)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    int companyId;
                    while (true)
                    {
                        PrintGrayMessage("Введите уникальный id компании (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId))
                        {
                            break;
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger, 
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                  .GetAwaiter()
                                                                  .GetResult();

                        if (company != null)
                        {
                            PrintRedMessage($"\nКомпания id {companyId} уже существует!");
                            return;
                        }

                        company = companiesController.AddOrUpdateCompanyAsync(new CompanyModel
                                                                              {
                                                                                  CompanyName = name, 
                                                                                  CompanyId = companyId, 
                                                                                  Drivers = new List<DriverModel>(), 
                                                                                  Vehicles = new List<VehicleModel>()
                                                                              })
                                                     .GetAwaiter()
                                                     .GetResult();

                        PrintGreenMessage($"\nКомпания {company.CompanyName} " +
                                          $"(id {companyId}) - добавлена.");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //6. Обновить данные компании
                void Command6()
                {
                    PrintGrayMessage("Обновить данные компании\n");

                    while (true)
                    {
                        PrintGrayMessage("Введите id компании (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out int companyId))
                        {
                            try
                            {
                                var companiesService = new CompaniesService(new CompaniesRepository());
                                var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                                 _logger, 
                                                                                                 companiesService);
                                var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                                var companiesController = new CompaniesController(_user, companiesServiceEvent);

                                CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                          .GetAwaiter()
                                                                          .GetResult();

                                if (company == null || company.IsDeleted)
                                {
                                    PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                                    break;
                                }

                                PrintYellowMessage($"\nВыбрана компания {company.CompanyName} " +
                                                   $"(id {companyId})");

                                string companyName;
                                while (true)
                                {
                                    PrintGrayMessage("Введите новое название компании: ");
                                    companyName = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(companyName)) break;
                                    PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                }

                                var updatedCompany = companiesController.AddOrUpdateCompanyAsync(new CompanyModel
                                                                                                 {
                                                                                                     Id = company.Id, 
                                                                                                     CreatedDate = company.CreatedDate, 
                                                                                                     CompanyId = companyId, 
                                                                                                     CompanyName = companyName
                                                                                                 })
                                                                        .GetAwaiter()
                                                                        .GetResult();

                                PrintGreenMessage($"\nДанные компании id {updatedCompany.CompanyId} - обновлены, " +
                                                  $"новое название - {updatedCompany.CompanyName}");
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                            }

                            break;
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }
                }

                //7. Удалить компанию
                void Command7()
                {
                    PrintGrayMessage("Удалить компанию\n");

                    PrintRedMessage("\nВНИМАНИЕ! \nПри удалении компании " +
                                    "будут удалены все её водители и ТС!\n");

                    int companyId;
                    while (true)
                    {
                        PrintGrayMessage("Введите id компании " +
                                         "(для отмены - введите 0): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    if (companyId == 0) return;

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger,
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        CompanyModel company = companiesController.RemoveCompanyByCompanyIdAsync(companyId)
                                                                  .GetAwaiter()
                                                                  .GetResult();

                        if (company == null) PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                        else PrintGreenMessage($"\nКомпания {company.CompanyName} (id {companyId}) - удалена");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }
            }
        }

        //Водители
        void Drivers()
        {
            bool isExit = true;

            while (isExit)
            {
                DriversMenu();

                PrintMagentaMessage(Resources.Message_EnterCommandNumber);
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        Command1();
                        break;
                    case "2":
                        Command2();
                        break;
                    case "3":
                        Command3();
                        break;
                    case "4":
                        Command4();
                        break;
                    case "5":
                        Command5();
                        break;
                    case "6":
                        Command6();
                        break;
                    case "7":
                        Command7();
                        break;
                    case "0":
                        isExit = false;
                        break;
                    default:
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                        break;
                }

                void DriversMenu()
                {
                    PrintCyanMessage("\nКакую команду выполнить?");
                    Console.WriteLine(@"1. Показать всех водителей");
                    Console.WriteLine(@"2. Показать данные водителя");
                    Console.WriteLine(@"3. Показать все ТС водителя");
                    Console.WriteLine(@"4. Добавить водителя");
                    Console.WriteLine(@"5. Обновить данные водителя");
                    Console.WriteLine(@"6. Удалить водителя");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");

                    PrintGreenMessage("ДЛЯ ТЕСТИРОВАНИЯ");
                    Console.WriteLine("7. Сгенерировать водителей\n");
                }

                //1. Показать всех водителей
                void Command1()
                {
                    PrintGrayMessage("Показать всех водителей\n");

                    try
                    {
                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger, 
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);
                        
                        List<DriverModel> drivers = driversController.GetAllDriversAsync()
                                                                     .GetAwaiter()
                                                                     .GetResult();

                        if (drivers.Count > 0)
                        {
                            PrintYellowMessage("\nСписок всех водителей");
                            drivers.ForEach(driver =>
                            {
                                string info = $"id: {driver.Id}, " +
                                              $"имя: {driver.Name}, " +
                                              $"id компании: {driver.CompanyId}";

                                if (!driver.IsDeleted) Console.WriteLine(info);
                                else PrintRedMessage(info + " (удалено!)");
                            });
                        }
                        else PrintYellowMessage("\nВ данный момент список водителей пуст");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //2. Показать данные водителя
                void Command2()
                {
                    PrintGrayMessage("Показать данные водителя\n");

                    while (true)
                    {
                        PrintGrayMessage("Введите id водителя (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out int id))
                        {
                            try
                            {
                                var driversService = new DriversService(new DriversRepository());
                                var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                             _logger, 
                                                                                             driversService);
                                var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                                var driversController = new DriversController(_user, driversServiceEvent);

                                DriverModel driver = driversController.GetDriverByIdAsync(id)
                                                                      .GetAwaiter()
                                                                      .GetResult();

                                if (driver == null || driver.IsDeleted)
                                {
                                    PrintRedMessage($"\nВодитель id {id} - не найден");
                                    break;
                                }

                                var notRemoteVehicles = driver.Vehicles.Where(v => !v.IsDeleted).ToList();

                                PrintYellowMessage($"\nДанные водителя id {driver.Id}");
                                Console.WriteLine($"Имя: {driver.Name}");
                                Console.WriteLine($"Id компании: {driver.CompanyId}");
                                Console.WriteLine($"Кол-во ТС: {notRemoteVehicles.Count}");
                                Console.WriteLine($"Дата создания (в базе): {driver.CreatedDate}");

                                break;
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                                break;
                            }
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }
                }

                //3. Показать все ТС водителя
                void Command3()
                {
                    PrintGrayMessage("Показать все ТС водителя\n");

                    int id;
                    while (true)
                    {
                        PrintGrayMessage("Введите id водителя (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out id)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger, 
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);

                        DriverModel driver = driversController.GetDriverByIdAsync(id)
                                                              .GetAwaiter()
                                                              .GetResult();

                        if (driver == null || driver.IsDeleted)
                        {
                            PrintRedMessage($"\nВодитель id {id} - не найден");
                            return;
                        }

                        var notRemoteVehicles = driver.Vehicles.Where(v => !v.IsDeleted).ToList();

                        if (notRemoteVehicles.Count > 0)
                        {
                            PrintYellowMessage($"\nСписок ТС водителя id {id}");
                            foreach (var vehicle in notRemoteVehicles)
                            {
                                Console.WriteLine($"id ТС: {vehicle.Id}, " +
                                                  $"модель: {vehicle.Model}, " +
                                                  $"гос.номер: {vehicle.GovernmentNumber}, " +
                                                  $"дата создания (в базе): {vehicle.CreatedDate}");
                            }
                        }
                        else PrintYellowMessage($"\nВ данный момент у водителя id {id} ещё нет ТС");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //4. Добавить водителя
                void Command4()
                {
                    PrintGrayMessage("Добавить водителя\n");

                    string name;
                    while (true)
                    {
                        PrintGrayMessage("Введите имя водителя: ");
                        name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(name)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    int companyId;
                    while (true)
                    {
                        PrintGrayMessage("Введите id компании водителя (цифры): ");
                        string sIdV = Console.ReadLine();

                        if (int.TryParse(sIdV, out companyId)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger, 
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                  .GetAwaiter()
                                                                  .GetResult();

                        if (company == null || company.IsDeleted)
                        {
                            PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                            return;
                        }

                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger, 
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);

                        var driver = driversController.AddOrUpdateDriverAsync(new DriverModel
                                                                              {
                                                                                  Name = name, 
                                                                                  CompanyId = companyId, 
                                                                                  Vehicles = new List<VehicleModel>()
                                                                              })
                                                       .GetAwaiter()
                                                       .GetResult();

                        PrintGreenMessage($"\nВодитель {driver.Name} - добавлен. " +
                                          $"Id в базе: {driver.Id}");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //5. Обновить данные водителя
                void Command5()
                {
                    PrintGrayMessage("Обновить данные водителя\n");

                    while (true)
                    {
                        PrintGrayMessage("Введите id водителя (цифры): ");
                        string sId = Console.ReadLine();
                        
                        if (int.TryParse(sId, out var id))
                        {
                            try
                            {
                                var driversService = new DriversService(new DriversRepository());
                                var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                             _logger, 
                                                                                             driversService);
                                var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                                var driversController = new DriversController(_user, driversServiceEvent);

                                DriverModel driver = driversController.GetDriverByIdAsync(id)
                                                                      .GetAwaiter()
                                                                      .GetResult();

                                if (driver == null || driver.IsDeleted)
                                {
                                    PrintRedMessage($"\nВодитель id {id} - не найден");
                                    break;
                                }

                                PrintYellowMessage($"\nВыбран водитель {driver.Name} (id {id}) " +
                                                   $"из компании id {driver.CompanyId}");

                                string name;
                                while (true)
                                {
                                    PrintGrayMessage("Введите новое имя водителя: ");
                                    name = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(name)) break;
                                    PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                }

                                int companyId;
                                while (true)
                                {
                                    PrintGrayMessage("Введите новый id компании водителя (цифры): ");
                                    string sIdV = Console.ReadLine();

                                    if (int.TryParse(sIdV, out companyId)) break;
                                    PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                }

                                var companiesService = new CompaniesService(new CompaniesRepository());
                                var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                                 _logger, 
                                                                                                 companiesService);
                                var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                                var companiesController = new CompaniesController(_user, companiesServiceEvent);

                                CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                          .GetAwaiter()
                                                                          .GetResult();

                                if (company == null || company.IsDeleted)
                                {
                                    PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                                    return;
                                }

                                var updatedDriver = driversController.AddOrUpdateDriverAsync(new DriverModel
                                                                                             {
                                                                                                 Id = id, 
                                                                                                 CreatedDate = driver.CreatedDate,
                                                                                                 Name = name, 
                                                                                                 CompanyId = companyId, 
                                                                                                 Vehicles = driver.Vehicles
                                                                                             })
                                                                     .GetAwaiter()
                                                                     .GetResult();

                                PrintGreenMessage($"\nДанные водителя id {updatedDriver.Id} - обновлены");
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                            }

                            break;
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }
                }

                //6. Удалить водителя
                void Command6()
                {
                    PrintGrayMessage("Удалить водителя\n");

                    int id;
                    while (true)
                    {
                        PrintGrayMessage("Введите id водителя (цифры): ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out id)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger,
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);
                        DriverModel driver = driversController.RemoveDriverByIdAsync(id)
                                                              .GetAwaiter()
                                                              .GetResult();

                        if (driver == null) PrintRedMessage($"\nВодитель id {id} - не найден");
                        else PrintGreenMessage($"\nВодитель id {id} - удален");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //7. Сгенерировать водителей
                void Command7()
                {
                    PrintGrayMessage("Сгенерировать водителей\n");

                    int count;
                    while (true)
                    {
                        PrintGrayMessage("Введите количество водителей: ");
                        string sId = Console.ReadLine();
                        if (int.TryParse(sId, out count)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var companiesService = new CompaniesService(new CompaniesRepository());
                        var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                         _logger, 
                                                                                         companiesService);
                        var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                        var companiesController = new CompaniesController(_user, companiesServiceEvent);

                        List<CompanyModel> companies = companiesController.GetAllCompaniesAsync()
                                                                          .GetAwaiter()
                                                                          .GetResult();

                        List<DriverModel> drivers = DriverGenerator.CreateDrivers(count, companies);

                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger, 
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);

                        foreach (var driver in drivers)
                        {
                            driversController.AddOrUpdateDriverAsync(driver)
                                             .GetAwaiter()
                                             .GetResult();
                        }
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }
            }
        }

        //ТС
        void Vehicles()
        {
            bool isExit = true;

            while (isExit)
            {
                VehiclesMenu();

                PrintMagentaMessage(Resources.Message_EnterCommandNumber);
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        Command1();
                        break;
                    case "2":
                        Command2();
                        break;
                    case "3":
                        Command3();
                        break;
                    case "4":
                        Command4();
                        break;
                    case "5":
                        Command5();
                        break;
                    case "6":
                        Command6();
                        break;
                    case "0":
                        isExit = false;
                        break;
                    default:
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                        break;
                }

                void VehiclesMenu()
                {
                    PrintCyanMessage("\nКакую команду выполнить?");
                    Console.WriteLine(@"1. Показать все ТС");
                    Console.WriteLine(@"2. Показать данные ТС");
                    Console.WriteLine(@"3. Добавить ТС");
                    Console.WriteLine(@"4. Обновить данные ТС");
                    Console.WriteLine(@"5. Удалить ТС");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");

                    PrintGreenMessage("ДЛЯ ТЕСТИРОВАНИЯ");
                    Console.WriteLine("6. Сгенерировать ТС\n");
                }

                //1. Показать все ТС
                void Command1()
                {
                    PrintGrayMessage("Показать все ТС\n");

                    var vehiclesService = new VehiclesService(new VehiclesRepository());
                    var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                   _logger, 
                                                                                   vehiclesService);
                    var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                    var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                    try
                    {
                        List<VehicleModel> vehicles = vehiclesController.GetAllVehiclesAsync()
                                                                        .GetAwaiter()
                                                                        .GetResult();

                        if (vehicles.Count > 0)
                        {
                            PrintYellowMessage("\nСписок всех ТС");
                            vehicles.ForEach(vehicle =>
                            {
                                string info = $"id ТС: {vehicle.Id}, " +
                                              $"id компании: {vehicle.CompanyId}, " +
                                              (vehicle.DriverId > 0
                                                  ? $"Id водителя: {vehicle.DriverId}, "
                                                  : "Id водителя: -, ") +
                                              $"модель: {vehicle.Model}, " +
                                              $"гос.номер: {vehicle.GovernmentNumber}";

                                if (!vehicle.IsDeleted) Console.WriteLine(info);
                                else PrintRedMessage(info + " (удалено!)");
                            });
                        }
                        else PrintYellowMessage("В данный момент список ТС пуст");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //2. Показать данные ТС
                void Command2()
                {
                    PrintGrayMessage("Показать данные ТС\n");

                    while (true)
                    {
                        PrintGrayMessage("Введите id ТС: ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out int id))
                        {
                            var vehiclesService = new VehiclesService(new VehiclesRepository());
                            var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                           _logger, 
                                                                                           vehiclesService);
                            var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                            var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                            try
                            {
                                VehicleModel vehicle = vehiclesController.GetVehicleByIdAsync(id)
                                                                         .GetAwaiter()
                                                                         .GetResult();

                                if (vehicle == null || vehicle.IsDeleted)
                                {
                                    PrintRedMessage($"\nТС id {id} - не найдено");
                                    break;
                                }

                                PrintYellowMessage($"\nДанные ТС id {vehicle.Id}");
                                Console.WriteLine($"Id компании: {vehicle.CompanyId}");
                                Console.WriteLine(vehicle.DriverId > 0 
                                                        ? $"Id водителя: {vehicle.DriverId}"
                                                        : "Id водителя: -");
                                Console.WriteLine($"Модель: {vehicle.Model}");
                                Console.WriteLine($"Гос.номер: {vehicle.GovernmentNumber}");
                                Console.WriteLine($"Дата создания (в базе): {vehicle.CreatedDate}");

                                break;
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                                break;
                            }
                        }
                    }
                }

                //3. Добавить ТС
                void Command3()
                {
                    PrintGrayMessage("Добавить ТС\n");

                    string model;
                    while (true)
                    {
                        PrintGrayMessage("Введите модель ТС: ");
                        model = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(model)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    string gn;
                    while (true)
                    {
                        PrintGrayMessage("Введите гос.номер ТС (А000АА00): ");
                        gn = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(model)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    int driverId;
                    while (true)
                    {
                        PrintGrayMessage("Введите id водителя ТС (цифры), " +
                                         "если водителя нет - введите 0: ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out driverId)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger,
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);

                        DriverModel driver = null;

                        if (driverId != 0)
                        {
                            driver = driversController.GetDriverByIdAsync(driverId)
                                                      .GetAwaiter()
                                                      .GetResult();

                            if (driver == null || driver.IsDeleted)
                            {
                                PrintRedMessage($"Водитель id {driverId} не найден!");
                                return;
                            }
                        }

                        int companyId;
                        if (driverId == 0)
                        {
                            while (true)
                            {
                                PrintGrayMessage("Введите id компании ТС (цифры): ");
                                string sIdV = Console.ReadLine();

                                if (int.TryParse(sIdV, out companyId)) break;
                                PrintRedMessage(Resources.Error_IncorrectData + "\n");
                            }

                            var companiesService = new CompaniesService(new CompaniesRepository());
                            var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                             _logger,
                                                                                             companiesService);
                            var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                            var companiesController = new CompaniesController(_user, companiesServiceEvent);

                            CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                      .GetAwaiter()
                                                                      .GetResult();

                            if (company == null || company.IsDeleted)
                            {
                                PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                                return;
                            }
                        }
                        else
                        {
                            companyId = driver.CompanyId;
                        }

                        var vehiclesService = new VehiclesService(new VehiclesRepository());
                        var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                       _logger, 
                                                                                       vehiclesService);
                        var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                        var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                        var vehicle = vehiclesController.AddOrUpdateVehicleAsync(new VehicleModel
                                                                                 {
                                                                                     Model = model, 
                                                                                     GovernmentNumber = gn, 
                                                                                     DriverId = driverId == 0 ? (int?) null : driverId, 
                                                                                     CompanyId = companyId
                                                                                 })
                                                        .GetAwaiter()
                                                        .GetResult();

                        PrintGreenMessage($"\nТС гос.номер {vehicle.GovernmentNumber} - добавлено. " +
                                          $"Id в базе: {vehicle.Id}");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //4. Обновить данные ТС
                void Command4()
                {
                    PrintGrayMessage("Обновить данные ТС\n");

                    while (true)
                    {
                        PrintGrayMessage("Введите id ТС: ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out int id))
                        {
                            try
                            {
                                var vehiclesService = new VehiclesService(new VehiclesRepository());
                                var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                               _logger, 
                                                                                               vehiclesService);
                                var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                                var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                                VehicleModel vehicle = vehiclesController.GetVehicleByIdAsync(id)
                                                                         .GetAwaiter()
                                                                         .GetResult();
                                if (vehicle == null || vehicle.IsDeleted)
                                {
                                    PrintRedMessage($"\nТС id {id} - не найдено");
                                    break;
                                }

                                string model;
                                while (true)
                                {
                                    PrintGrayMessage("Введите новую модель ТС: ");
                                    model = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(model)) break;
                                    PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                }

                                string gn;
                                while (true)
                                {
                                    PrintGrayMessage("Введите новый гос.номер ТС (А000АА00): ");
                                    gn = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(model)) break;
                                    PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                }

                                int driverId;
                                while (true)
                                {
                                    PrintGrayMessage("Введите новый id водителя ТС (цифры), " +
                                                     "если водителя нет - введите 0: ");
                                    string sDriverId = Console.ReadLine();

                                    if (int.TryParse(sDriverId, out driverId)) break;
                                    PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                }

                                var driversService = new DriversService(new DriversRepository());
                                var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                             _logger, 
                                                                                             driversService);
                                var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                                var driversController = new DriversController(_user, driversServiceEvent);

                                DriverModel driver = null;

                                if (driverId != 0)
                                {
                                    driver = driversController.GetDriverByIdAsync(driverId)
                                                              .GetAwaiter()
                                                              .GetResult();

                                    if (driver == null || driver.IsDeleted)
                                    {
                                        PrintRedMessage($"Водитель id {driverId} не найден!");
                                        return;
                                    }
                                }

                                int companyId;
                                if (driverId == 0)
                                {
                                    while (true)
                                    {
                                        PrintGrayMessage("Введите новый id компании ТС (цифры): ");
                                        string sCompanyId = Console.ReadLine();

                                        if (int.TryParse(sCompanyId, out companyId)) break;
                                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                                    }

                                    var companiesService = new CompaniesService(new CompaniesRepository());
                                    var companiesServiceLogger = new CompaniesServiceLoggerDecorator(_user,
                                                                                                     _logger,
                                                                                                     companiesService);
                                    var companiesServiceEvent = new CompaniesServiceEventDecorator(companiesServiceLogger);
                                    var companiesController = new CompaniesController(_user, companiesServiceEvent);

                                    CompanyModel company = companiesController.GetCompanyByCompanyIdAsync(companyId)
                                                                              .GetAwaiter()
                                                                              .GetResult();

                                    if (company == null || company.IsDeleted)
                                    {
                                        PrintRedMessage("\n" + Resources.Error_CompanyNotFound);
                                        return;
                                    }
                                }
                                else
                                {
                                    companyId = driver.CompanyId;
                                }

                                vehiclesService = new VehiclesService(new VehiclesRepository());
                                vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                           _logger, 
                                                                                           vehiclesService);
                                vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                                vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);
                                
                                var updatedVehicle =
                                    vehiclesController.AddOrUpdateVehicleAsync(new VehicleModel
                                                                               {
                                                                                   Id = id, 
                                                                                   CreatedDate = vehicle.CreatedDate, 
                                                                                   Model = model, 
                                                                                   GovernmentNumber = gn, 
                                                                                   DriverId = driverId == 0 ? (int?) null : driverId, 
                                                                                   CompanyId = companyId
                                                                               })
                                                       .GetAwaiter()
                                                       .GetResult();

                                PrintGreenMessage($"\nДанные ТС id {updatedVehicle.Id} - обновлены");
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                            }

                            break;
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }
                }

                //5. Удалить ТС
                void Command5()
                {
                    PrintGrayMessage("Удалить ТС\n");

                    int id;
                    while (true)
                    {
                        PrintGrayMessage("Введите id ТС (цифры): ");
                        string sId = Console.ReadLine();
                        if (int.TryParse(sId, out id)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var vehiclesService = new VehiclesService(new VehiclesRepository());
                        var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user, 
                                                                                       _logger, 
                                                                                       vehiclesService);
                        var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                        var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                        var vehicle = vehiclesController.RemoveVehicleByIdAsync(id)
                                                        .GetAwaiter()
                                                        .GetResult();

                        if (vehicle == null) PrintRedMessage($"\nТС id {id} - не найдено");
                        else PrintGreenMessage($"\nТС id {id} - удалено");
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }

                //6. Сгенерировать ТС
                void Command6()
                {
                    PrintGrayMessage("Сгенерировать ТС\n");

                    int count;
                    while (true)
                    {
                        PrintGrayMessage("Введите количество ТС: ");
                        string sId = Console.ReadLine();
                        if (int.TryParse(sId, out count)) break;
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }

                    try
                    {
                        var driversService = new DriversService(new DriversRepository());
                        var driversServiceLogger = new DriversServiceLoggerDecorator(_user,
                                                                                     _logger, 
                                                                                     driversService);
                        var driversServiceEvent = new DriversServiceEventDecorator(driversServiceLogger);
                        var driversController = new DriversController(_user, driversServiceEvent);

                        List<DriverModel> drivers = driversController.GetAllDriversAsync()
                                                                     .GetAwaiter()
                                                                     .GetResult();

                        List<VehicleModel> vehicles = VehicleGenerator.CreateVehicles(count, drivers);

                        var vehiclesService = new VehiclesService(new VehiclesRepository());
                        var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                       _logger, 
                                                                                       vehiclesService);
                        var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                        var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                        foreach (var vehicle in vehicles)
                        {
                            vehiclesController.AddOrUpdateVehicleAsync(vehicle)
                                              .GetAwaiter()
                                              .GetResult();
                        }
                    }
                    catch (Exception e)
                    {
                        PrintRedMessage("\n" + e.Message);
                    }
                }
            }
        }

        //Отчёты
        void Reports()
        {
            bool isExit = true;

            while (isExit)
            {
                ReportsMenu();

                PrintMagentaMessage(Resources.Message_EnterCommandNumber);
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        Command1();
                        break;
                    case "2":
                        Command2();
                        break;
                    case "3":
                        Command3();
                        break;
                    case "4":
                        Command4();
                        break;
                    case "5":
                        Command5();
                        break;
                    case "6":
                        Command6();
                        break;
                    case "0":
                        isExit = false;
                        break;
                    default:
                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                        break;
                }

                void ReportsMenu()
                {
                    PrintCyanMessage("\nКакой отчёт составить?");
                    Console.WriteLine(@"1. Суммарный отчет по всем ТС за сегодня");
                    Console.WriteLine(@"2. Суммарный отчет по всем ТС за период");
                    Console.WriteLine(@"3. Среднесуточный отчет по всем ТС за период");
                    Console.WriteLine(@"4. Суммарный отчет по одному ТС за сегодня");
                    Console.WriteLine(@"5. Суммарный отчет по одному ТС за период");
                    Console.WriteLine(@"6. Среднесуточный отчет по одному ТС за период");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");
                }

                //1. Суммарный отчет по всем ТС за сегодня
                void Command1()
                {
                    PrintGrayMessage("Суммарный отчет по всем ТС за сегодня\n");
                    
                    DateTime date = DateTime.Today;
                    
                    var statisticService = new TelemetryPacketsService(new TelemetryRepository());
                    var statisticController = new TelemetryPacketsController(statisticService);

                    var task = statisticController.GetStatisticAsync(date, 
                                                                     date, 
                                                                     StatisticsType.Summary);
                    
                    Wait(task);
                    
                    TelemetryPacketModel telemetryPacket = task.GetAwaiter().GetResult();
                    
                    PrintReport(telemetryPacket, 
                                date, 
                                date, 
                                StatisticsType.Summary);
                }

                //2. Суммарный отчет по всем ТС за период
                void Command2()
                {
                    PrintGrayMessage("Суммарный отчет по всем ТС за период\n");
                    
                    ReadDate(out var fromDate, out var toDate);
                    
                    var statisticService = new TelemetryPacketsService(new TelemetryRepository());
                    var statisticController = new TelemetryPacketsController(statisticService);

                    var task = statisticController.GetStatisticAsync(fromDate, 
                                                                     toDate, 
                                                                     StatisticsType.Summary);
                    
                    Wait(task);
                    
                    TelemetryPacketModel telemetryPacket = task.GetAwaiter().GetResult();
                    
                    PrintReport(telemetryPacket, 
                                fromDate, 
                                toDate, 
                                StatisticsType.Summary);
                }

                //3. Среднесуточный отчет по всем ТС за период
                void Command3()
                {
                    PrintGrayMessage("Среднесуточный отчет по всем ТС за период\n");

                    ReadDate(out var fromDate, out var toDate);
                    
                    var statisticService = new TelemetryPacketsService(new TelemetryRepository());
                    var statisticController = new TelemetryPacketsController(statisticService);

                    var task = statisticController.GetStatisticAsync(fromDate, 
                                                                     toDate, 
                                                                     StatisticsType.Average);
                    
                    Wait(task);
                    
                    TelemetryPacketModel telemetryPacket = task.GetAwaiter().GetResult();
                    
                    PrintReport(telemetryPacket, 
                                fromDate, 
                                toDate, 
                                StatisticsType.Average);
                }

                //4. Суммарный отчет по одному ТС за сегодня
                void Command4()
                {
                    PrintGrayMessage("Суммарный отчет по одному ТС за сегодня\n");

                    var vehicle = FindVehicle();

                    if (vehicle != null)
                    {
                        DateTime date = DateTime.Today;

                        var statisticService = new TelemetryPacketsService(new TelemetryRepository());
                        var statisticController = new TelemetryPacketsController(statisticService);

                        var task = statisticController.GetStatisticAsync(date,
                                                                         date, 
                                                                         StatisticsType.Summary,
                                                                         vehicle.Id);

                        Wait(task);

                        TelemetryPacketModel telemetryPacket = task.GetAwaiter().GetResult();

                        PrintReport(telemetryPacket, 
                                    date, 
                                    date, 
                                    StatisticsType.Summary, 
                                    vehicle.Id);
                    }
                }

                //5. Суммарный отчет по одному ТС за период
                void Command5()
                {
                    PrintGrayMessage("Суммарный отчет по одному ТС за период\n");

                    var vehicle = FindVehicle();

                    if (vehicle != null)
                    {
                        ReadDate(out var fromDate, out var toDate);

                        var statisticService = new TelemetryPacketsService(new TelemetryRepository());
                        var statisticController = new TelemetryPacketsController(statisticService);

                        var task = statisticController.GetStatisticAsync(fromDate,
                                                                         toDate, 
                                                                         StatisticsType.Summary,
                                                                         vehicle.Id);

                        Wait(task);

                        TelemetryPacketModel telemetryPacket = task.GetAwaiter().GetResult();

                        PrintReport(telemetryPacket, 
                                    fromDate, 
                                    toDate, 
                                    StatisticsType.Summary, 
                                    vehicle.Id);
                    }
                }
                
                //6. Среднесуточный отчет по одному ТС за период
                void Command6()
                {
                    PrintGrayMessage("Среднесуточный отчет по одному ТС за период\n");

                    var vehicle = FindVehicle();

                    if (vehicle != null)
                    {
                        ReadDate(out var fromDate, out var toDate);
                        
                        var statisticService = new TelemetryPacketsService(new TelemetryRepository());
                        var statisticController = new TelemetryPacketsController(statisticService);

                        var task = statisticController.GetStatisticAsync(fromDate,
                                                                         toDate, 
                                                                         StatisticsType.Average,
                                                                         vehicle.Id);

                        Wait(task);

                        TelemetryPacketModel statistic = task.GetAwaiter().GetResult();

                        PrintReport(statistic, 
                                    fromDate, 
                                    toDate, 
                                    StatisticsType.Average, 
                                    vehicle.Id);
                    }
                }

                //Чтение id ТС и поиск этого ТС в бд
                VehicleModel FindVehicle()
                {
                    while (true)
                    {
                        PrintGrayMessage("\nВведите id ТС: ");
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out int id))
                        {
                            var vehiclesService = new VehiclesService(new VehiclesRepository());
                            var vehiclesServiceLogger = new VehiclesServiceLoggerDecorator(_user,
                                                                                           _logger, 
                                                                                           vehiclesService);
                            var vehiclesServiceEvent = new VehiclesServiceEventDecorator(vehiclesServiceLogger);
                            var vehiclesController = new VehiclesController(_user, vehiclesServiceEvent);

                            try
                            {
                                VehicleModel vehicle = vehiclesController.GetVehicleByIdAsync(id)
                                                                         .GetAwaiter()
                                                                         .GetResult();

                                if (vehicle != null && !vehicle.IsDeleted) return vehicle;
                                PrintRedMessage($"\nТС id {id} - не найдено");
                                return null;
                            }
                            catch (Exception e)
                            {
                                PrintRedMessage("\n" + e.Message);
                                return null;
                            }
                        }
                        
                        PrintRedMessage(Resources.Error_IncorrectData);
                    }
                }

                //Чтение и передача введенных дат
                void ReadDate(out DateTime fromDate, out DateTime toDate)
                {
                    while (true)
                    {
                        Console.Write("\nВведите начальную дату в формате \"дд/мм/гггг\": ");
                        string sDate1 = Console.ReadLine();

                        if (DateTime.TryParse(sDate1,
                            CultureInfo.CreateSpecificCulture("en-GB"), 
                            DateTimeStyles.None, 
                            out fromDate)) break;

                        PrintRedMessage(Resources.Error_IncorrectData);
                    }

                    while (true)
                    {
                        Console.Write("Введите конечную дату в формате \"дд/мм/гггг\": ");
                        string sDate2 = Console.ReadLine();

                        if (DateTime.TryParse(sDate2,
                            CultureInfo.CreateSpecificCulture("en-GB"), 
                            DateTimeStyles.None, 
                            out toDate))
                        {
                            if (fromDate > toDate) PrintRedMessage("Конечная дата раньше начальной!\n");
                            break;
                        }

                        PrintRedMessage(Resources.Error_IncorrectData + "\n");
                    }
                }

                //Вывод данных статистики
                void PrintReport(TelemetryPacketModel statistic, 
                                 DateTime fromDate, 
                                 DateTime toDate, 
                                 StatisticsType statisticType, 
                                 int id = 0)
                {
                    if (statistic == null)
                    {
                        Console.WriteLine();
                        PrintRedMessage("\nСтатистика отсутсвует!");
                        return;
                    }
                    
                    string type = statisticType == StatisticsType.Summary ? "Суммарный" : "Среднесуточный";
                    string vehicles = id == 0 ? "всем ТС" : $"ТС id {id}";
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    Console.WriteLine($"\n{type} отчет по {vehicles}");
                    Console.ResetColor();
                    Console.WriteLine(fromDate == toDate ? $"Дата: {fromDate.Date:d}г."
                                                         : $"Период: {fromDate.Date:d}г. - {toDate.Date:d}г.");
                    Console.WriteLine($@"Пройденное расстояние: {Math.Round(statistic.Distance, 2)}км.");
                    Console.WriteLine($@"Потрачено топлива: {Math.Round(statistic.FuelConsumption, 2)}л.");
                    Console.WriteLine($@"Время в движении: {statistic.TravelTime}");
                }

                //Ожидание выполнения Задачи
                void Wait(Task task)
                {
                    PrintGrayMessage("\nПожалуйста, подождите.");
                    while (!task.IsCompleted)
                    {
                        PrintGrayMessage(".");
                        Thread.Sleep(400);
                    }
                }
            }
        }

        void PrintYellowMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        void PrintGrayMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(message);
            Console.ResetColor();
        }

        void PrintRedMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        void PrintGreenMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        void PrintCyanMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        void PrintMagentaMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
