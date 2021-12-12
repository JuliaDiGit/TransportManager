using System;
using TransportManager.API.Controllers;
using TransportManager.Common.Helpers;
using TransportManager.DataEF.Repositories;
using TransportManager.Loggers.Abstract;
using TransportManager.Models;
using TransportManager.Services;
using TransportManager.Services.Decorators.UsersServiceDecorators;
using TransportManager.UI.Properties;

namespace TransportManager.UI
{
    public class Authorization
    {
        private readonly ILogger _logger;
        
        public Authorization(ILogger logger)
        {
            _logger = logger;
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("\nДля входа нажмите - Enter");
                Console.WriteLine("Для регистрации введите - 1");
                Console.WriteLine("Для выхода из программы введите - 0");
                string sCmd = Console.ReadLine();

                if (int.TryParse(sCmd, out int cmd))
                {
                    switch (cmd)
                    {
                        case 1:
                            Registration();
                            break;
                        case 0:
                            MessagePrinter.PrintConsoleColorMessage("\nРабота программы завершена.", ConsoleColor.Green);
                            break;
                    }

                    break;
                }

                UserModel user = StartAuthorization();

                if (user != null)
                {
                    UserInterface userInterface = new UserInterface(user, _logger);
                    userInterface.Start();
                }
            }
        }

        //Авторизация
        UserModel StartAuthorization()
        {
            Console.Write("\nВведите логин: ");
            string login = Console.ReadLine();
            Console.Write(@"Введите пароль: ");
            string password = Console.ReadLine();

            var usersRepository = new UsersRepository();
            var usersService = new UsersService(usersRepository);
            var usersServiceLoggerDecorator = new UsersServiceLoggerDecorator(_logger, usersService);
            var usersServiceEventDecorator = new UsersServiceEventDecorator(usersServiceLoggerDecorator);
            var usersController = new UsersController(usersServiceEventDecorator);

            try
            {
                UserModel user = usersController.GetUserByLoginAsync(login).GetAwaiter().GetResult();
                if (user != null && user.Password == password) return user;
                MessagePrinter.PrintConsoleColorMessage("\nНеверный логин или пароль", ConsoleColor.Red);
                return null;
            }
            catch (Exception e)
            {
                MessagePrinter.PrintConsoleColorMessage("\n" + e.Message, ConsoleColor.Red);
                return null;
            }
        }

        //Регистрация
        void Registration()
        {
            string login;
            string password;

            while (true)
            {
                Console.Write("\nВведите логин (латиница): ");
                login = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(login)) break;
                MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n", ConsoleColor.Red);
            }

            while (true)
            {
                Console.Write(@"Введите пароль (латиница): ");
                password = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(password))
                {
                    while (true)
                    {
                        Console.Write(@"Подтвердите пароль (латиница): ");
                        string password2 = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(password))
                        {
                            if (password2 == password)
                            {
                                break;
                            }

                            MessagePrinter.PrintConsoleColorMessage("Пароли не совпадают!", ConsoleColor.Red);
                        }
                        else MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n", ConsoleColor.Red);
                    }

                    break;
                }

                MessagePrinter.PrintConsoleColorMessage(Resources.Error_IncorrectData + "\n", ConsoleColor.Red);
            }

            try
            {
                var usersRepository = new UsersRepository();
                var usersService = new UsersService(usersRepository);
                var usersServiceLoggerDecorator = new UsersServiceLoggerDecorator(_logger, usersService);
                var usersServiceEventDecorator = new UsersServiceEventDecorator(usersServiceLoggerDecorator);
                var usersController = new UsersController(usersServiceEventDecorator);

                var user = usersController.AddOrUpdateUserAsync(new UserModel 
                                                                { 
                                                                    Login = login, 
                                                                    Password = password
                                                                })
                                           .GetAwaiter()
                                           .GetResult();

                if (user == null) MessagePrinter.PrintConsoleColorMessage("Fail", ConsoleColor.Red);
                else MessagePrinter.PrintConsoleColorMessage("Success", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                MessagePrinter.PrintConsoleColorMessage(e.Message, ConsoleColor.Red);
            }

            Start();
        }
    }
}