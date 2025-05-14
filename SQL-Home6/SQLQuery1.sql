Use Procedures
GO

CREATE TABLE [Employees] (
 [ID] INT PRIMARY KEY IDENTITY,
 [Name] NVARCHAR(50),
 [Position] NVARCHAR(50),
 [Department] NVARCHAR(50)
);
CREATE TABLE [EmployeeDetails] (
 [ID] INT PRIMARY KEY IDENTITY,
 [EmployeeID] INT FOREIGN KEY REFERENCES [Employees]([ID]),
 [Email] NVARCHAR(50),
 [PhoneNumber] NVARCHAR(15)
);

--1. Add data procedure
GO
CREATE PROCEDURE AddEmployee
    @Name NVARCHAR(50),
    @Position NVARCHAR(50),
    @Department NVARCHAR(50),
    @Email NVARCHAR(50),
    @PhoneNumber NVARCHAR(15)
AS
BEGIN
    DECLARE @NewEmployeeID INT;

    INSERT INTO Employees (Name, Position, Department)
    VALUES (@Name, @Position, @Department);

    SET @NewEmployeeID = SCOPE_IDENTITY();

    INSERT INTO EmployeeDetails (EmployeeID, Email, PhoneNumber)
    VALUES (@NewEmployeeID, @Email, @PhoneNumber);
END;


--2. Get data from both tables

GO
CREATE PROCEDURE GetAllEmployees
AS
BEGIN
    SELECT 
        e.ID,
        e.Name,
        e.Position,
        e.Department,
        d.Email,
        d.PhoneNumber
    FROM Employees e
    LEFT JOIN EmployeeDetails d ON e.ID = d.EmployeeID;
END;


--3. Data editing
GO
CREATE PROCEDURE UpdateEmployee
    @ID INT,
    @Name NVARCHAR(50),
    @Position NVARCHAR(50),
    @Department NVARCHAR(50),
    @Email NVARCHAR(50),
    @PhoneNumber NVARCHAR(15)
AS
BEGIN
    UPDATE Employees
    SET Name = @Name, Position = @Position, Department = @Department
    WHERE ID = @ID;

    UPDATE EmployeeDetails
    SET Email = @Email, PhoneNumber = @PhoneNumber
    WHERE EmployeeID = @ID;
END;

--4. ID deleteion
GO
CREATE PROCEDURE DeleteByID
    @ID INT
AS
BEGIN
    DELETE FROM EmployeeDetails WHERE EmployeeID = @ID;
    DELETE FROM Employees WHERE ID = @ID;
END;

--5. Data exchange with unnecessary parameters

GO
CREATE PROCEDURE SearchEmployees
    @Name NVARCHAR(50) = NULL,
    @Department NVARCHAR(50) = NULL
AS
BEGIN
    SELECT 
        e.ID,
        e.Name,
        e.Position,
        e.Department,
        d.Email,
        d.PhoneNumber
    FROM Employees e
    LEFT JOIN EmployeeDetails d ON e.ID = d.EmployeeID
    WHERE (@Name IS NULL OR e.Name LIKE '%' + @Name + '%')
      AND (@Department IS NULL OR e.Department = @Department);
END;

--6. Two parameters

GO
CREATE PROCEDURE GetParams
    @ID INT,
    @Name NVARCHAR(50) OUTPUT,
    @Position NVARCHAR(50) OUTPUT
AS
BEGIN
    SELECT 
        @Name = Name,
        @Position = Position
    FROM Employees
    WHERE ID = @ID;
END;