USE master;
GO
 

 --1. Hello procedure

 CREATE PROCEDURE Hello
AS
BEGIN
    PRINT 'Hello, world!';
END;

--2. Current time

CREATE PROCEDURE GetCurrentTime
AS
BEGIN
    SELECT CONVERT(TIME, GETDATE()) AS CurrentTime;
END;

--3. Current data

CREATE PROCEDURE GetCurrentDate
AS
BEGIN
    SELECT CONVERT(DATE, GETDATE()) AS CurrentDate;
END;

--4. Sum of three numbers

CREATE PROCEDURE ThreeSum
    @a INT, @b INT, @c INT
AS
BEGIN
    SELECT @a + @b + @c AS Sum;
END;

--5. Three average

CREATE PROCEDURE ThreeAvg
    @a INT, @b INT, @c INT
AS
BEGIN
    SELECT (@a + @b + @c) / 3 AS Average;
END;

--6. Max of three

CREATE PROCEDURE MaxOfThree
    @a INT, @b INT, @c INT
AS
BEGIN
    SELECT 
        CASE 
            WHEN @a >= @b AND @a >= @c THEN @a
            WHEN @b >= @a AND @b >= @c THEN @b
            ELSE @c
        END AS MaxValue;
END;

--7. Min of three

CREATE PROCEDURE MinOfThree
    @a INT, @b INT, @c INT
AS
BEGIN
    SELECT 
        CASE 
            WHEN @a <= @b AND @a <= @c THEN @a
            WHEN @b <= @a AND @b <= @c THEN @b
            ELSE @c
        END AS MinValue;
END;

--8. Line of symbols

CREATE PROCEDURE Line
    @length INT, @symbol CHAR(1)
AS
BEGIN
    DECLARE @result NVARCHAR(MAX) = '';
    DECLARE @i INT = 1;

    WHILE @i <= @length
    BEGIN
        SET @result = @result + @symbol;
        SET @i = @i + 1;
    END

    SELECT @result AS Line;
END;


--9. Power of number

CREATE PROCEDURE NumPower
    @base FLOAT, @exponent INT
AS
BEGIN
    SELECT POWER(@base, @exponent) AS Result;
END;

--10. Factorial

CREATE PROCEDURE Factorial
    @number INT
AS
BEGIN
    DECLARE @result BIGINT = 1;
    DECLARE @i INT = 1;

    WHILE @i <= @number
    BEGIN
        SET @result = @result * @i;
        SET @i = @i + 1;
    END

    SELECT @result AS FactorialResult;
END;