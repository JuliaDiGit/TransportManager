CREATE PROCEDURE [dbo].[sp_InsertCompany]
	@CompanyId int OUTPUT,
	@CompanyName nvarchar(80) OUTPUT
AS
	INSERT INTO Companies (CompanyId, CompanyName)
	VALUES (@CompanyId, @CompanyName)

	SELECT SCOPE_IDENTITY()
GO

CREATE PROCEDURE [dbo].[sp_UpdateCompany]
	@CompanyId int OUTPUT,
	@CompanyName nvarchar(80) OUTPUT
AS
	UPDATE Companies SET
	CompanyName = @CompanyName
	WHERE CompanyId = @CompanyId
GO

CREATE PROCEDURE [dbo].[sp_DeleteCompanyByCompanyId]
	@CompanyId int OUTPUT
AS
	DELETE FROM Companies WHERE CompanyId = @CompanyId
GO

CREATE PROCEDURE [dbo].[sp_GetAllCompanies]
AS
	SELECT * FROM Companies
GO

CREATE PROCEDURE [dbo].[sp_GetAllCompaniesWithDriversAndVehiclesByNextResult]
AS
	SELECT * FROM Companies
	LEFT JOIN Drivers ON Drivers.CompanyId = Companies.CompanyId
	LEFT JOIN Vehicles ON Vehicles.DriverId = Drivers.Id
	ORDER BY Companies.CompanyId

	SELECT * FROM Companies
	LEFT JOIN Vehicles ON Vehicles.CompanyId = Companies.CompanyId
	ORDER BY Companies.CompanyId
GO

CREATE PROCEDURE [dbo].[sp_GetCompany]
	@Id int OUTPUT
AS
	SELECT * FROM Companies
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetCompanyWithDriversAndVehiclesByNextResult]
	@Id int OUTPUT
AS
	SELECT * FROM Companies
	LEFT JOIN Drivers ON Drivers.CompanyId = Companies.CompanyId
	LEFT JOIN Vehicles ON Vehicles.DriverId = Drivers.Id
	WHERE Companies.Id = @Id
	ORDER BY Drivers.Id

	SELECT * FROM Companies
	LEFT JOIN Vehicles ON Vehicles.CompanyId = Companies.CompanyId
	WHERE Companies.Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetCompanyByCompanyId]
	@CompanyId int OUTPUT
AS
	SELECT * FROM Companies
	WHERE CompanyId = @CompanyId
GO

CREATE PROCEDURE [dbo].[sp_GetCompanyWithDriversAndVehiclesByCompanyIdByNextResult]
	@CompanyId int OUTPUT
AS
	SELECT * FROM Companies
	LEFT JOIN Drivers ON Drivers.CompanyId = @CompanyId
	LEFT JOIN Vehicles ON Vehicles.DriverId = Drivers.Id
	WHERE Companies.CompanyId = @CompanyId
	ORDER BY Drivers.Id

	SELECT * FROM Vehicles
	WHERE CompanyId = @CompanyId
GO