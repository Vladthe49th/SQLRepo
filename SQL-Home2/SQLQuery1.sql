use Doctors


--Tables

CREATE TABLE [Specializations] (

[Id] INT PRIMARY KEY,
[Name] VARCHAR(50)

);

CREATE TABLE [Doctors] (

[Id] INT PRIMARY KEY,
[Name] VARCHAR(50),
[Premium] MONEY,
[Salary] MONEY,
[Surname] VARCHAR(50)

);

CREATE TABLE [DoctorsSpecializations] (

[Id] INT PRIMARY KEY,
[DoctorId] INT,
FOREIGN KEY ([DoctorId]) REFERENCES [Doctors]([Id]),
[SpecializationId] INT,
FOREIGN KEY ([SpecializationId]) REFERENCES [Specializations]([Id])

);

CREATE TABLE [Vacations] (

[Id] INT PRIMARY KEY,
[EndDate] DATE,
[StartDate] DATE,
[DoctorId] INT,
FOREIGN KEY ([DoctorId]) REFERENCES [Doctors]([Id]),

);


CREATE TABLE [Departments] (

[Id] INT PRIMARY KEY,
[Name] VARCHAR(50)

);

CREATE TABLE [Wards] (

[Id] INT PRIMARY KEY,
[Name] VARCHAR(50),
[DepartmentId] INT,
FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])

);

CREATE TABLE [Sponsors] (

[Id] INT PRIMARY KEY,
[Name] VARCHAR(50)

);

CREATE TABLE [Donations] (

[Id] INT PRIMARY KEY,
[Amount] MONEY,
[Date] DATE,
[DepartmentId] INT,
FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id]),
[SponsorId] INT,
FOREIGN KEY ([SponsorId]) REFERENCES [Sponsors]([Id])


);

CREATE TABLE [Diseases] (
    [Id] INT PRIMARY KEY,
    [Name] VARCHAR(100),
    [IsContagious] BIT,           
    [SeverityLevel] INT           
);

CREATE TABLE [Examinations] (
    [Id] INT PRIMARY KEY,
    [DoctorId] INT,
    FOREIGN KEY ([DoctorId]) REFERENCES [Doctors]([Id]),
    [DiseaseId] INT,
    FOREIGN KEY ([DiseaseId]) REFERENCES [Diseases]([Id]),
    [WardId] INT,
    FOREIGN KEY ([WardId]) REFERENCES [Wards]([Id]),
    [Date] DATE,
    [IsWeekend] BIT,           

   
    
    
);



-- Specializations
INSERT INTO [Specializations] VALUES 
(1, 'Cardiology'),
(2, 'Neurology'),
(3, 'Infectious Diseases'),
(4, 'Surgery');

-- Doctors
INSERT INTO [Doctors] VALUES 
(1, 'Lara', 5000, 15000, 'Croft'),
(2, 'Will', 0, 12000, 'Smith'),
(3, 'Emily', 3000, 13000, 'Clark'),
(4, 'Kolan', 2000, 11000, 'Korichnevyi');

-- Doctors specializations
INSERT INTO [DoctorsSpecializations] VALUES 
(1, 1, 1),
(2, 2, 2),
(3, 3, 3),
(4, 4, 4);

-- Vacations
INSERT INTO [Vacations] VALUES 
(1, '2025-05-10', '2025-04-25', 3), 
(2, '2025-05-05', '2025-04-20', 1);  

-- Departments
INSERT INTO [Departments] VALUES 
(1, 'Cardiology Unit'),
(2, 'Intensive Treatment'),
(3, 'Infectious Unit'),
(4, 'Neurology Wing');

-- Wards
INSERT INTO [Wards] VALUES 
(1, 'Ward A', 1),
(2, 'Ward B', 2),
(3, 'Ward C', 3),
(4, 'Ward D', 4);

-- Sponsors
INSERT INTO [Sponsors] VALUES 
(1, 'Umbrella Corporation'),
(2, 'Stark Industries'),
(3, 'Wayne Enterprises');

-- Donations
INSERT INTO [Donations] VALUES 
(1, 150000, '2025-04-01', 2, 1), -- Umbrella - Intensive Treatment
(2, 80000, '2025-04-15', 1, 2),
(3, 200000, '2025-04-25', 3, 1); -- Umbrella - Infectious Unit

-- Diseases
INSERT INTO [Diseases] VALUES 
(1, 'Heart Failure', 0, 4),
(2, 'Migraine', 0, 2),
(3, 'COVID-25', 1, 5),
(4, 'T-Virus', 0, 3);

-- Examinations
INSERT INTO [Examinations] VALUES 
(1, 1, 1, 1, '2025-04-20', 0),
(2, 2, 2, 4, '2025-04-19', 0),
(3, 3, 3, 2, '2025-04-28', 0), -- Infections
(4, 1, 3, 2, '2025-04-22', 1), -- Holiday
(5, 4, 4, 3, '2025-04-25', 0);


