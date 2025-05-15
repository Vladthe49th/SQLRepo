create database exam
GO


CREATE TABLE Categories (
    Id INT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);


CREATE TABLE Products (
    Id INT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Brand VARCHAR(100),
    Price DECIMAL(10,2) CHECK (Price > 0),
    Description TEXT,
    CategoryId INT,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);


CREATE TABLE Customers (
    Id INT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Address VARCHAR(200),
    Email VARCHAR(100) UNIQUE
);


CREATE TABLE Employees (
    Id INT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Position VARCHAR(50) NOT NULL DEFAULT 'Salesperson'
);


CREATE TABLE Orders (
    Id INT PRIMARY KEY,
    CustomerID INT,
    EmployeeID INT,
    Date DATE NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) CHECK (Status IN ('Pending', 'Shipped', 'Cancelled')),
    TotalAmount DECIMAL(10,2) DEFAULT 0 CHECK (TotalAmount >= 0),
    FOREIGN KEY (CustomerID) REFERENCES Customers(Id),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(Id)
);

CREATE TABLE OrderDetails (
    OrderId INT,
    ProductId INT,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    PRIMARY KEY (OrderId, ProductId),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);


CREATE TABLE Reviews (
    Id INT PRIMARY KEY,
    ProductID INT,
    CustomerID INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    FOREIGN KEY (ProductID) REFERENCES Products(Id),
    FOREIGN KEY (CustomerID) REFERENCES Customers(Id)
);


INSERT INTO Categories (Id, Name) VALUES
(1, 'Electronics'),
(2, 'Books'),
(3, 'Clothing');

INSERT INTO Products (Id, Name, Brand, Price, Description, CategoryId) VALUES
(1, 'Smartphone', 'Samsung', 499.99, 'Android smartphone with AMOLED display', 1),
(2, 'Laptop', 'Lenovo', 899.99, 'Lightweight laptop for office work', 1),
(3, 'Novel - "Metro 2033"', 'Dmitry Glukhovsky', 14.99, 'Post-apocalyptic novel', 2),
(4, 'T-Shirt', 'Nike', 24.99, 'Cotton sportswear', 3);


INSERT INTO Customers (Id, FirstName, LastName, Address, Email) VALUES
(1, 'Ivan', 'Obama', 'Kyiv, Shevchenka St. 12', 'ivanobama@pipidastr.com'),
(2, 'Grobocop', 'Ivanovich', 'Odessa, Graveyard St. 8', 'grobocop@gmail.com');

INSERT INTO Employees (Id, FirstName, LastName, Position) VALUES
(1, 'Killian', 'Ventura', 'Sales Manager'),
(2, 'Ildar', 'Ildar', 'Support Specialist');

INSERT INTO Orders (Id, CustomerID, EmployeeID, Date, Status, TotalAmount) VALUES
(1, 1, 1, '2025-05-10', 'Shipped', 524.98),
(2, 2, 2, '2025-05-12', 'Pending', 914.98);

INSERT INTO OrderDetails (OrderId, ProductId, Quantity) VALUES
(1, 1, 1), 
(1, 4, 1), 
(2, 2, 1), 
(2, 3, 1); 

INSERT INTO Reviews (Id, ProductID, CustomerID, Rating, Comment) VALUES
(1, 1, 1, 5, 'Great smartphone, can capture me climbing oaks in 4k!'),
(2, 2, 2, 4, 'Good performance, but battery life could be 200%.'),
(3, 3, 1, 5, 'Loved the atmosphere in this book. Reminds me of my hometown...'),
(4, 4, 2, 3, 'Comfortable, but too expensive... My wallet is sent to reanimation, but I guess it`s worth it!');


--1. Products in a single category

SELECT p.Name, p.Brand, p.Price
FROM Products p
JOIN Categories c ON p.CategoryId = c.Id
WHERE c.Name = 'Electronics';


--2. Average price in each category

SELECT c.Name AS Category, AVG(p.Price) AS AvgPrice
FROM Products p
JOIN Categories c ON p.CategoryId = c.Id
GROUP BY c.Name;


--3. Orders of a specific customer

SELECT o.Id AS OrderID, o.Date, o.Status, o.TotalAmount
FROM Orders o
JOIN Customers c ON o.CustomerID = c.Id
WHERE c.FirstName = 'Ivan';


--4. Clients with orders higher than specific price (500)

SELECT DISTINCT c.FirstName, c.LastName, SUM(o.TotalAmount) AS TotalSpent
FROM Customers c
JOIN Orders o ON c.Id = o.CustomerID
GROUP BY c.Id, c.FirstName, c.LastName
HAVING SUM(o.TotalAmount) > 500;


--5. Clients with orders cheaper than specific price (100)

SELECT DISTINCT c.Name
FROM Products p
JOIN Categories c ON p.CategoryId = c.Id
WHERE p.Price < 100;


--6. Number of products in each category

SELECT c.Name AS Category, COUNT(p.Id) AS ProductCount
FROM Categories c
LEFT JOIN Products p ON c.Id = p.CategoryId
GROUP BY c.Name;

--7. Client with the most reviews

SELECT TOP 1 c.FirstName, c.LastName, COUNT(r.Id) AS ReviewCount
FROM Customers c
JOIN Reviews r ON c.Id = r.CustomerID
GROUP BY c.Id, c.FirstName, c.LastName
ORDER BY ReviewCount DESC


--8. Sum of orders for a each client

SELECT c.FirstName, c.LastName, SUM(o.TotalAmount) AS TotalSpent
FROM Customers c
JOIN Orders o ON c.Id = o.CustomerID
GROUP BY c.FirstName, c.LastName;


--9. Products without reviews

SELECT p.Name, p.Brand
FROM Products p
LEFT JOIN Reviews r ON p.Id = r.ProductID
WHERE r.Id IS NULL;



-- 10. Loyalty level 

ALTER TABLE Customers
ADD LoyaltyLevel VARCHAR(20);

UPDATE Customers
SET LoyaltyLevel = CASE 
    WHEN Total.TotalAmount > 2000 THEN 'Gold'
    WHEN Total.TotalAmount > 1000 THEN 'Silver'
    ELSE 'Bronze'
END
FROM Customers c
JOIN (
    SELECT CustomerID, SUM(TotalAmount) AS TotalAmount
    FROM Orders
    GROUP BY CustomerID
) AS Total ON c.Id = Total.CustomerID;

--11. Order status update procedure

GO
CREATE PROCEDURE UpdateOrderStatus
AS
BEGIN
    UPDATE Orders
    SET Status = CASE
        WHEN DATEDIFF(DAY, Date, GETDATE()) < 7 THEN 'New'
        WHEN DATEDIFF(DAY, Date, GETDATE()) < 30 THEN 'In Progress'
        WHEN DATEDIFF(DAY, Date, GETDATE()) < 365 THEN 'Shipped'
        ELSE 'Completed'
    END;
END;

EXEC UpdateOrderStatus;

--12. Average rating function
GO
CREATE FUNCTION CalculateAverageRating (@ProductId INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @Average FLOAT;

    SELECT @Average = AVG(CAST(Rating AS FLOAT))
    FROM Reviews
    WHERE ProductID = @ProductId;

    RETURN @Average;
END;