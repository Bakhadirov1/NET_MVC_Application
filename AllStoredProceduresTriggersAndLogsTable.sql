

/*
 Most of the tables does not require any additional indexes, because all the
 operations made in the database are too small
 The reason for that is that the main Employees table is
 divided into several tables which makes usage of additional indexes
 not efficient
 when it comes to Projects and Positions tables
 they are also too small for implementing additional indexes
*/

CREATE OR ALTER PROCEDURE insertProject @ProjectName nvarchar(50)
AS
BEGIN
	INSERT INTO Projects(ProjectName) VALUES (@ProjectName)
	RETURN;
END

GO

CREATE OR ALTER PROCEDURE deleteProject @ProjectId int
AS
BEGIN
	DELETE FROM Positions WHERE @ProjectId = @ProjectId
END

GO

CREATE OR ALTER PROCEDURE updateProject @ProjectId int,
@ProjectName nvarchar(50)
AS
BEGIN
	UPDATE Projects
	SET ProjectName = @ProjectName
	WHERE ProjectId = @ProjectId
END

GO

CREATE OR ALTER PROCEDURE getProjects
AS
BEGIN
	SELECT * FROM Projects
	RETURN;
END

GO

CREATE OR ALTER PROCEDURE getProject
@ProjectId int
AS
BEGIN
	SELECT * FROM Projects
	WHERE ProjectId = @ProjectId
	RETURN;
END

GO

CREATE OR ALTER PROCEDURE insertPosition @PositionName nvarchar(50)
AS
BEGIN
	INSERT INTO Positions(PositionName) VALUES (@PositionName)
	RETURN;
END

GO

CREATE OR ALTER PROCEDURE deletePosition @PositionId int
AS
BEGIN
	DELETE FROM Positions WHERE PositionId = @PositionId
END
GO

CREATE OR ALTER PROCEDURE updatePosition @PositionId int,
@PositionName nvarchar(50)
AS
BEGIN
	UPDATE Positions
	SET PositionName = @PositionName
	WHERE PositionId = @PositionId
END

GO

CREATE OR ALTER PROCEDURE getPositions
AS
BEGIN
	SELECT * FROM Positions
	RETURN;
END

GO

CREATE OR ALTER PROCEDURE getPosition
@PositionId int
AS
BEGIN
	SELECT * FROM Positions
	WHERE PositionId = @PositionId
	RETURN;
END


GO

CREATE OR ALTER PROCEDURE insertEmployee
@ProjectId int,
@PositionId int,
@Name nvarchar(50),
@LastName nvarchar(50),
@Photo varbinary(MAX),
@DateOfBirth date,
@PhoneNumber nvarchar(50),
@Email nvarchar(50),
@Address nvarchar(200),
@City nvarchar(200)
AS
BEGIN
	DECLARE @NewEmployeeId TABLE(Id int)

	INSERT INTO Employees(PositionId, ProjectId)
	OUTPUT inserted.EmployeeId INTO @NewEmployeeId(ID)
	VALUES (@PositionId, @ProjectId);

	INSERT INTO EmployeePersonalInfo(EmployeeId, Name, LastName, Photo, DateOfBirth)
	VALUES ((SELECT Id FROM @NewEmployeeId), @Name, @LastName, @Photo, @DateOfBirth)

	INSERT INTO EmployeeContacts(EmployeeId, PhoneNumber, Email)
	VALUES ((SELECT Id FROM @NewEmployeeId), @PhoneNumber, @Email)

	INSERT INTO EmployeeAddressInfo(EmployeeId, Address, City)
	VALUES ((SELECT Id FROM @NewEmployeeId), @Address, @City)

	RETURN;

END

GO

CREATE OR ALTER PROCEDURE getEmployees
AS
BEGIN
	SELECT * FROM Employees emp
	JOIN EmployeePersonalInfo empPerInfo ON empPerInfo.EmployeeId = emp.EmployeeId
	JOIN EmployeeContacts empCon ON empCon.EmployeeId = emp.EmployeeId
	JOIN EmployeeAddressInfo empAdd ON empAdd.EmployeeId = emp.EmployeeId
	JOIN Projects proj ON proj.ProjectId = emp.ProjectId
	JOIN Positions pos ON pos.PositionId = emp.PositionId