-- 1 Names and specializations
SELECT 
    D.[Name] + ' ' + D.[Surname] AS [FullName],
    S.[Name] AS [Specialization]
FROM [Doctors] D
JOIN [DoctorsSpecializations] DS ON D.Id = DS.DoctorId
JOIN [Specializations] S ON DS.[SpecializationId] = S.[Id]


--2 Surnames and salaries

SELECT 
    D.[Surname],
    D.[Salary] + D.[Premium] AS [TotalSalary]
FROM [Doctors] D
WHERE D.[Id] NOT IN (
    SELECT [DoctorId]
    FROM [Vacations]
    WHERE GETDATE() BETWEEN [StartDate] AND [EndDate]
);


--3 Ward names

SELECT W.[Name] AS [WardName]
FROM [Wards] W
JOIN [Departments] [Dept] ON W.[DepartmentId] = Dept.[Id]
WHERE Dept.[Name] = 'Intensive Treatment';


--4 Depts sponsored by Umbrella

SELECT DISTINCT Dept.[Name] AS [DepartmentName]
FROM [Donations] D
JOIN [Sponsors] S ON D.[SponsorId] = S.[Id]
JOIN [Departments] Dept ON D.[DepartmentId] = Dept.[Id]
WHERE S.[Name] = 'Umbrella Corporation';


--5 Last month donations

SELECT 
    Dept.[Name] AS [Department],
    S.[Name] AS [Sponsor],
    D.[Amount],
    D.[Date]
FROM [Donations] D
JOIN [Departments] Dept ON D.[DepartmentId] = Dept.[Id]
JOIN [Sponsors] S ON D.[SponsorId] = S.[Id]
WHERE D.[Date] >= DATEADD(MONTH, -1, GETDATE());

--6. Weekday surnames

SELECT 
    D.[Surname],
    Dept.[Name] AS [Department]
FROM [Examinations] E
JOIN [Doctors] D ON E.[DoctorId] = D.[Id]
JOIN [Wards] W ON E.[WardId] = W.[Id]
JOIN [Departments] Dept ON W.[DepartmentId] = Dept.[Id]
WHERE E.[IsWeekend] = 0
GROUP BY D.[Surname], Dept.[Name];

--7  Will Smith wards/depts

SELECT 
    W.[Name] AS [Ward],
    Dept.[Name] AS [Department]
FROM [Examinations] E
JOIN [Doctors] D ON E.[DoctorId] = D.[Id]
JOIN [Wards] W ON E.[WardId] = W.[Id]
JOIN [Departments] Dept ON W.[DepartmentId] = Dept.[Id]
WHERE D.[Name] = 'Will' AND D.[Surname] = 'Smith';

--8 Donations > 100000 and dept. doctors

SELECT 
    Dept.[Name] AS [Department],
    D.[Name] + ' ' + D.[Surname] AS [DoctorName]
FROM [Donations] Don
JOIN [Departments] Dept ON Don.[DepartmentId] = Dept.[Id]
JOIN [Wards] W ON W.[DepartmentId] = Dept.[Id]
JOIN [Examinations] E ON E.[WardId] = W.[Id]
JOIN [Doctors] D ON E.[DoctorId] = D.[Id]
WHERE Don.[Amount] > 100000;

--9 Non-premium departments

SELECT DISTINCT Dept.[Name] AS [Department]
FROM [Doctors] D
JOIN [Examinations] E ON D.[Id] = E.[DoctorId]
JOIN [Wards] W ON E.[WardId] = W.[Id]
JOIN [Departments] Dept ON W.[DepartmentId] = Dept.[Id]
WHERE D.[Premium] = 0;


--10 Severity > 30 level 

SELECT DISTINCT S.[Name] AS [Specialization]
FROM [Examinations] E
JOIN [Diseases] Di ON E.[DiseaseId] = Di.[Id]
JOIN [DoctorsSpecializations] DS ON E.[DoctorId] = DS.[DoctorId]
JOIN [Specializations] S ON DS.[SpecializationId] = S.[Id]
WHERE Di.[SeverityLevel] > 3;


--11 Half-year examination depts.

SELECT DISTINCT Dept.[Name] AS [Department], Di.[Name] AS [Disease]
FROM [Examinations] E
JOIN [Diseases] Di ON E.[DiseaseId] = Di.[Id]
JOIN [Wards] W ON E.[WardId] = W.[Id]
JOIN [Departments] Dept ON W.[DepartmentId] = Dept.[Id]
WHERE E.[Date] >= DATEADD(MONTH, -6, GETDATE());

--12 Contagious examination wards

SELECT DISTINCT Dept.[Name] AS [Department], W.[Name] AS [Ward]
FROM [Examinations] E
JOIN [Diseases] Di ON E.[DiseaseId] = Di.[Id]
JOIN [Wards] W ON E.[WardId] = W.[Id]
JOIN [Departments] Dept ON W.[DepartmentId] = Dept.[Id]
WHERE Di.[IsContagious] = 1;