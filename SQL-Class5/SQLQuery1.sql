Use DataBase3;
GO

-------------------------------------------------------

--1. Extract a year

SELECT EXTRACT(YEAR FROM my_date_column) AS year_only
FROM my_table;

--2. Find SQL text

SELECT *
FROM my_table
WHERE my_text_column ILIKE '%SQL%';

--3. Replace cat with dog

SELECT REPLACE(my_text_column, 'cat', 'dog') AS replaced_text
FROM my_table;

--4. Split before the first -

SELECT SPLIT_PART(my_text_column, ' - ', 1) AS before_dash
FROM my_table;

--5. Split after the last /

SELECT REVERSE(SPLIT_PART(REVERSE(my_text_column), ' / ', 1)) AS after_last_slash
FROM my_table;

--7. Extract a weekday

SELECT TO_CHAR(my_date_column, 'Day') AS weekday
FROM my_table;

--8. Find text in which column number is < 10

SELECT *
FROM my_table
WHERE my_column_number < 10;

--9. Average column number

SELECT AVG(my_column_number) AS average_value
FROM my_table;

--10. Sum of values where date is > 01.01.2021

SELECT SUM(my_column_number) AS total_sum
FROM my_table
WHERE my_date_column > '2021-01-01';


--11. Extract month and year

SELECT TO_CHAR(my_date_column, 'MM/YYYY') AS month_year
FROM my_table;

--12. Trim spaces

SELECT TRIM(my_text_column) AS trimmed_text
FROM my_table;

--13. Difference between two dates

SELECT my_date_column - someother_date_column AS date_difference
FROM my_table;

--14. Value sum by groups

SELECT my_text_group_column, SUM(my_number_column) AS total_per_group
FROM my_table
GROUP BY my_text_group_column;

--15. UPPER!

SELECT UPPER(my_text_column) AS uppercase_text
FROM my_table;