USE DataBase3


--  Departments
CREATE TABLE [Departments] (
    [DepartmentId] INT PRIMARY KEY,              
    [DepartmentName] VARCHAR(100) NOT NULL,        
    [DepartmentHead] VARCHAR(100),                 
    [DepartmentLocation] VARCHAR(100)              
);

--  Employees
CREATE TABLE [Employees] (
    [EmployeeId] INT PRIMARY KEY,                 
    [EmployeeName] VARCHAR(100) NOT NULL,       
    [BirthDate] DATE,                               
    [Position] VARCHAR(100),                        
    [DepartmentId] INT,                              
    FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([DepartmentId]) -- The connection of "many to one" with Departments
);

--  Projects
CREATE TABLE [Projects] (
    [ProjectId] INT PRIMARY KEY,                  
    [ProjectName] VARCHAR(100) NOT NULL,            
    [StartDate] DATE,                               
    [EndDate] DATE,                                 
    [DepartmentId] INT,                           
    FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([DepartmentId]) -- Many to one again
);

-- EmployeeProjects
CREATE TABLE [EmployeeProjects] (
    [EmployeeId] INT,                               
    [ProjectId] INT,                           
    [AssignmentDate] DATE,                         
    PRIMARY KEY ([EmployeeId], [ProjectId]),        
    FOREIGN KEY ([EmployeeId]) REFERENCES [Employees]([EmployeeId]),  -- Connection to employees
    FOREIGN KEY ([ProjectId]) REFERENCES [Projects]([ProjectId])      -- Connection to projects
);



INSERT INTO [Departments] ([DepartmentId], [DepartmentName], [DepartmentHead], [DepartmentLocation])
VALUES
(1, 'IT', 'Boris Johnson', 'New York'),
(2, 'Tourism', 'Lara Croft', 'London');


INSERT INTO [Employees] ([EmployeeId], [EmployeeName], [BirthDate], [Position], [DepartmentId])
VALUES
(1, 'John Deer', '1990-05-15', 'Developer', 1),
(2, 'Sarodip Veuh', '1985-11-23', 'Project Manager', 1),
(3, 'Sam Weed', '1992-07-09', 'Marketer', 2);

INSERT INTO [Projects] ([ProjectId], [ProjectName], [StartDate], [EndDate], [DepartmentId])
VALUES
(1, 'Website Redesign', '2025-01-01', '2025-06-01', 1),
(2, 'Ad Campaign', '2025-02-15', '2025-05-30', 2);


INSERT INTO [EmployeeProjects] ([EmployeeId], [ProjectId], [AssignmentDate])
VALUES
(1, 1, '2025-01-02'),
(2, 1, '2025-01-05'),
(3, 2, '2025-02-16');
