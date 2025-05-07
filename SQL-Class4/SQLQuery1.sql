create database BCN
use BCN
 
create table Department
(
Id int IDENTITY PRIMARY KEY,
department_name varchar(60) NOT NULL
)
 
create table Workers
(
Id int IDENTITY PRIMARY KEY,
DepartmentId int REFERENCES Department(Id),
Name varchar(60) NOT NULL,
HobbyId int,
Salary money NOT NULL
)
 
create table Hobby
(
Id int IDENTITY PRIMARY KEY,
Name varchar(60)
)
 
insert into Department values
('IT'),
('Marketing'),
('Finance')
 
insert into Hobby values
('Fishing'),
('Games'),
('Sport'),
('Movie'),
('Walk')
 
insert into Workers values
(1, 'Alex',1, 7000),
(1, 'Tom', NULL, 9000),
(3, 'Marry',2, 6500),
(2, 'Alex',3,12000),
(1, 'Smith',NULL,15000),
(3, 'John',2,14500)



--1. Workers and departments
SELECT 
    Workers.[Name] AS WorkerName,
    Department.department_name AS DepartmentName,
    Workers.Salary
FROM Workers
JOIN Department ON Workers.DepartmentId = Department.Id;

--2. Departments with no workers

SELECT 
    Department.department_name
FROM Department
LEFT JOIN Workers ON Department.Id = Workers.DepartmentId
WHERE Workers.Id IS NULL;

--3. Workers with > 10000 salary, + department name

SELECT 
    Workers.[Name] AS WorkerName,
    Department.department_name AS DepartmentName,
    Workers.Salary
FROM Workers
JOIN Department ON Workers.DepartmentId = Department.Id
WHERE Workers.Salary > 10000;

-- 4. With hobbies and no hobbies

-- Workers with hobby

SELECT 
    Workers.[Name] AS WorkerName,
    Hobby.Name AS HobbyName
FROM Workers
JOIN Hobby ON Workers.HobbyId = Hobby.Id

UNION

-- Without hobby 

SELECT 
    Workers.[Name] AS WorkerName,
    NULL AS HobbyName
FROM Workers
WHERE Workers.HobbyId IS NULL;

--5. Workers with salary < 10000 and games hobby

SELECT 
    Workers.[Name] AS WorkerName,
    Hobby.[Name] AS HobbyName,
    Workers.Salary
FROM Workers
JOIN Hobby ON Workers.HobbyId = Hobby.Id
WHERE Workers.Salary < 10000 AND Hobby.Name = 'Games';

