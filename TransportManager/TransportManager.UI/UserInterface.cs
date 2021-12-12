using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransportManager.API.Controllers;
using TransportManager.Common.Enums;
using TransportManager.Common.Helpers;
using TransportManager.DataEF.Repositories;
using TransportManager.Generators;
using TransportManager.Loggers.Abstract;
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

                MessagePrinter.PrintConsoleColorMessage(Resources.Message_EnterCommandNumber,
                                                        ConsoleColor.Magenta);
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
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                        break;
                }
            }

            void Menu()
            {
                MessagePrinter.PrintConsoleColorMessage("\nКакую категорию выбрать?",
                                                        ConsoleColor.Cyan);
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

                MessagePrinter.PrintConsoleColorMessage(Resources.Message_EnterCommandNumber,
                                                        ConsoleColor.Magenta);
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
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                        break;
                }

                void CompaniesMenu()
                {
                    MessagePrinter.PrintConsoleColorMessage("\nКакую команду выполнить?",
                                                            ConsoleColor.Cyan);
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
                    MessagePrinter.PrintConsoleColorMessage("Показать все компании\n",
                                                            ConsoleColor.Gray);

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
                            MessagePrinter.PrintConsoleColorMessage("\nСписок всех компаний",
                                                                    ConsoleColor.Yellow);
                            companies.ForEach(company =>
                            {
                                string info = $"id: {company.CompanyId}, " +
                                              $"название: {company.CompanyName}";

                                if (!company.IsDeleted) Console.WriteLine(info);
                                else MessagePrinter.PrintConsoleColorMessage(info + " (удалено!)",
                                                                             ConsoleColor.Red);
                            });
                        }
                        else MessagePrinter.PrintConsoleColorMessage("В данный момент список компаний пуст",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //2. Показать данные компании
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать данные компании\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id компании (цифры): ",
                                                                ConsoleColor.Gray);
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
                                    MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                var notRemoteDrivers = company.Drivers.Where(d => !d.IsDeleted).ToList();
                                var notRemoteVehicles = company.Vehicles.Where(v => !v.IsDeleted).ToList();

                                MessagePrinter.PrintConsoleColorMessage($"\nДанные компании id {companyId}",
                                                                        ConsoleColor.Yellow);
                                Console.WriteLine($"Название: {company.CompanyName}" +
                                                  $"\nКол-во водителей: {notRemoteDrivers.Count}" +
                                                  $"\nКол-во ТС: {notRemoteVehicles.Count}" +
                                                  $"\nДата создания (в базе): {company.CreatedDate}");

                                break;
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                            }
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }
                }

                //3. Показать всех водителей компании
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать всех водителей компании\n",
                                                            ConsoleColor.Gray);

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id компании (цифры): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                            MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                    ConsoleColor.Red);
                            return;
                        }

                        var notRemoteDrivers = company.Drivers.Where(d => !d.IsDeleted).ToList();

                        if (notRemoteDrivers.Count > 0)
                        {
                            MessagePrinter.PrintConsoleColorMessage("\nСписок водителей компании " +
                                                                    $"{company.CompanyName} (id {companyId})",
                                                                    ConsoleColor.Yellow);

                            foreach (var driver in notRemoteDrivers)
                            {
                                Console.WriteLine($"id водителя: {driver.Id}, " +
                                                  $"имя: {driver.Name}");
                            }
                        }
                        else MessagePrinter.PrintConsoleColorMessage("\nВ данный момент у компании " +
                                                                     $"{company.CompanyName} (id {companyId}) ещё нет водителей",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //4. Показать все ТС компании
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать все ТС компании\n",
                                                            ConsoleColor.Gray);

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id компании (цифры): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                            MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                    ConsoleColor.Red);
                            return;
                        }

                        var notRemoteVehicles = company.Vehicles.Where(v => !v.IsDeleted).ToList();

                        if (notRemoteVehicles.Count > 0)
                        {
                            MessagePrinter.PrintConsoleColorMessage($"\nСписок ТС компании {company.CompanyName} (id {companyId})",
                                                                    ConsoleColor.Yellow);

                            foreach (var vehicle in notRemoteVehicles)
                            {
                                Console.WriteLine($"id ТС: {vehicle.Id}, " +
                                                  $"модель: {vehicle.Model}, " +
                                                  $"гос.номер: {vehicle.GovernmentNumber}, " +
                                                  $"дата создания (в базе): {vehicle.CreatedDate}");
                            }
                        }
                        else MessagePrinter.PrintConsoleColorMessage("\nВ данный момент у компании " +
                                                                     $"{company.CompanyName} (id {companyId}) ещё нет ТС",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //5. Добавить компанию
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("Добавить компанию\n",
                                                            ConsoleColor.Gray);

                    string name;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите название компании: ",
                                                                ConsoleColor.Gray);
                        name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(name)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите уникальный id компании (цифры): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId))
                        {
                            break;
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                            MessagePrinter.PrintConsoleColorMessage($"\nКомпания id {companyId} уже существует!",
                                                                    ConsoleColor.Red);
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

                        MessagePrinter.PrintConsoleColorMessage($"\nКомпания {company.CompanyName} " +
                                                                $"(id {companyId}) - добавлена.",
                                                                ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //6. Обновить данные компании
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("Обновить данные компании\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id компании (цифры): ",
                                                                ConsoleColor.Gray);
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
                                    MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                MessagePrinter.PrintConsoleColorMessage($"\nВыбрана компания {company.CompanyName} " +
                                                                        $"(id {companyId})",
                                                                        ConsoleColor.Yellow);

                                string companyName;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("Введите новое название компании: ",
                                                                            ConsoleColor.Gray);
                                    companyName = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(companyName)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
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

                                MessagePrinter.PrintConsoleColorMessage($"\nДанные компании id {updatedCompany.CompanyId} - обновлены, " +
                                                                        $"новое название - {updatedCompany.CompanyName}",
                                                                        ConsoleColor.Green);
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                            }

                            break;
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }
                }

                //7. Удалить компанию
                void Command7()
                {
                    MessagePrinter.PrintConsoleColorMessage("Удалить компанию\n",
                                                            ConsoleColor.Gray);

                    MessagePrinter.PrintConsoleColorMessage("\nВНИМАНИЕ! \nПри удалении компании " +
                                                            "будут удалены все её водители и ТС!\n",
                                                            ConsoleColor.Red);

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id компании " +
                                                                "(для отмены - введите 0): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out companyId)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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

                        if (company == null) MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                                     ConsoleColor.Red);
                        else MessagePrinter.PrintConsoleColorMessage($"\nКомпания {company.CompanyName} (id {companyId}) - удалена",
                                                                     ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
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

                MessagePrinter.PrintConsoleColorMessage(Resources.Message_EnterCommandNumber,
                                                        ConsoleColor.Magenta);
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
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                        break;
                }

                void DriversMenu()
                {
                    MessagePrinter.PrintConsoleColorMessage("\nКакую команду выполнить?",
                                                            ConsoleColor.Cyan);
                    Console.WriteLine(@"1. Показать всех водителей");
                    Console.WriteLine(@"2. Показать данные водителя");
                    Console.WriteLine(@"3. Показать все ТС водителя");
                    Console.WriteLine(@"4. Добавить водителя");
                    Console.WriteLine(@"5. Обновить данные водителя");
                    Console.WriteLine(@"6. Удалить водителя");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");

                    MessagePrinter.PrintConsoleColorMessage("ДЛЯ ТЕСТИРОВАНИЯ",
                                                            ConsoleColor.Green);
                    Console.WriteLine("7. Сгенерировать водителей\n");
                }

                //1. Показать всех водителей
                void Command1()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать всех водителей\n",
                                                            ConsoleColor.Gray);

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
                            MessagePrinter.PrintConsoleColorMessage("\nСписок всех водителей",
                                                                    ConsoleColor.Yellow);
                            drivers.ForEach(driver =>
                            {
                                string info = $"id: {driver.Id}, " +
                                              $"имя: {driver.Name}, " +
                                              $"id компании: {driver.CompanyId}";

                                if (!driver.IsDeleted) Console.WriteLine(info);
                                else MessagePrinter.PrintConsoleColorMessage(info + " (удалено!)",
                                                                             ConsoleColor.Red);
                            });
                        }
                        else MessagePrinter.PrintConsoleColorMessage("\nВ данный момент список водителей пуст",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //2. Показать данные водителя
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать данные водителя\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id водителя (цифры): ",
                                                                ConsoleColor.Gray);
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
                                    MessagePrinter.PrintConsoleColorMessage($"\nВодитель id {id} - не найден",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                var notRemoteVehicles = driver.Vehicles.Where(v => !v.IsDeleted).ToList();

                                MessagePrinter.PrintConsoleColorMessage($"\nДанные водителя id {driver.Id}",
                                                                        ConsoleColor.Yellow);
                                Console.WriteLine($"Имя: {driver.Name}");
                                Console.WriteLine($"Id компании: {driver.CompanyId}");
                                Console.WriteLine($"Кол-во ТС: {notRemoteVehicles.Count}");
                                Console.WriteLine($"Дата создания (в базе): {driver.CreatedDate}");

                                break;
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                                break;
                            }
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }
                }

                //3. Показать все ТС водителя
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать все ТС водителя\n",
                                                            ConsoleColor.Gray);

                    int id;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id водителя (цифры): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out id)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                            MessagePrinter.PrintConsoleColorMessage($"\nВодитель id {id} - не найден",
                                                                    ConsoleColor.Red);
                            return;
                        }

                        var notRemoteVehicles = driver.Vehicles.Where(v => !v.IsDeleted).ToList();

                        if (notRemoteVehicles.Count > 0)
                        {
                            MessagePrinter.PrintConsoleColorMessage($"\nСписок ТС водителя id {id}",
                                                                    ConsoleColor.Yellow);
                            foreach (var vehicle in notRemoteVehicles)
                            {
                                Console.WriteLine($"id ТС: {vehicle.Id}, " +
                                                  $"модель: {vehicle.Model}, " +
                                                  $"гос.номер: {vehicle.GovernmentNumber}, " +
                                                  $"дата создания (в базе): {vehicle.CreatedDate}");
                            }
                        }
                        else MessagePrinter.PrintConsoleColorMessage($"\nВ данный момент у водителя id {id} ещё нет ТС",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //4. Добавить водителя
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("Добавить водителя\n",
                                                            ConsoleColor.Gray);

                    string name;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите имя водителя: ",
                                                                ConsoleColor.Gray);
                        name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(name)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id компании водителя (цифры): ",
                                                                ConsoleColor.Gray);
                        string sIdV = Console.ReadLine();

                        if (int.TryParse(sIdV, out companyId)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                            MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                    ConsoleColor.Red);
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

                        MessagePrinter.PrintConsoleColorMessage($"\nВодитель {driver.Name} - добавлен. " +
                                                                $"Id в базе: {driver.Id}",
                                                                ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //5. Обновить данные водителя
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("Обновить данные водителя\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id водителя (цифры): ",
                                                                ConsoleColor.Gray);
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
                                    MessagePrinter.PrintConsoleColorMessage($"\nВодитель id {id} - не найден",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                MessagePrinter.PrintConsoleColorMessage($"\nВыбран водитель {driver.Name} (id {id}) " +
                                                                        $"из компании id {driver.CompanyId}",
                                                                        ConsoleColor.Yellow);

                                string name;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("Введите новое имя водителя: ",
                                                                            ConsoleColor.Gray);
                                    name = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(name)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
                                }

                                int companyId;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("Введите новый id компании водителя (цифры): ",
                                                                            ConsoleColor.Gray);
                                    string sIdV = Console.ReadLine();

                                    if (int.TryParse(sIdV, out companyId)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
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
                                    MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                            ConsoleColor.Red);
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

                                MessagePrinter.PrintConsoleColorMessage($"\nДанные водителя id {updatedDriver.Id} - обновлены",
                                                                        ConsoleColor.Green);
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                            }

                            break;
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }
                }

                //6. Удалить водителя
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("Удалить водителя\n",
                                                            ConsoleColor.Gray);

                    int id;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id водителя (цифры): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out id)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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

                        if (driver == null) MessagePrinter.PrintConsoleColorMessage($"\nВодитель id {id} - не найден",
                                                                                    ConsoleColor.Red);
                        else MessagePrinter.PrintConsoleColorMessage($"\nВодитель id {id} - удален",
                                                                     ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //7. Сгенерировать водителей
                void Command7()
                {
                    MessagePrinter.PrintConsoleColorMessage("Сгенерировать водителей\n",
                                                            ConsoleColor.Gray);

                    int count;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите количество водителей: ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();
                        if (int.TryParse(sId, out count)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
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

                MessagePrinter.PrintConsoleColorMessage(Resources.Message_EnterCommandNumber,
                                                        ConsoleColor.Magenta);
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
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                        break;
                }

                void VehiclesMenu()
                {
                    MessagePrinter.PrintConsoleColorMessage("\nКакую команду выполнить?",
                                                            ConsoleColor.Cyan);
                    Console.WriteLine(@"1. Показать все ТС");
                    Console.WriteLine(@"2. Показать данные ТС");
                    Console.WriteLine(@"3. Добавить ТС");
                    Console.WriteLine(@"4. Обновить данные ТС");
                    Console.WriteLine(@"5. Удалить ТС");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");

                    MessagePrinter.PrintConsoleColorMessage("ДЛЯ ТЕСТИРОВАНИЯ",
                                                            ConsoleColor.Green);
                    Console.WriteLine("6. Сгенерировать ТС\n");
                }

                //1. Показать все ТС
                void Command1()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать все ТС\n",
                                                            ConsoleColor.Gray);

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
                            MessagePrinter.PrintConsoleColorMessage("\nСписок всех ТС",
                                                                    ConsoleColor.Yellow);
                            vehicles.ForEach(vehicle =>
                            {
                                string info = $"id ТС: {vehicle.Id}, " +
                                              $"id компании: {vehicle.CompanyId}, " +
                                              (vehicle.DriverId > 0 ? $"Id водителя: {vehicle.DriverId}, "
                                                                    : "Id водителя: -, ") +
                                              $"модель: {vehicle.Model}, " +
                                              $"гос.номер: {vehicle.GovernmentNumber}";

                                if (!vehicle.IsDeleted) Console.WriteLine(info);
                                else MessagePrinter.PrintConsoleColorMessage(info + " (удалено!)",
                                                                             ConsoleColor.Red);
                            });
                        }
                        else MessagePrinter.PrintConsoleColorMessage("В данный момент список ТС пуст",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //2. Показать данные ТС
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("Показать данные ТС\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id ТС: ",
                                                                ConsoleColor.Gray);
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
                                    MessagePrinter.PrintConsoleColorMessage($"\nТС id {id} - не найдено",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                MessagePrinter.PrintConsoleColorMessage($"\nДанные ТС id {vehicle.Id}",
                                                                        ConsoleColor.Yellow);
                                Console.WriteLine($"Id компании: {vehicle.CompanyId}");
                                Console.WriteLine(vehicle.DriverId > 0 ? $"Id водителя: {vehicle.DriverId}"
                                                                       : "Id водителя: -");
                                Console.WriteLine($"Модель: {vehicle.Model}");
                                Console.WriteLine($"Гос.номер: {vehicle.GovernmentNumber}");
                                Console.WriteLine($"Дата создания (в базе): {vehicle.CreatedDate}");

                                break;
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                                break;
                            }
                        }
                    }
                }

                //3. Добавить ТС
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("Добавить ТС\n",
                                                            ConsoleColor.Gray);

                    string model;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите модель ТС: ",
                                                                ConsoleColor.Gray);
                        model = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(model)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    string governmentNumber;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите гос.номер ТС (А000АА00): ",
                                                                ConsoleColor.Gray);
                        governmentNumber = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(model)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    int driverId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id водителя ТС (цифры), " +
                                                                "если водителя нет - введите 0: ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();

                        if (int.TryParse(sId, out driverId)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                                MessagePrinter.PrintConsoleColorMessage($"Водитель id {driverId} не найден!",
                                                                        ConsoleColor.Red);
                                return;
                            }
                        }

                        int companyId;
                        if (driverId == 0)
                        {
                            while (true)
                            {
                                MessagePrinter.PrintConsoleColorMessage("Введите id компании ТС (цифры): ",
                                                                        ConsoleColor.Gray);
                                string sIdV = Console.ReadLine();

                                if (int.TryParse(sIdV, out companyId)) break;
                                MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                        ConsoleColor.Red);
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
                                MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                        ConsoleColor.Red);
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
                                                                                     GovernmentNumber = governmentNumber, 
                                                                                     DriverId = driverId == 0 ? (int?) null : driverId, 
                                                                                     CompanyId = companyId
                                                                                 })
                                                        .GetAwaiter()
                                                        .GetResult();

                        MessagePrinter.PrintConsoleColorMessage($"\nТС гос.номер {vehicle.GovernmentNumber} - добавлено. " +
                                                                $"Id в базе: {vehicle.Id}",
                                                                ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //4. Обновить данные ТС
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("Обновить данные ТС\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id ТС: ",
                                                                ConsoleColor.Gray);
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
                                    MessagePrinter.PrintConsoleColorMessage($"\nТС id {id} - не найдено",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                string model;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("Введите новую модель ТС: ",
                                                                            ConsoleColor.Gray);
                                    model = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(model)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
                                }

                                string governmentNumber;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("Введите новый гос.номер ТС (А000АА00): ",
                                                                            ConsoleColor.Gray);
                                    governmentNumber = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(model)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
                                }

                                int driverId;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("Введите новый id водителя ТС (цифры), " +
                                                                            "если водителя нет - введите 0: ",
                                                                            ConsoleColor.Gray);
                                    string sDriverId = Console.ReadLine();

                                    if (int.TryParse(sDriverId, out driverId)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
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
                                        MessagePrinter.PrintConsoleColorMessage($"Водитель id {driverId} не найден!",
                                                                                ConsoleColor.Red);
                                        return;
                                    }
                                }

                                int companyId;
                                if (driverId == 0)
                                {
                                    while (true)
                                    {
                                        MessagePrinter.PrintConsoleColorMessage("Введите новый id компании ТС (цифры): ",
                                                                                ConsoleColor.Gray);
                                        string sCompanyId = Console.ReadLine();

                                        if (int.TryParse(sCompanyId, out companyId)) break;
                                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                                ConsoleColor.Red);
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
                                        MessagePrinter.PrintConsoleColorMessage("\n" + Resources.Error_CompanyNotFound,
                                                                                ConsoleColor.Red);
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
                                                                                   GovernmentNumber = governmentNumber, 
                                                                                   DriverId = driverId == 0 ? (int?) null : driverId, 
                                                                                   CompanyId = companyId
                                                                               })
                                                       .GetAwaiter()
                                                       .GetResult();

                                MessagePrinter.PrintConsoleColorMessage($"\nДанные ТС id {updatedVehicle.Id} - обновлены",
                                                                        ConsoleColor.Green);
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                            }

                            break;
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }
                }

                //5. Удалить ТС
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("Удалить ТС\n",
                                                            ConsoleColor.Gray);

                    int id;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите id ТС (цифры): ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();
                        if (int.TryParse(sId, out id)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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

                        if (vehicle == null) MessagePrinter.PrintConsoleColorMessage($"\nТС id {id} - не найдено",
                                                                                     ConsoleColor.Red);
                        else MessagePrinter.PrintConsoleColorMessage($"\nТС id {id} - удалено",
                                                                     ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //6. Сгенерировать ТС
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("Сгенерировать ТС\n",
                                                            ConsoleColor.Gray);

                    int count;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("Введите количество ТС: ",
                                                                ConsoleColor.Gray);
                        string sId = Console.ReadLine();
                        if (int.TryParse(sId, out count)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
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

                MessagePrinter.PrintConsoleColorMessage(Resources.Message_EnterCommandNumber,
                                                        ConsoleColor.Magenta);
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
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                        break;
                }

                void ReportsMenu()
                {
                    MessagePrinter.PrintConsoleColorMessage("\nКакой отчёт составить?",
                                                            ConsoleColor.Cyan);
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
                    MessagePrinter.PrintConsoleColorMessage("Суммарный отчет по всем ТС за сегодня\n",
                                                            ConsoleColor.Gray);
                    
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
                    MessagePrinter.PrintConsoleColorMessage("Суммарный отчет по всем ТС за период\n",
                                                            ConsoleColor.Gray);
                    
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
                    MessagePrinter.PrintConsoleColorMessage("Среднесуточный отчет по всем ТС за период\n",
                                                            ConsoleColor.Gray);

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
                    MessagePrinter.PrintConsoleColorMessage("Суммарный отчет по одному ТС за сегодня\n",
                                                            ConsoleColor.Gray);

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
                    MessagePrinter.PrintConsoleColorMessage("Суммарный отчет по одному ТС за период\n",
                                                            ConsoleColor.Gray);

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
                    MessagePrinter.PrintConsoleColorMessage("Среднесуточный отчет по одному ТС за период\n",
                                                            ConsoleColor.Gray);

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
                        MessagePrinter.PrintConsoleColorMessage("\nВведите id ТС: ",
                                                                ConsoleColor.Gray);
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
                                MessagePrinter.PrintConsoleColorMessage($"\nТС id {id} - не найдено",
                                                                        ConsoleColor.Red);
                                return null;
                            }
                            catch (Exception e)
                            {
                                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                        ConsoleColor.Red);
                                return null;
                            }
                        }
                        
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData,
                                                                ConsoleColor.Red);
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

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData,
                                                                ConsoleColor.Red);
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
                            if (fromDate > toDate) MessagePrinter.PrintConsoleColorMessage("Конечная дата раньше начальной!\n",
                                                                                           ConsoleColor.Red);
                            break;
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
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
                        MessagePrinter.PrintConsoleColorMessage("\nСтатистика отсутсвует!",
                                                                ConsoleColor.Red);
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
                    MessagePrinter.PrintConsoleColorMessage("\nПожалуйста, подождите.",
                                                            ConsoleColor.Gray);
                    while (!task.IsCompleted)
                    {
                        MessagePrinter.PrintConsoleColorMessage(".",
                                                                ConsoleColor.Gray);
                        Thread.Sleep(400);
                    }
                }
            }
        }
    }
}
