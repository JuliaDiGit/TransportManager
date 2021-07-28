CREATE PROCEDURE [dbo].[sp_InsertVehicle]
	@CompanyId int OUTPUT,
	@DriverId int = NULL OUTPUT,
	@Model nvarchar(80) OUTPUT,
	@GovernmentNumber nvarchar(9) OUTPUT
AS
	INSERT INTO Vehicles(CompanyId, DriverId, Model, GovernmentNumber)
	VALUES (@CompanyId, @DriverId, @Model, @GovernmentNumber)

	SELECT SCOPE_IDENTITY()
GO

CREATE PROCEDURE [dbo].[sp_UpdateVehicle]
	@Id int OUTPUT,
	@CompanyId int OUTPUT,
	@DriverId int = NULL OUTPUT,
	@Model nvarchar(80) OUTPUT,
	@GovernmentNumber nvarchar(8) OUTPUT
AS
	UPDATE Vehicles SET
	CompanyId = @CompanyId,
	DriverId = @DriverId,
	Model = @Model,
	GovernmentNumber = @GovernmentNumber
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_DeleteVehicle]
	@Id int OUTPUT
AS
	DELETE FROM Vehicles WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetAllVehicles]
AS
    SELECT * FROM Vehicles
GO

CREATE PROCEDURE [dbo].[sp_GetVehicle]
	@Id int OUTPUT
AS
	SELECT * FROM Vehicles
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetVehiclesByDriverId]
	@DriverId int OUTPUT
AS
	SELECT * FROM Vehicles
	WHERE DriverId = @DriverId
GO

CREATE PROCEDURE [dbo].[sp_GetVehiclesByCompanyId]
	@CompanyId int OUTPUT
AS
	SELECT * FROM Vehicles
	WHERE CompanyId = @CompanyId]
GO