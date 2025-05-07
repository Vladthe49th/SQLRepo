Use Furniture
GO

CREATE TABLE [Product] (

[ProductID] INT PRIMARY KEY,
[Name] VARCHAR(50),
[Price] MONEY

);

CREATE TABLE [Customer] (

[CustomerID] INT PRIMARY KEY,
[FullName] VARCHAR(100),
[Address] VARCHAR(50),
[Email] VARCHAR(50)

);

CREATE TABLE [OrderTable] (
    [OrderID] INT PRIMARY KEY,
    [CustomerID] INT,
    [ProductID] INT,
    [DeliveryID] INT,
    FOREIGN KEY ([CustomerID]) REFERENCES [Customer]([CustomerID]),
    FOREIGN KEY ([ProductID]) REFERENCES [Product]([ProductID]),
    FOREIGN KEY ([DeliveryID]) REFERENCES [Delivery]([DeliveryID])
);

CREATE TABLE [Delivery] (
    [DeliveryID] INT PRIMARY KEY,
    [Type] VARCHAR(50), 
    [Price] DECIMAL(10, 2)
);



INSERT INTO [Product] VALUES
(1, 'Wooden chair', 1200),
(2, 'Dinner table', 4500),
(3, 'Corner sofa', 15000),
(4, 'Bedside table', 1200),
(5, 'Office chair', 3200);


INSERT INTO [Customer] VALUES
(1, 'Kratos', 'Midgard st. 10', 'Godofwar@norsemail.com'),
(2, 'Petrov Vasechkin', 'Derevyannaya st. 5', 'petrov@gmail.com'),
(3, 'Justin Bieber', 'Peace st. 20', 'Biba@gmail.com');

INSERT INTO [Delivery] VALUES
(1, 'Delivery man', 300),
(2, 'Self pickup', 0),
(3, 'КУРЬЕР', 500);

INSERT INTO [OrderTable] VALUES
(1, 1, 1, 1),
(2, 1, 2, 1),
(3, 2, 3, 2),
(4, 3, 4, 3),
(5, 2, 5, 1);


--1. Min price
SELECT MIN(Price) AS MinPrice FROM Product;

--2. Max price
SELECT MAX(Price) AS MaxPrice FROM Product;

--3. Similar prices
SELECT Price, COUNT(*) 
FROM Product
GROUP BY Price
HAVING COUNT(*) > 1;

--4. Two customers with most purchases

SELECT TOP 2 C.FullName, SUM(P.Price + D.Price) AS TotalSpent
FROM OrderTable O
JOIN Customer C ON O.CustomerID = C.CustomerID
JOIN Product P ON O.ProductID = P.ProductID
JOIN Delivery D ON O.DeliveryID = D.DeliveryID
GROUP BY C.FullName
ORDER BY TotalSpent DESC;



--5. Two customers with least purchases

SELECT TOP 2 C.FullName, SUM(P.Price + D.Price) AS TotalSpent
FROM OrderTable O
JOIN Customer C ON O.CustomerID = C.CustomerID
JOIN Product P ON O.ProductID = P.ProductID
JOIN Delivery D ON O.DeliveryID = D.DeliveryID
GROUP BY C.FullName
ORDER BY TotalSpent ASC;

--6. Self pickup customers

SELECT DISTINCT C.FullName
FROM OrderTable O
JOIN Customer C ON O.CustomerID = C.CustomerID
JOIN Delivery D ON O.DeliveryID = D.DeliveryID
WHERE D.Type = 'Self pickup';

-- 7. Total sum for every customer

SELECT C.FullName, SUM(P.Price + D.Price) AS TotalSpent
FROM OrderTable O
JOIN Customer C ON O.CustomerID = C.CustomerID
JOIN Product P ON O.ProductID = P.ProductID
JOIN Delivery D ON O.DeliveryID = D.DeliveryID
GROUP BY C.FullName;

--8. Total sum for a certain customer

SELECT SUM(P.Price + D.Price) AS TotalSpent
FROM OrderTable O
JOIN Product P ON O.ProductID = P.ProductID
JOIN Delivery D ON O.DeliveryID = D.DeliveryID
WHERE O.CustomerID = 1;  -- For example

--9. Info about customer and their purchases

SELECT C.FullName, C.Address, C.Email, P.Name AS ProductName
FROM OrderTable O
JOIN Customer C ON O.CustomerID = C.CustomerID
JOIN Product P ON O.ProductID = P.ProductID;


--10. Total products with total price

SELECT COUNT(*) AS TotalProducts, SUM(Price) AS TotalPrice
FROM Product;

