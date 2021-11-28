CREATE PROCEDURE [dbo].[sp_InsertUser]
	@Login varchar(20) OUTPUT,
	@Password nvarchar(50) OUTPUT,
	@Role int OUTPUT
AS
	INSERT INTO Users (Login, Password, Role)
	VALUES (@Login, @Password, @Role)

	SELECT SCOPE_IDENTITY()
GO

CREATE PROCEDURE [dbo].[sp_UpdateUser]
	@Id int OUTPUT,
	@Login varchar(20) OUTPUT,
	@Password nvarchar(50) OUTPUT,
	@Role int OUTPUT
AS
	UPDATE Users SET
	Login = @Login,
	Password = @Password,
	Role = @Role
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetAllUsers]
AS
    SELECT * FROM Users
GO

CREATE PROCEDURE [dbo].[sp_GetUser]
	@Id int OUTPUT
AS
	SELECT * FROM Users
	WHERE Id = @Id
GO

CREATE PROCEDURE [dbo].[sp_GetUserByLogin]
	@Login varchar(20) OUTPUT
AS
	SELECT * FROM Users
	WHERE Login = @Login
GO

CREATE PROCEDURE [dbo].[sp_DeleteUser]
	@Id int OUTPUT
AS
	DELETE FROM Users WHERE Id = @Id
GO