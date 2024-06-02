CREATE OR ALTER VIEW UsrVwContactAgeDays
As
Select Id as UsrId, Name as UsrName, Birthdate as UsrBirthDate,
DATEDIFF(day, BirthDate, GETDATE()) as UsrAgeDays,
Id as UsrContactId
From Contact