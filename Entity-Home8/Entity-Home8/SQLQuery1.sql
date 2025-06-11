-- Get users with companies

CREATE PROCEDURE GetUsersWithCompanies
AS
BEGIN
    SELECT u.Id, u.Email, u.Age, u.Salary, c.Name AS CompanyName
    FROM Users u
    JOIN Companies c ON u.CompanyId = c.Id
END


-- Get by first name (For Tom)

CREATE PROCEDURE GetUsersByFirstName
    @Name NVARCHAR(50)
AS
BEGIN
    SELECT u.Id, us.FirstName, us.LastName, u.Email
    FROM Users u
    JOIN UserSettings us ON u.Id = us.UserId
    WHERE us.FirstName LIKE @Name + '%'
END


-- Average user age

CREATE PROCEDURE GetAverageAge
    @AvgAge FLOAT OUTPUT
AS
BEGIN
    SELECT @AvgAge = AVG(CAST(Age AS FLOAT)) FROM Users
END

