-- Customers
CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL
);

-- Ratings

CREATE TABLE Ratings (
    Id INT PRIMARY KEY IDENTITY,
    CustomerId INT NOT NULL,
    BookId INT NOT NULL,
    Score INT CHECK (Score BETWEEN 1 AND 5),
    CONSTRAINT FK_Ratings_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_Ratings_Books FOREIGN KEY (BookId) REFERENCES Books(Id)
);


-- Categories

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

-- Products

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Stock INT NOT NULL,
    Description NVARCHAR(MAX),
    CategoryId INT,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Orders
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2) NOT NULL
);


-- Order details

CREATE TABLE OrderDetails (
    Id INT PRIMARY KEY IDENTITY,
    OrderId INT,
    ProductId INT,
    Quantity INT,
    Price DECIMAL(10, 2),
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);


-- Reviews
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY,
    ProductId INT,
    CustomerName NVARCHAR(100),
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

