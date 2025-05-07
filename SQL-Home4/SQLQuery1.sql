use Airport
GO

CREATE TABLE Passengers (
    passenger_id INT PRIMARY KEY,
    full_name VARCHAR(50),
    passport_number VARCHAR(20)
);

CREATE TABLE Flights (
    flight_id INT PRIMARY KEY,
    destination VARCHAR(50),
    departure_date DATE,
    departure_time TIME,
    arrival_time TIME,
    duration_mins TIME
);


CREATE TABLE Tickets (
    ticket_id INT PRIMARY KEY,
    flight_id INT,
    passenger_id INT,
    ticket_class VARCHAR(10),
    price MONEY,
    purchase_date DATE,
    FOREIGN KEY (flight_id) REFERENCES Flights(flight_id),
    FOREIGN KEY (passenger_id) REFERENCES Passengers(passenger_id)
);


INSERT INTO Flights VALUES
(1, 'Kyiv', '2025-05-06', '08:00:00', '10:30:00', 150),
(2, 'Lviv', '2025-05-06', '09:00:00', '10:00:00', 60),
(3, 'Detroit', '2025-05-07', '12:00:00', '14:30:00', 150),
(4, 'Odesa', '2025-05-06', '13:00:00', '15:15:00', 135);


INSERT INTO Passengers VALUES
(1, 'Killian Murphy', 'AA123456'),
(2, 'Anna Ivanova', 'BB654321');


INSERT INTO Tickets VALUES
(1, 1, 1, 'economy', 1500.00, '2025-05-01'),
(2, 2, 2, 'business', 3000.00, '2025-05-01');


--1. Flights to a city on a date, sorted by time

SELECT * FROM Flights
WHERE destination_city = 'Kyiv' AND departure_date = '2025-05-06'
ORDER BY departure_time;

--2. Flight with the longest duration

SELECT TOP 1 *
FROM Flights
ORDER BY duration_minutes DESC;

--3. Longer than 2 hour flights

SELECT * FROM Flights
WHERE duration_minutes > 120;

--4. Total flights in each city

SELECT destination_city, COUNT(*) AS total_flights
FROM Flights
GROUP BY destination_city;

--5. City with the most flights

SELECT TOP 1 *
FROM Flights
GROUP BY destination_city
ORDER BY COUNT(*) DESC;

--6. Flights in each city + total count

-- In each city
SELECT destination_city, COUNT(*) AS flights_in_city
FROM Flights
WHERE EXTRACT(MONTH FROM departure_date) = 5
GROUP BY destination_city;


-- Total count
SELECT COUNT(*) AS total_flights
FROM Flights
WHERE EXTRACT(MONTH FROM departure_date) = 5;

--7. Flights today in business class

SELECT f.*
FROM Flights f
WHERE f.departure_date = Getdate()
AND f.flight_id IN (
    SELECT t.flight_id
    FROM Tickets t
    WHERE t.ticket_class = 'business'
    GROUP BY t.flight_id
    HAVING COUNT(*) < 10 -- 10 business seats in total
);

--8. Number and sum of tickets for a specific date

SELECT COUNT(*) AS tickets_sold, SUM(price) AS total_sum
FROM Tickets
WHERE purchase_date = '2025-05-01';

--9. Pre-sales for specific date

SELECT f.flight_id, f.destination_city, COUNT(t.ticket_id) AS tickets_sold
FROM Flights f
LEFT JOIN Tickets t ON f.flight_id = t.flight_id
WHERE f.departure_date = '2025-05-06'
GROUP BY f.flight_id, f.destination_city;

--10. All flights and cities

SELECT DISTINCT flight_id, destination_city
FROM Flights;