END


GO

CREATE OR ALTER PROCEDURE getOneEmployee
@EmployeeId int
AS
BEGIN
	SELECT emp.EmployeeId, Name, LastName, Photo, DateOfBirth,
	PhoneNumber, Email, Address, City, pos.PositionId, pos.PositionName,
	proj.ProjectId, proj.ProjectName
	FROM Employees emp
	JOIN EmployeePersonalInfo emPer ON emPer.EmployeeId = emp.EmployeeId
	JOIN EmployeeContacts emCon ON emCon.EmployeeId = emp.EmployeeId
	JOIN EmployeeAddressInfo emAd ON emAd.EmployeeId = emp.EmployeeId
	JOIN Positions pos ON pos.PositionId = emp.PositionId
	JOIN Projects proj on proj.ProjectId = emp.ProjectId
	WHERE emp.EmployeeId = @EmployeeId
	RETURN;
END

GO

CREATE OR ALTER PROCEDURE deleteOneEmployee
@EmployeeId int
AS
BEGIN
	DELETE FROM EmployeeAddressInfo WHERE EmployeeId = @EmployeeId
	DELETE FROM EmployeeContacts WHERE EmployeeId = @EmployeeId
	DELETE FROM EmployeePersonalInfo WHERE EmployeeId = @EmployeeId
	DELETE FROM Employees WHERE EmployeeId = @EmployeeId
END

GO

CREATE OR ALTER PROCEDURE updateOneEmployee
@EmployeeId int,
@ProjectId int,
@PositionId int,
@Name nvarchar(50),
@LastName nvarchar(50),
@Photo varbinary(MAX),
@DateOfBirth date,
@PhoneNumber nvarchar(50),
@Email nvarchar(50),
@Address nvarchar(200),
@City nvarchar(200)
AS
BEGIN
	UPDATE Employees
	SET PositionId = @PositionId, ProjectId = @ProjectId
	WHERE EmployeeId = @EmployeeId

	UPDATE EmployeePersonalInfo
	SET Name = @Name, LastName = @LastName, Photo = @Photo, DateOfBirth = @DateOfBirth
	WHERE EmployeeId = @EmployeeId

	UPDATE EmployeeContacts
	SET PhoneNumber = @PhoneNumber, Email = @Email
	WHERE EmployeeId = @EmployeeId

	UPDATE EmployeeAddressInfo
	SET Address = @Address, City = @City
	WHERE EmployeeId = @EmployeeId
END

GO

