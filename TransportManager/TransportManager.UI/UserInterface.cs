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
                MessagePrinter.PrintConsoleColorMessage("\n?????????? ?????????????????? ???????????????",
                                                        ConsoleColor.Cyan);
                Console.WriteLine("1. ????????????????");
                Console.WriteLine("2. ????????????????");
                Console.WriteLine("3. ?????????????????????? ????????????????");
                Console.WriteLine("4. ???????????? ???? ???????????????????? ????");
                Console.WriteLine("\n0. ?????????? ???? ?????????????? ????????????\n");
            }
        }

        //????????????????
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
                    MessagePrinter.PrintConsoleColorMessage("\n?????????? ?????????????? ???????????????????",
                                                            ConsoleColor.Cyan);
                    Console.WriteLine("1. ???????????????? ?????? ????????????????");
                    Console.WriteLine("2. ???????????????? ???????????? ????????????????");
                    Console.WriteLine("3. ???????????????? ???????? ?????????????????? ????????????????");
                    Console.WriteLine("4. ???????????????? ?????? ???? ????????????????");
                    Console.WriteLine("5. ???????????????? ????????????????");
                    Console.WriteLine("6. ???????????????? ???????????? ????????????????");
                    Console.WriteLine("7. ?????????????? ????????????????");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");
                }

                //1. ???????????????? ?????? ????????????????
                void Command1()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ?????? ????????????????\n",
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
                            MessagePrinter.PrintConsoleColorMessage("\n???????????? ???????? ????????????????",
                                                                    ConsoleColor.Yellow);
                            companies.ForEach(company =>
                            {
                                string info = $"id: {company.CompanyId}, " +
                                              $"????????????????: {company.CompanyName}";

                                if (!company.IsDeleted) Console.WriteLine(info);
                                else MessagePrinter.PrintConsoleColorMessage(info + " (??????????????!)",
                                                                             ConsoleColor.Red);
                            });
                        }
                        else MessagePrinter.PrintConsoleColorMessage("?? ???????????? ???????????? ???????????? ???????????????? ????????",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //2. ???????????????? ???????????? ????????????????
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???????????????? id {companyId}",
                                                                        ConsoleColor.Yellow);
                                Console.WriteLine($"????????????????: {company.CompanyName}" +
                                                  $"\n??????-???? ??????????????????: {notRemoteDrivers.Count}" +
                                                  $"\n??????-???? ????: {notRemoteVehicles.Count}" +
                                                  $"\n???????? ???????????????? (?? ????????): {company.CreatedDate}");

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

                //3. ???????????????? ???????? ?????????????????? ????????????????
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????? ?????????????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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
                            MessagePrinter.PrintConsoleColorMessage("\n???????????? ?????????????????? ???????????????? " +
                                                                    $"{company.CompanyName} (id {companyId})",
                                                                    ConsoleColor.Yellow);

                            foreach (var driver in notRemoteDrivers)
                            {
                                Console.WriteLine($"id ????????????????: {driver.Id}, " +
                                                  $"??????: {driver.Name}");
                            }
                        }
                        else MessagePrinter.PrintConsoleColorMessage("\n?? ???????????? ???????????? ?? ???????????????? " +
                                                                     $"{company.CompanyName} (id {companyId}) ?????? ?????? ??????????????????",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //4. ???????????????? ?????? ???? ????????????????
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ?????? ???? ????????????????\n",
                                                            ConsoleColor.Gray);

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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
                            MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???? ???????????????? {company.CompanyName} (id {companyId})",
                                                                    ConsoleColor.Yellow);

                            foreach (var vehicle in notRemoteVehicles)
                            {
                                Console.WriteLine($"id ????: {vehicle.Id}, " +
                                                  $"????????????: {vehicle.Model}, " +
                                                  $"??????.??????????: {vehicle.GovernmentNumber}, " +
                                                  $"???????? ???????????????? (?? ????????): {vehicle.CreatedDate}");
                            }
                        }
                        else MessagePrinter.PrintConsoleColorMessage("\n?? ???????????? ???????????? ?? ???????????????? " +
                                                                     $"{company.CompanyName} (id {companyId}) ?????? ?????? ????",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //5. ???????????????? ????????????????
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    string name;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ???????????????? ????????????????: ",
                                                                ConsoleColor.Gray);
                        name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(name)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ???????????????????? id ???????????????? (??????????): ",
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
                            MessagePrinter.PrintConsoleColorMessage($"\n???????????????? id {companyId} ?????? ????????????????????!",
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

                        MessagePrinter.PrintConsoleColorMessage($"\n???????????????? {company.CompanyName} " +
                                                                $"(id {companyId}) - ??????????????????.",
                                                                ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //6. ???????????????? ???????????? ????????????????
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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

                                MessagePrinter.PrintConsoleColorMessage($"\n?????????????? ???????????????? {company.CompanyName} " +
                                                                        $"(id {companyId})",
                                                                        ConsoleColor.Yellow);

                                string companyName;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? ???????????????? ????????????????: ",
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

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???????????????? id {updatedCompany.CompanyId} - ??????????????????, " +
                                                                        $"?????????? ???????????????? - {updatedCompany.CompanyName}",
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

                //7. ?????????????? ????????????????
                void Command7()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    MessagePrinter.PrintConsoleColorMessage("\n????????????????! \n?????? ???????????????? ???????????????? " +
                                                            "?????????? ?????????????? ?????? ???? ???????????????? ?? ????!\n",
                                                            ConsoleColor.Red);

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? " +
                                                                "(?????? ???????????? - ?????????????? 0): ",
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
                        else MessagePrinter.PrintConsoleColorMessage($"\n???????????????? {company.CompanyName} (id {companyId}) - ??????????????",
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

        //????????????????
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
                    MessagePrinter.PrintConsoleColorMessage("\n?????????? ?????????????? ???????????????????",
                                                            ConsoleColor.Cyan);
                    Console.WriteLine(@"1. ???????????????? ???????? ??????????????????");
                    Console.WriteLine(@"2. ???????????????? ???????????? ????????????????");
                    Console.WriteLine(@"3. ???????????????? ?????? ???? ????????????????");
                    Console.WriteLine(@"4. ???????????????? ????????????????");
                    Console.WriteLine(@"5. ???????????????? ???????????? ????????????????");
                    Console.WriteLine(@"6. ?????????????? ????????????????");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");

                    MessagePrinter.PrintConsoleColorMessage("?????? ????????????????????????",
                                                            ConsoleColor.Green);
                    Console.WriteLine("7. ?????????????????????????? ??????????????????\n");
                }

                //1. ???????????????? ???????? ??????????????????
                void Command1()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????? ??????????????????\n",
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
                            MessagePrinter.PrintConsoleColorMessage("\n???????????? ???????? ??????????????????",
                                                                    ConsoleColor.Yellow);
                            drivers.ForEach(driver =>
                            {
                                string info = $"id: {driver.Id}, " +
                                              $"??????: {driver.Name}, " +
                                              $"id ????????????????: {driver.CompanyId}";

                                if (!driver.IsDeleted) Console.WriteLine(info);
                                else MessagePrinter.PrintConsoleColorMessage(info + " (??????????????!)",
                                                                             ConsoleColor.Red);
                            });
                        }
                        else MessagePrinter.PrintConsoleColorMessage("\n?? ???????????? ???????????? ???????????? ?????????????????? ????????",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //2. ???????????????? ???????????? ????????????????
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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
                                    MessagePrinter.PrintConsoleColorMessage($"\n???????????????? id {id} - ???? ????????????",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                var notRemoteVehicles = driver.Vehicles.Where(v => !v.IsDeleted).ToList();

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???????????????? id {driver.Id}",
                                                                        ConsoleColor.Yellow);
                                Console.WriteLine($"??????: {driver.Name}");
                                Console.WriteLine($"Id ????????????????: {driver.CompanyId}");
                                Console.WriteLine($"??????-???? ????: {notRemoteVehicles.Count}");
                                Console.WriteLine($"???????? ???????????????? (?? ????????): {driver.CreatedDate}");

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

                //3. ???????????????? ?????? ???? ????????????????
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ?????? ???? ????????????????\n",
                                                            ConsoleColor.Gray);

                    int id;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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
                            MessagePrinter.PrintConsoleColorMessage($"\n???????????????? id {id} - ???? ????????????",
                                                                    ConsoleColor.Red);
                            return;
                        }

                        var notRemoteVehicles = driver.Vehicles.Where(v => !v.IsDeleted).ToList();

                        if (notRemoteVehicles.Count > 0)
                        {
                            MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???? ???????????????? id {id}",
                                                                    ConsoleColor.Yellow);
                            foreach (var vehicle in notRemoteVehicles)
                            {
                                Console.WriteLine($"id ????: {vehicle.Id}, " +
                                                  $"????????????: {vehicle.Model}, " +
                                                  $"??????.??????????: {vehicle.GovernmentNumber}, " +
                                                  $"???????? ???????????????? (?? ????????): {vehicle.CreatedDate}");
                            }
                        }
                        else MessagePrinter.PrintConsoleColorMessage($"\n?? ???????????? ???????????? ?? ???????????????? id {id} ?????? ?????? ????",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //4. ???????????????? ????????????????
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    string name;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ?????? ????????????????: ",
                                                                ConsoleColor.Gray);
                        name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(name)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    int companyId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? ???????????????? (??????????): ",
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

                        MessagePrinter.PrintConsoleColorMessage($"\n???????????????? {driver.Name} - ????????????????. " +
                                                                $"Id ?? ????????: {driver.Id}",
                                                                ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //5. ???????????????? ???????????? ????????????????
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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
                                    MessagePrinter.PrintConsoleColorMessage($"\n???????????????? id {id} - ???? ????????????",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???????????????? {driver.Name} (id {id}) " +
                                                                        $"???? ???????????????? id {driver.CompanyId}",
                                                                        ConsoleColor.Yellow);

                                string name;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? ?????? ????????????????: ",
                                                                            ConsoleColor.Gray);
                                    name = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(name)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
                                }

                                int companyId;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? id ???????????????? ???????????????? (??????????): ",
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

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???????????????? id {updatedDriver.Id} - ??????????????????",
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

                //6. ?????????????? ????????????????
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????? ????????????????\n",
                                                            ConsoleColor.Gray);

                    int id;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? (??????????): ",
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

                        if (driver == null) MessagePrinter.PrintConsoleColorMessage($"\n???????????????? id {id} - ???? ????????????",
                                                                                    ConsoleColor.Red);
                        else MessagePrinter.PrintConsoleColorMessage($"\n???????????????? id {id} - ????????????",
                                                                     ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //7. ?????????????????????????? ??????????????????
                void Command7()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????????????????? ??????????????????\n",
                                                            ConsoleColor.Gray);

                    int count;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ???????????????????? ??????????????????: ",
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

        //????
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
                    MessagePrinter.PrintConsoleColorMessage("\n?????????? ?????????????? ???????????????????",
                                                            ConsoleColor.Cyan);
                    Console.WriteLine(@"1. ???????????????? ?????? ????");
                    Console.WriteLine(@"2. ???????????????? ???????????? ????");
                    Console.WriteLine(@"3. ???????????????? ????");
                    Console.WriteLine(@"4. ???????????????? ???????????? ????");
                    Console.WriteLine(@"5. ?????????????? ????");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");

                    MessagePrinter.PrintConsoleColorMessage("?????? ????????????????????????",
                                                            ConsoleColor.Green);
                    Console.WriteLine("6. ?????????????????????????? ????\n");
                }

                //1. ???????????????? ?????? ????
                void Command1()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ?????? ????\n",
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
                            MessagePrinter.PrintConsoleColorMessage("\n???????????? ???????? ????",
                                                                    ConsoleColor.Yellow);
                            vehicles.ForEach(vehicle =>
                            {
                                string info = $"id ????: {vehicle.Id}, " +
                                              $"id ????????????????: {vehicle.CompanyId}, " +
                                              (vehicle.DriverId > 0 ? $"Id ????????????????: {vehicle.DriverId}, "
                                                                    : "Id ????????????????: -, ") +
                                              $"????????????: {vehicle.Model}, " +
                                              $"??????.??????????: {vehicle.GovernmentNumber}";

                                if (!vehicle.IsDeleted) Console.WriteLine(info);
                                else MessagePrinter.PrintConsoleColorMessage(info + " (??????????????!)",
                                                                             ConsoleColor.Red);
                            });
                        }
                        else MessagePrinter.PrintConsoleColorMessage("?? ???????????? ???????????? ???????????? ???? ????????",
                                                                     ConsoleColor.Yellow);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //2. ???????????????? ???????????? ????
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????????? ????\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ????: ",
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
                                    MessagePrinter.PrintConsoleColorMessage($"\n???? id {id} - ???? ??????????????",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???? id {vehicle.Id}",
                                                                        ConsoleColor.Yellow);
                                Console.WriteLine($"Id ????????????????: {vehicle.CompanyId}");
                                Console.WriteLine(vehicle.DriverId > 0 ? $"Id ????????????????: {vehicle.DriverId}"
                                                                       : "Id ????????????????: -");
                                Console.WriteLine($"????????????: {vehicle.Model}");
                                Console.WriteLine($"??????.??????????: {vehicle.GovernmentNumber}");
                                Console.WriteLine($"???????? ???????????????? (?? ????????): {vehicle.CreatedDate}");

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

                //3. ???????????????? ????
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ????\n",
                                                            ConsoleColor.Gray);

                    string model;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ???????????? ????: ",
                                                                ConsoleColor.Gray);
                        model = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(model)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    string governmentNumber;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ??????.?????????? ???? (??000????00): ",
                                                                ConsoleColor.Gray);
                        governmentNumber = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(model)) break;
                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }

                    int driverId;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? ???? (??????????), " +
                                                                "???????? ???????????????? ?????? - ?????????????? 0: ",
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
                                MessagePrinter.PrintConsoleColorMessage($"???????????????? id {driverId} ???? ????????????!",
                                                                        ConsoleColor.Red);
                                return;
                            }
                        }

                        int companyId;
                        if (driverId == 0)
                        {
                            while (true)
                            {
                                MessagePrinter.PrintConsoleColorMessage("?????????????? id ???????????????? ???? (??????????): ",
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

                        MessagePrinter.PrintConsoleColorMessage($"\n???? ??????.?????????? {vehicle.GovernmentNumber} - ??????????????????. " +
                                                                $"Id ?? ????????: {vehicle.Id}",
                                                                ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //4. ???????????????? ???????????? ????
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????? ???????????? ????\n",
                                                            ConsoleColor.Gray);

                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ????: ",
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
                                    MessagePrinter.PrintConsoleColorMessage($"\n???? id {id} - ???? ??????????????",
                                                                            ConsoleColor.Red);
                                    break;
                                }

                                string model;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? ???????????? ????: ",
                                                                            ConsoleColor.Gray);
                                    model = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(model)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
                                }

                                string governmentNumber;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? ??????.?????????? ???? (??000????00): ",
                                                                            ConsoleColor.Gray);
                                    governmentNumber = Console.ReadLine();

                                    if (!string.IsNullOrWhiteSpace(model)) break;
                                    MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                            ConsoleColor.Red);
                                }

                                int driverId;
                                while (true)
                                {
                                    MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? id ???????????????? ???? (??????????), " +
                                                                            "???????? ???????????????? ?????? - ?????????????? 0: ",
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
                                        MessagePrinter.PrintConsoleColorMessage($"???????????????? id {driverId} ???? ????????????!",
                                                                                ConsoleColor.Red);
                                        return;
                                    }
                                }

                                int companyId;
                                if (driverId == 0)
                                {
                                    while (true)
                                    {
                                        MessagePrinter.PrintConsoleColorMessage("?????????????? ?????????? id ???????????????? ???? (??????????): ",
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

                                MessagePrinter.PrintConsoleColorMessage($"\n???????????? ???? id {updatedVehicle.Id} - ??????????????????",
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

                //5. ?????????????? ????
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????? ????\n",
                                                            ConsoleColor.Gray);

                    int id;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? id ???? (??????????): ",
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

                        if (vehicle == null) MessagePrinter.PrintConsoleColorMessage($"\n???? id {id} - ???? ??????????????",
                                                                                     ConsoleColor.Red);
                        else MessagePrinter.PrintConsoleColorMessage($"\n???? id {id} - ??????????????",
                                                                     ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n" + e.Message,
                                                                ConsoleColor.Red);
                    }
                }

                //6. ?????????????????????????? ????
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????????????????? ????\n",
                                                            ConsoleColor.Gray);

                    int count;
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("?????????????? ???????????????????? ????: ",
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

        //????????????
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
                    MessagePrinter.PrintConsoleColorMessage("\n?????????? ?????????? ???????????????????",
                                                            ConsoleColor.Cyan);
                    Console.WriteLine(@"1. ?????????????????? ?????????? ???? ???????? ???? ???? ??????????????");
                    Console.WriteLine(@"2. ?????????????????? ?????????? ???? ???????? ???? ???? ????????????");
                    Console.WriteLine(@"3. ???????????????????????????? ?????????? ???? ???????? ???? ???? ????????????");
                    Console.WriteLine(@"4. ?????????????????? ?????????? ???? ???????????? ???? ???? ??????????????");
                    Console.WriteLine(@"5. ?????????????????? ?????????? ???? ???????????? ???? ???? ????????????");
                    Console.WriteLine(@"6. ???????????????????????????? ?????????? ???? ???????????? ???? ???? ????????????");
                    Console.WriteLine("\n" + Resources.Message_ReturnToMainMenu + "\n");
                }

                //1. ?????????????????? ?????????? ???? ???????? ???? ???? ??????????????
                void Command1()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????????? ?????????? ???? ???????? ???? ???? ??????????????\n",
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

                //2. ?????????????????? ?????????? ???? ???????? ???? ???? ????????????
                void Command2()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????????? ?????????? ???? ???????? ???? ???? ????????????\n",
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

                //3. ???????????????????????????? ?????????? ???? ???????? ???? ???? ????????????
                void Command3()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????????????????? ?????????? ???? ???????? ???? ???? ????????????\n",
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

                //4. ?????????????????? ?????????? ???? ???????????? ???? ???? ??????????????
                void Command4()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????????? ?????????? ???? ???????????? ???? ???? ??????????????\n",
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

                //5. ?????????????????? ?????????? ???? ???????????? ???? ???? ????????????
                void Command5()
                {
                    MessagePrinter.PrintConsoleColorMessage("?????????????????? ?????????? ???? ???????????? ???? ???? ????????????\n",
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
                
                //6. ???????????????????????????? ?????????? ???? ???????????? ???? ???? ????????????
                void Command6()
                {
                    MessagePrinter.PrintConsoleColorMessage("???????????????????????????? ?????????? ???? ???????????? ???? ???? ????????????\n",
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

                //???????????? id ???? ?? ?????????? ?????????? ???? ?? ????
                VehicleModel FindVehicle()
                {
                    while (true)
                    {
                        MessagePrinter.PrintConsoleColorMessage("\n?????????????? id ????: ",
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
                                MessagePrinter.PrintConsoleColorMessage($"\n???? id {id} - ???? ??????????????",
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

                //???????????? ?? ???????????????? ?????????????????? ??????
                void ReadDate(out DateTime fromDate, out DateTime toDate)
                {
                    while (true)
                    {
                        Console.Write("\n?????????????? ?????????????????? ???????? ?? ?????????????? \"????/????/????????\": ");
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
                        Console.Write("?????????????? ???????????????? ???????? ?? ?????????????? \"????/????/????????\": ");
                        string sDate2 = Console.ReadLine();

                        if (DateTime.TryParse(sDate2,
                            CultureInfo.CreateSpecificCulture("en-GB"), 
                            DateTimeStyles.None, 
                            out toDate))
                        {
                            if (fromDate > toDate) MessagePrinter.PrintConsoleColorMessage("???????????????? ???????? ???????????? ??????????????????!\n",
                                                                                           ConsoleColor.Red);
                            break;
                        }

                        MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n",
                                                                ConsoleColor.Red);
                    }
                }

                //?????????? ???????????? ????????????????????
                void PrintReport(TelemetryPacketModel statistic, 
                                 DateTime fromDate, 
                                 DateTime toDate, 
                                 StatisticsType statisticType, 
                                 int id = 0)
                {
                    if (statistic == null)
                    {
                        Console.WriteLine();
                        MessagePrinter.PrintConsoleColorMessage("\n???????????????????? ????????????????????!",
                                                                ConsoleColor.Red);
                        return;
                    }
                    
                    string type = statisticType == StatisticsType.Summary ? "??????????????????" : "????????????????????????????";
                    string vehicles = id == 0 ? "???????? ????" : $"???? id {id}";
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    Console.WriteLine($"\n{type} ?????????? ???? {vehicles}");
                    Console.ResetColor();
                    Console.WriteLine(fromDate == toDate ? $"????????: {fromDate.Date:d}??."
                                                         : $"????????????: {fromDate.Date:d}??. - {toDate.Date:d}??.");
                    Console.WriteLine($@"???????????????????? ????????????????????: {Math.Round(statistic.Distance, 2)}????.");
                    Console.WriteLine($@"?????????????????? ??????????????: {Math.Round(statistic.FuelConsumption, 2)}??.");
                    Console.WriteLine($@"?????????? ?? ????????????????: {statistic.TravelTime}");
                }

                //???????????????? ???????????????????? ????????????
                void Wait(Task task)
                {
                    MessagePrinter.PrintConsoleColorMessage("\n????????????????????, ??????????????????.",
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
