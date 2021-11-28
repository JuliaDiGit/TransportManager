CREATE DATABASE MyDB
GO

USE [MyDB]
GO

CREATE TABLE [dbo].[Users]
(
    [Id] [int] IDENTITY,
    [CreatedDate] [smalldatetime] DEFAULT(GETDATE()) NOT NULL,
    [Login] [varchar] (20) UNIQUE NOT NULL,
    [Password] [nvarchar] (50) NOT NULL,
    [Role] [int] NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Companies]
(
    [Id] [int] IDENTITY,
    [CreatedDate] [smalldatetime] DEFAULT(GETDATE()) NOT NULL,
    [CompanyId] [int] PRIMARY KEY CLUSTERED NOT NULL,
    [CompanyName] [nvarchar] (80) NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Drivers]
(
    [Id] [int] PRIMARY KEY CLUSTERED IDENTITY,
    [CreatedDate] [smalldatetime] DEFAULT(GETDATE()) NOT NULL,
    [CompanyId] [int] NOT NULL,
    [Name] [nvarchar] (50) NOT NULL
) ON [PRIMARY]

ALTER TABLE [dbo].[Drivers] WITH CHECK ADD CONSTRAINT [FK_Drivers_To_CompanyId] FOREIGN KEY(CompanyId)
    REFERENCES [dbo].[Companies] (CompanyId)
    ON DELETE CASCADE

CREATE TABLE [dbo].[Vehicles]
(
    [Id] [int] PRIMARY KEY CLUSTERED IDENTITY,
    [CreatedDate] [smalldatetime] DEFAULT(GETDATE()) NOT NULL,
    [CompanyId] [int] NOT NULL,
    [DriverId] [int] NULL,
    [Model] [nvarchar] (80) NOT NULL,
    [GovernmentNumber] [nvarchar] (9) NOT NULL
) ON [PRIMARY]

ALTER TABLE [dbo].[Vehicles] WITH CHECK ADD CONSTRAINT [FK_Vehicles_To_CompanyId] FOREIGN KEY(CompanyId)
    REFERENCES [dbo].[Companies] (CompanyId)
    ON DELETE CASCADE

ALTER TABLE [dbo].[Vehicles] WITH CHECK ADD CONSTRAINT [FK_Vehicles_To_DriverId] FOREIGN KEY(DriverId)
    REFERENCES [dbo].[Drivers] (Id)

GO

INSERT INTO [Users] (Login, Password, Role) VALUES
('Admin', 'A1', 0),
('Employee', 'E1', 1)


INSERT INTO [Companies] (CompanyId, CompanyName) VALUES
(101, 'Yandex'),
(102, 'Uber'),
(103, 'YouDrive'),
(104, 'Делимобиль'),
(105, 'MyTaxi')

INSERT INTO [Drivers] (CompanyId, Name) VALUES
(101, 'William'),
(105, 'Mary'),
(102, 'Marta'),
(103, 'Bobby'),
(103, 'Jack'),
(101, 'Summer'),
(104, 'Rose'),
(103, 'Deacon'),
(101, 'Rory'),
(102, 'Cara')

INSERT INTO [Vehicles] (CompanyId, DriverId, Model, GovernmentNumber) VALUES
(101, 1, 'Honda Civic', 'О561ВС178'),
(105, 2, 'Nissan GT-R', 'Х260СР178'),
(102, 3, 'Hyundai Tucson', 'К327ТУ98'),
(103, 4, 'Honda CR-V', 'О578ОР198'),
(103, 5, 'Lada Xray', 'В666РМ198'),
(101, 6, 'Toyota Supra', 'В452НМ47'),
(104, 7, 'Audi TT', 'Х500АК78'),
(103, 8, 'Nissan 350Z', 'В202УК47'),
(101, 9, 'Kia Mohave', 'К597РС147'),
(102, NULL, 'BMW M2', 'Н267РЕ47'),
(104, 7, 'Hyundai Solaris', 'К763НК55'),
(105, 7, 'Kia Rio', 'Р811СР63'),
(102, 2, 'Audi R8', 'Р568РВ90'),
(105, NULL, 'BMW X5', 'У540ВУ46'),
(104, 8, 'Lada Vesta', 'С426ОА17')

GO