CREATE OR ALTER PROCEDURE filterEmployees
@Name nvarchar(50) = null,
@LastName nvarchar(50) = null,
@PhoneNumber nvarchar(50) = null,
@DateOfBirth nvarchar(MAX) = null,
@PositionId int = null,
@ProjectId int = null,
@SortName nvarchar(50) = 'Name',
@Sort nvarchar(50) = 'DESC',
@Page int = 1,
@PageSize int = 3,
@totalCount int OUT
AS
BEGIN
	DECLARE @SQL nvarchar(MAX);
	DECLARE @SQLCount nvarchar(MAX) = 'SELECT @totalCountOUT = count(*) FROM Employees emp ';
	DECLARE @SQLWhere nvarchar(MAX) = ' WHERE -1 = -1';
	DECLARE @SQLJoins nvarchar(max) = ' ';
	SET @SQL = '
			SELECT emp.EmployeeId, Name, LastName, Photo, DateOfBirth,
	PhoneNumber, Email, Address, City, convert(int, pos.PositionId) AS PositionId, pos.PositionName,
	convert(int, proj.ProjectId) as ProjectId, proj.ProjectName
	FROM Employees emp
	'

	SET @SQLJoins = 'JOIN EmployeePersonalInfo epf ON emp.EmployeeId = epf.EmployeeId
	JOIN EmployeeContacts ec ON emp.EmployeeId = ec.EmployeeId
	JOIN EmployeeAddressInfo eaf ON emp.EmployeeId = eaf.EmployeeId
	JOIN Positions pos ON pos.PositionId = emp.PositionId
	JOIN Projects proj ON proj.ProjectId = emp.ProjectId
	'
	SET @SQL = @SQL + @SQLJoins
	SET @SQLCount = @SQLCount + @SQLJoins
	

	IF @Name IS NOT NULL AND @Name <> ''
		SET @SQLWhere = @SQLWhere + 'AND Name like ''%' + @Name + '%'''

	IF @LastName IS NOT NULL AND @LastName <> ''
		SET @SQLWhere = @SQLWhere + 'AND LastName like ''%' + @LastName + '%'''

	IF @PhoneNumber IS NOT NULL AND @PhoneNumber <> ''
		SET @SQLWhere = @SQLWhere + 'AND PhoneNumber like ''%' + @PhoneNumber + '%'''

	IF @DateOfBirth IS NOT NULL AND @DateOfBirth <> ''
		SET @SQLWhere = @SQLWhere + 'AND DateOfBirth = ''' + @DateOfBirth + ''''

	IF @PositionId IS NOT NULL AND @PositionId <> 0
		SET @SQLWhere = @SQLWhere + 'AND pos.PositionId = ' + CAST(@PositionId as varchar)

	IF @ProjectId IS NOT NULL AND @ProjectId <> 0
		SET @SQLWhere = @SQLWhere + 'AND proj.ProjectId = ' + CAST(@ProjectId as varchar)

	SET @SQLCount = @SQLCount + @SQLWhere
	IF len(ltrim(rtrim(@SortName))) <= 0 OR @SortName IS NULL
		SET @SortName = 'Name'
	SET @SQLWhere = @SQLWhere + ' ORDER BY ' + LTRIM(RTRIM(@SortName)) + ' ' + @Sort

	IF @Page IS NULL OR @Page = 0 
		SET @Page = 1
	if @PageSize IS NULL OR @PageSize = 0
		SET @PageSize = 3

	DECLARE @Offset int = (@Page - 1) * @PageSize;
	SET @SQLWhere += ' OFFSET ' + CAST(@Offset as nvarchar) + ' ROWS FETCH NEXT ' + CAST(@PageSize as nvarchar) + ' ROWS ONLY'
	IF LEN(@SQLWhere) > 0
		SET @SQL = @SQL + @SQLWhere
	PRINT(@SQL)
	DECLARE @paramCount nvarchar(1000) =  '@TotalCountOUT int OUT'
	DECLARE @paramMainSQL nvarchar(1000) =  ' '
	EXEC(@SQL)
	exec sp_executesql @SQLCount, @paramCount , @totalCountOUT = @totalCount OUT

END

GO


CREATE OR ALTER PROCEDURE employeeExportXML
@Name nvarchar(50) = null,
@LastName nvarchar(50) = null,
@PhoneNumber nvarchar(50) = null,
@DateOfBirth nvarchar(MAX) = null,
@PositionId int = null,
@ProjectId int = null,
@SortName nvarchar(50) = 'Name',
@Sort nvarchar(50) = 'DESC',
@Page int = 1,
@PageSize int = 3,
@xml xml OUT
AS
BEGIN
	DECLARE @cnt int;

	DECLARE @tab table (
		[EmployeeId] int,
		[Name] nvarchar(50),
		[LastName] nvarchar(50),
		[Photo] varbinary(MAX),
		[DateOfBirth] date,
		[PhoneNumber] nvarchar(50),
		[Email] nvarchar(50),
		[Address] nvarchar(200),
		[City] nvarchar(200),
		[PositionId] int,
		[PositionName] nvarchar(50),
		[ProjectId] int,
		[ProjectName] nvarchar(50)
	)

	INSERT INTO @tab
	EXEC filterEmployees @Name, @LastName, @PhoneNumber, @DateOfBirth, @PositionId,
	@ProjectId, @SortName, @Sort, @Page, @PageSize,
	@totalCount = @cnt OUT

	
	SET @xml = (
		SELECT EmployeeId, Name, LastName, DateOfBirth, PhoneNumber, Email, Address,
		City, PositionId, PositionName, ProjectId, ProjectName
		FROM @tab
		FOR XML PATH('Employee'), ROOT('Employees')
	)
END

GO


CREATE OR ALTER PROCEDURE employeeExportJSON
@Name nvarchar(50) = null,
@LastName nvarchar(50) = null,
@PhoneNumber nvarchar(50) = null,
@DateOfBirth nvarchar(MAX) = null,
@PositionId int = null,
@ProjectId int = null,
@SortName nvarchar(50) = 'Name',
@Sort nvarchar(50) = 'DESC',
@Page int = 1,
@PageSize int = 3,
@JSON varchar(max) OUT
AS
BEGIN
	DECLARE @cnt int;

	DECLARE @tab table (
		[EmployeeId] int,
		[Name] nvarchar(50),
		[LastName] nvarchar(50),
		[Photo] varbinary(MAX),
		[DateOfBirth] date,
		[PhoneNumber] nvarchar(50),
		[Email] nvarchar(50),
		[Address] nvarchar(200),
		[City] nvarchar(200),
		[PositionId] int,
		[PositionName] nvarchar(50),
		[ProjectId] int,
		[ProjectName] nvarchar(50)
	)

	INSERT INTO @tab
	EXEC filterEmployees @Name, @LastName, @PhoneNumber, @DateOfBirth, @PositionId,
	@ProjectId, @SortName, @Sort, @Page, @PageSize,
	@totalCount = @cnt OUT

	
	SET @JSON = (
		SELECT EmployeeId, Name, LastName, DateOfBirth, PhoneNumber, Email, Address,
		City, PositionId, PositionName, ProjectId, ProjectName
		FROM @tab
		FOR JSON PATH
	)
END

GO


create or alter procedure employeeExportCSV
@Name nvarchar(50) = null,
@LastName nvarchar(50) = null,
@PhoneNumber nvarchar(50) = null,
@DateOfBirth nvarchar(MAX) = null,
@PositionId int = null,
@ProjectId int = null,
@SortName nvarchar(50) = 'Name',
@Sort nvarchar(50) = 'DESC',
@Page int = 1,
@PageSize int = 3,
@csv varchar(max) OUT
AS
BEGIN
DECLARE @cnt int;
	DECLARE @tab table (
		[EmployeeId] int,
		[Name] nvarchar(50),
		[LastName] nvarchar(50),
		[Photo] varbinary(MAX),
		[DateOfBirth] date,
		[PhoneNumber] nvarchar(50),
		[Email] nvarchar(50),
		[Address] nvarchar(200),
		[City] nvarchar(200),
		[PositionId] int,
		[PositionName] nvarchar(50),
		[ProjectId] int,
		[ProjectName] nvarchar(50)
	)

	INSERT INTO @tab
	EXEC filterEmployees @Name, @LastName, @PhoneNumber, @DateOfBirth, @PositionId,
	@ProjectId, @SortName, @Sort, @Page, @PageSize,
	@totalCount = @cnt OUT
	
	SET @csv = (
		select string_agg(CONCAT(
		EmployeeId, ',', Name, ',', LastName, ',', DateOfBirth, ',',
		PhoneNumber, ',', Email, ',', Address, ',',
		City, ',', PositionId, ',', PositionName, ',', ProjectId, ',', ProjectName), char(13))
    from @tab
	)
end

GO


-- Log table --
CREATE TABLE ModificationsLog (
	Id int IDENTITY (1, 1) NOT NULL,
	TableName varchar(200),
	Operation varchar(100),
	OperationDate date,
	UserName varchar(200),
	InsertedData xml,
	DeletedData xml
	CONSTRAINT pk_ModificationsLog_Id PRIMARY KEY (Id)
)

GO

CREATE OR ALTER TRIGGER changesLogForEmployee
ON EmployeeAddressInfo
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @operation VARCHAR(100)
		SET @operation = CASE
				WHEN EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
					THEN 'Update'
				WHEN EXISTS(SELECT * FROM inserted)
					THEN 'Insert'
				WHEN EXISTS(SELECT * FROM deleted)
					THEN 'Delete'
				ELSE NULL
		END

		DECLARE @xmlDel xml;
		DECLARE @xmlIns xml;

		PRINT(@Operation)

		IF @operation = 'Delete'
		BEGIN
			 SET @xmlDel = (SELECT d.EmployeeId, Name, LastName, Photo, DateOfBirth,
				PhoneNumber, Email, Address, City, pos.PositionId, pos.PositionName,
				proj.ProjectId, proj.ProjectName
				FROM deleted d
				JOIN Employees emp ON emp.EmployeeId = d.EmployeeId
				JOIN EmployeeContacts emCon ON emCon.EmployeeId = d.EmployeeId
				JOIN EmployeePersonalInfo emPin ON emPin.EmployeeId = d.EmployeeId
				JOIN Positions pos ON pos.PositionId = emp.PositionId
				JOIN Projects proj on proj.ProjectId = emp.ProjectId
				FOR XML PATH('Employee'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, DeletedData)
			VALUES ('Employees, EmployeePersonalInfo, EmployeeContacts, EmployeeAddressInfo',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlDel)
		END

		IF @operation = 'Insert'
		BEGIN

		SET @xmlIns = (SELECT i.EmployeeId, Name, LastName, Photo, DateOfBirth,
				PhoneNumber, Email, Address, City, pos.PositionId, pos.PositionName,
				proj.ProjectId, proj.ProjectName
				FROM inserted i
				JOIN Employees emp ON emp.EmployeeId = i.EmployeeId
				JOIN EmployeeContacts emCon ON emCon.EmployeeId = i.EmployeeId
				JOIN EmployeePersonalInfo emPin ON emPin.EmployeeId = i.EmployeeId
				JOIN Positions pos ON pos.PositionId = emp.PositionId
				JOIN Projects proj on proj.ProjectId = emp.ProjectId
				FOR XML PATH('Employee'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, InsertedData)
			VALUES ('Employees, EmployeePersonalInfo, EmployeeContacts, EmployeeAddressInfo',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlIns)
		END
 
		IF @operation = 'Update'
		BEGIN
			SET @xmlDel = (SELECT d.EmployeeId, Name, LastName, Photo, DateOfBirth,
					PhoneNumber, Email, Address, City, pos.PositionId, pos.PositionName,
					proj.ProjectId, proj.ProjectName
					FROM deleted d
					JOIN Employees emp ON emp.EmployeeId = d.EmployeeId
					JOIN EmployeeContacts emCon ON emCon.EmployeeId = d.EmployeeId
					JOIN EmployeePersonalInfo emPin ON emPin.EmployeeId = d.EmployeeId
					JOIN Positions pos ON pos.PositionId = emp.PositionId
					JOIN Projects proj on proj.ProjectId = emp.ProjectId
					FOR XML PATH('Employee'))
			SET @xmlIns = (SELECT i.EmployeeId, Name, LastName, Photo, DateOfBirth,
					PhoneNumber, Email, Address, City, pos.PositionId, pos.PositionName,
					proj.ProjectId, proj.ProjectName
					FROM inserted i
					JOIN Employees emp ON emp.EmployeeId = i.EmployeeId
					JOIN EmployeeContacts emCon ON emCon.EmployeeId = i.EmployeeId
					JOIN EmployeePersonalInfo emPin ON emPin.EmployeeId = i.EmployeeId
					JOIN Positions pos ON pos.PositionId = emp.PositionId
					JOIN Projects proj on proj.ProjectId = emp.ProjectId
					FOR XML PATH('Employee'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, InsertedData, DeletedData)
			VALUES ('Employees, EmployeePersonalInfo, EmployeeContacts, EmployeeAddressInfo',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlIns,
			@xmlDel)
		END
END

GO


CREATE OR ALTER TRIGGER changesLogForPositions
ON Positions
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @operation VARCHAR(100)
		SET @operation = CASE
				WHEN EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
					THEN 'Update'
				WHEN EXISTS(SELECT * FROM inserted)
					THEN 'Insert'
				WHEN EXISTS(SELECT * FROM deleted)
					THEN 'Delete'
				ELSE NULL
		END

		DECLARE @xmlDel xml;
		DECLARE @xmlIns xml;

		IF @operation = 'Delete'
		BEGIN
			 SET @xmlDel = (SELECT d.PositionId, d.PositionName
				FROM deleted d
				FOR XML PATH('Position'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, DeletedData)
			VALUES ('Positions',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlDel)
		END

		IF @operation = 'Insert'
		BEGIN

		SET @xmlIns = (SELECT i.PositionId, i.PositionName
				FROM inserted i
				FOR XML PATH('Position'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, InsertedData)
			VALUES ('Positions',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlIns)
		END
 
		IF @operation = 'Update'
		BEGIN
			SET @xmlDel = (SELECT d.PositionId, d.PositionName
				FROM deleted d
				FOR XML PATH('Position'))
			SET @xmlIns = (SELECT i.PositionId, i.PositionName
				FROM inserted i
				FOR XML PATH('Position'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, InsertedData, DeletedData)
			VALUES ('Positions',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlIns,
			@xmlDel)
		END
END

GO

CREATE OR ALTER TRIGGER changesLogForProjects
ON Projects
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @operation VARCHAR(100)
		SET @operation = CASE
				WHEN EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
					THEN 'Update'
				WHEN EXISTS(SELECT * FROM inserted)
					THEN 'Insert'
				WHEN EXISTS(SELECT * FROM deleted)
					THEN 'Delete'
				ELSE NULL
		END

		DECLARE @xmlDel xml;
		DECLARE @xmlIns xml;

		IF @operation = 'Delete'
		BEGIN
			 SET @xmlDel = (SELECT d.ProjectId, d.ProjectName
				FROM deleted d
				FOR XML PATH('Project'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, DeletedData)
			VALUES ('Projects',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlDel)
		END

		IF @operation = 'Insert'
		BEGIN

		SET @xmlIns = (SELECT i.ProjectId, i.ProjectName
				FROM inserted i
				FOR XML PATH('Project'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, InsertedData)
			VALUES ('Projects',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlIns)
		END
 
		IF @operation = 'Update'
		BEGIN
			SET @xmlDel = (SELECT d.ProjectId, d.ProjectName
				FROM deleted d
				FOR XML PATH('Project'))
			SET @xmlIns = (SELECT i.ProjectId, i.ProjectName
				FROM inserted i
				FOR XML PATH('Project'))
			INSERT INTO ModificationsLog (TableName, Operation, OperationDate, UserName, InsertedData, DeletedData)
			VALUES ('Projects',
			@operation,
			GETDATE(),
			USER_NAME(),
			@xmlIns,
			@xmlDel)
		END
END

GO

CREATE OR ALTER TRIGGER validatePositions
ON Positions
INSTEAD OF INSERT
AS
BEGIN
	DECLARE @PositionNameLoc varchar(200);
	SELECT @PositionNameLoc = PositionName FROM inserted
	IF LEN(@PositionNameLoc) < 3
	BEGIN
		RAISERROR('PositionName is too short, at least 3 characters needed', 10, 1)
		ROLLBACK TRANSACTION
	END
	ELSE
	BEGIN
		INSERT INTO Positions(PositionName) VALUES (@PositionNameLoc)
	END
END


GO
CREATE OR ALTER TRIGGER validateProject
ON Projects
INSTEAD OF INSERT
AS
BEGIN
	DECLARE @ProjectNameLoc varchar(200);
	SELECT @ProjectNameLoc = ProjectName FROM inserted
	IF LEN(@ProjectNameLoc) < 3
	BEGIN
		RAISERROR('ProjectName is too short, at least 3 characters needed', 10, 1)
		ROLLBACK TRANSACTION
	END
	ELSE
	BEGIN
		INSERT INTO Projects(ProjectName) VALUES (@ProjectNameLoc)
	END
END
GO

