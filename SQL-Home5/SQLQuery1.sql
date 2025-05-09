Use Employees
GO


-- Main task --
CREATE TABLE [Departments] (
 [Id] INT PRIMARY KEY IDENTITY,
 [Name] NVARCHAR(50)
);
CREATE TABLE Employees (
 [Id] INT PRIMARY KEY IDENTITY,
 [FirstName] NVARCHAR(50),
 [LastName] NVARCHAR(50),
 [HireDate] DATE,
 [DepartmentID] INT,
 FOREIGN KEY ([DepartmentID]) REFERENCES [Departments]([Id])
);
CREATE TABLE [Orders] (
 [Id] INT PRIMARY KEY IDENTITY,
 [EmployeeID] INT,
 [OrderDate] DATE,
 [Amount] DECIMAL(10, 2),
 FOREIGN KEY (EmployeeID) REFERENCES Employees([Id])
);

INSERT INTO Departments ([Name]) VALUES 
('Sales'),
('IT'),
('HR');

INSERT INTO Employees (FirstName, LastName, HireDate, DepartmentID) VALUES
('Dr.', 'Robotnik', '2015-03-15', 1),
('Jabal', 'Shamalan', '2018-07-10', 2),
('Biba', 'Boba', '2020-01-20', 1),
('Sasha', 'Gray', '2022-11-01', 3),
('Alexander', 'Petrovsky', '2017-05-25', 2);

INSERT INTO Orders (EmployeeID, OrderDate, Amount) VALUES
(1, '2024-01-10', 250.00),
(1, '2024-03-15', 150.75),
(2, '2024-02-20', 300.00),
(3, '2024-04-01', 450.50),
(4, '2024-03-05', 100.00),
(5, '2025-02-28', 380.20),
(2, '2025-04-20', 200.00),
(1, '2025-03-15', 120.00),
(5, '2025-05-01', 75.00);


--1. UPPER names

SELECT UPPER(FirstName) AS FirstName, UPPER(LastName) AS LastName
FROM Employees;

--2. Employee age with other columns
SELECT *, DATEDIFF(YEAR, HireDate, GETDATE()) AS Age
FROM Employees;

--3. Orders after a certain date

SELECT *
FROM Orders
WHERE OrderDate > '2024-01-01';

--4. Sum of orders for each employee

SELECT EmployeeID, SUM(Amount) AS TotalAmount
FROM Orders
GROUP BY EmployeeID;

--5. Employee count in each dept.

SELECT DepartmentID, COUNT(*) AS EmployeeCount
FROM Employees
GROUP BY DepartmentID;


--6. Averagy days worked by employees

SELECT AVG(DATEDIFF(DAY, HireDate, GETDATE())) AS AvgDaysWorked
FROM Employees;


--7. Name length of each employee

SELECT FirstName, LastName, LEN(FirstName) AS FirstNameLength, LEN(LastName) AS LastNameLength
FROM Employees;

--8. Sum of orders with $ sign

SELECT Id, FORMAT(Amount, 'C', 'en-US') AS FormattedAmount
FROM Orders;


--9. First 3 letters of employee names

SELECT SUBSTRING(FirstName, 1, 3) AS First3Letters
FROM Employees;

--10. Digits amount after '.' in order

SELECT Id, Amount,
       LEN(CAST(Amount AS VARCHAR)) - CHARINDEX('.', CAST(Amount AS VARCHAR)) AS DigitsAmount
FROM Orders
WHERE Amount LIKE '%.%';

--11. Employees with the longest name

SELECT *
FROM Employees
WHERE LEN(FirstName) = (
    SELECT MAX(LEN(FirstName)) FROM Employees
);

--12. Average days worked in each department

SELECT DepartmentID, AVG(DATEDIFF(DAY, HireDate, GETDATE())) AS AvgDaysWorked
FROM Employees
GROUP BY DepartmentID;

--13. Sum of orders for each employee in last 90 days

SELECT EmployeeID, SUM(Amount) AS TotalLast90Days
FROM Orders
WHERE OrderDate >= DATEADD(DAY, -90, GETDATE())
GROUP BY EmployeeID;

--14. Top-3 employees with highest order sums

SELECT TOP 3 E.Id, E.FirstName, E.LastName, SUM(O.Amount) AS TotalAmount
FROM Employees E
JOIN Orders O ON E.Id = O.EmployeeID
GROUP BY E.Id, E.FirstName, E.LastName
ORDER BY TotalAmount DESC;

--15. Column with name and sum of orders

SELECT E.Id, E.FirstName, E.LastName,
       CONCAT(E.FirstName, ' ', E.LastName, ' - $', SUM(O.Amount)) AS NameAndTotal
FROM Employees E
LEFT JOIN Orders O ON E.Id = O.EmployeeID
GROUP BY E.Id, E.FirstName, E.LastName;




-- Sub-tasks --


--1. Check if variable is null

IF @variable IS NULL OR @variable = ''
    PRINT 'Variable is either empty of NULL';
ELSE
    PRINT 'Variable holds a value';

--2. Products table + price category

SELECT 
    ProductName,
    Price,
    CASE 
        WHEN Price < 100 THEN 'Cheap'
        WHEN Price BETWEEN 100 AND 500 THEN 'Middle price'
        ELSE 'Expensive'
    END AS Category
FROM Products;


--3. Boxer weight category

DECLARE @weight INT = 63; 

SELECT 
    CASE 
        WHEN @weight < 60 THEN 'Light weight'
        WHEN @weight < 64 THEN 'First half-middle weight'
        WHEN @weight <= 69 THEN 'Half-middle weight'
    END AS Category;


--4. Comparison of num and it`s product of digits

DECLARE @num INT = 42;
DECLARE @digit1 INT = @num / 10;
DECLARE @digit2 INT = @num % 10;

IF @num > @digit1 * @digit2
    PRINT 'Number is bigger than it`s digit product!';
ELSE IF @num < @digit1 * @digit2
    PRINT 'Digit product is bigger!';
ELSE
    PRINT 'Number and digit product are equal!';


--5. Sum of flat numbers

DECLARE @start INT = 10, @end INT = 86;
DECLARE @count INT = @end - @start + 1;
DECLARE @sum INT = (@start + @end) * @count / 2;

IF @sum % 2 = 0
    PRINT 'Sum of nums is even';
ELSE
    PRINT 'The sum is odd';

--6. Generation of two borders and three nums in between

-- Border generation
DECLARE @a INT = FLOOR(RAND() * 90 + 10);
DECLARE @b INT = FLOOR(RAND() * 90 + 10);
DECLARE @min INT = IIF(@a < @b, @a, @b);
DECLARE @max INT = IIF(@a > @b, @a, @b);

-- Random nums generation
SELECT FLOOR(RAND(CHECKSUM(NEWID())) * (@max - @min + 1)) + @min AS Rand1,
       FLOOR(RAND(CHECKSUM(NEWID()) + 1) * (@max - @min + 1)) + @min AS Rand2,
       FLOOR(RAND(CHECKSUM(NEWID()) + 2) * (@max - @min + 1)) + @min AS Rand3;

--7. Payment replacement

UPDATE Employees
SET Salary = CASE 
    WHEN Salary = 900 THEN 1000
    ELSE 1500
END;

--8. IF ELSE in SELECT - who is under and over paid?

SELECT 
    EmployeeName,
    Salary,
    CASE 
        WHEN Salary <= 2000 THEN 'UNDERPAID'
        WHEN Salary >= 4000 THEN 'OVERPAID'
        ELSE 'OK'
    END AS PayStatus
FROM Employees;