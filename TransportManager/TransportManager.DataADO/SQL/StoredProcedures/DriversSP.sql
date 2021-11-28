CREATE PROCEDURE [dbo].[sp_InsertDriver]
	@CompanyId int OUTPUT,
	@Name nvarchar(50) OUTPUT
AS
	INSERT INTO Drivers (CompanyId, Name)
	VALUES (@CompanyId, @Name)

	SELECT SCOPE_IDENTITY()
GO

CREATE PROCEDURE [dbo].[sp_UpdateDriver]
	@Id int OUTPUT,
	@CompanyId int OUTPUT,
	@Name nvarchar(50) OUTPUT
AS
	UPDATE Drivers SET
	CompanyId = @CompanyId,
	Name = @Name
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_DeleteDriver]
	@Id int OUTPUT
AS
	DELETE FROM Drivers WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetAllDrivers]
AS
	SELECT * FROM Drivers
GO

CREATE PROCEDURE [dbo].[sp_GetAllDriversWithVehicles]
AS
	SELECT * FROM Drivers
	LEFT JOIN Vehicles ON Drivers.Id = Vehicles.DriverId
	ORDER BY Drivers.Id
GO

CREATE PROCEDURE [dbo].[sp_GetDriver]
	@Id int OUTPUT
AS
	SELECT * FROM Drivers
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetDriverWithVehicles]
	@Id int OUTPUT
AS
	SELECT * FROM Drivers
	LEFT JOIN Vehicles ON Drivers.Id = Vehicles.DriverId
	WHERE Drivers.Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetDriversByCompanyId]
	@CompanyId int OUTPUT
AS
	SELECT * FROM Drivers
	WHERE CompanyId = @CompanyId
GO

CREATE PROCEDURE [dbo].[sp_GetDriversWithVehiclesByCompanyId]
	@CompanyId int OUTPUT
AS
	SELECT * FROM Drivers
	LEFT JOIN Vehicles ON Drivers.Id = Vehicles.DriverId
	WHERE Drivers.CompanyId = @CompanyId
	ORDER BY Drivers.Id
GO
