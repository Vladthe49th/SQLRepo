Use Accounts
GO

CREATE TABLE Accounts (
    AccountID INT PRIMARY KEY IDENTITY(10000, 1),
    AccountNumber NVARCHAR(20) UNIQUE NOT NULL,
    Balance DECIMAL(10, 2) NOT NULL CHECK (Balance >= 0)
);

INSERT INTO Accounts (AccountNumber, Balance) VALUES
('UA1234567890', 1000),
('UA0987654321', 500);


-- Transfer of funds with transaction
BEGIN TRANSACTION;

BEGIN TRY
    DECLARE @FromAccount NVARCHAR(20) = 'UA1234567890';
    DECLARE @ToAccount NVARCHAR(20) = 'UA0987654321';
    DECLARE @Amount DECIMAL(10,2) = 200;

    
    IF (SELECT Balance FROM Accounts WHERE AccountNumber = @FromAccount) < @Amount
        THROW 50000, 'Not enough cash for transfer...', 1;

    UPDATE Accounts
    SET Balance = Balance - @Amount
    WHERE AccountNumber = @FromAccount;


    UPDATE Accounts
    SET Balance = Balance + @Amount
    WHERE AccountNumber = @ToAccount;

    COMMIT TRANSACTION;
    PRINT 'Transaction complete!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Transaction incomplete :( ' + ERROR_MESSAGE();
END CATCH;