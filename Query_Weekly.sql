

DECLARE @StartDate DATETIME = '2020-08-31';
DECLARE @EndDate DATETIME = '2020-09-30';

DECLARE @vt_Employees TABLE
(
    --Id INT IDENTITY(1,1),
    CurrentDate DATETIME,
    EmployeeName VARCHAR(50),
    InTime INT,
    OutTime INT,
    OnLeave BIT DEFAULT(1),
    WorkingTime DECIMAL(18,2),
    Attire VARCHAR(50)
)


INSERT INTO @vt_Employees
(EmployeeName)
VALUES
('kumudu '),
('supun'),
('Oshen')

UPDATE VT
SET VT.InTime = INTIME.[entrysecond],
    VT.OutTime = OUTTIME.[entrysecond],
    VT.WorkingTime = OUTTIME.[entrysecond] - INTIME.[entrysecond]
FROM @vt_Employees VT
    CROSS APPLY (SELECT AVG([second]) AS [entrysecond] FROM EmployeeAttendence WHERE employeename = VT.EmployeeName AND (CAST(entrydate AS DATE) BETWEEN @StartDate AND @EndDate) AND in_out = 'true') [INTIME]
    OUTER APPLY (SELECT AVG([second]) AS [entrysecond] FROM EmployeeAttendence WHERE employeename = VT.EmployeeName AND (CAST(entrydate AS DATE) BETWEEN @StartDate AND @EndDate) AND in_out = 'false') [OUTTIME]

SELECT EmployeeName,
       InTime,
       OutTime,
       WorkingTime
  FROM @vt_Employees