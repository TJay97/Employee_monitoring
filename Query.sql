
DECLARE @Date DATETIME = GETDATE();

DECLARE @vt_Employees TABLE
(
    --Id INT IDENTITY(1,1),
    CurrentDate DATETIME,
    EmployeeName VARCHAR(50),
    InTime DATETIME,
    OutTime DATETIME,
    OnLeave BIT DEFAULT(1),
    WorkingTime DECIMAL(18,2),
    Attire VARCHAR(50)
)

DECLARE @InTime DATETIME = CONVERT(DATETIME, '2020-09-21 08:00:00');
DECLARE @OutTime DATETIME = CONVERT(DATETIME, '2020-09-21 17:00:00');


INSERT INTO @vt_Employees
(EmployeeName)
VALUES
('haritha '),
('pamudi')

UPDATE VT
SET VT.InTime = DATEADD(SECOND,INTIME.[entrysecond]/1000, @InTime),
    VT.OutTime = DATEADD(SECOND,OUTTIME.[entrysecond]/1000, @OutTime),
    VT.CurrentDate = @Date,
    VT.WorkingTime = OUTTIME.[entrysecond] - INTIME.[entrysecond],
    VT.Attire = INTIME_ATTIRE.[attire],
    VT.OnLeave = 0
FROM @vt_Employees VT
    CROSS APPLY (SELECT MIN([second]) AS [entrysecond] FROM EmployeeAttendence WHERE employeename = VT.EmployeeName AND CAST(entrydate AS DATE) = CAST(@Date AS DATE) AND in_out = 'true') [INTIME]
    CROSS APPLY (SELECT TOP 1 Attire FROM EmployeeAttendence WHERE employeename = VT.EmployeeName AND CAST(entrydate AS DATE) = CAST(@Date AS DATE) AND in_out = 'true') [INTIME_ATTIRE]
    OUTER APPLY (SELECT MAX([second]) AS [entrysecond] FROM EmployeeAttendence WHERE employeename = VT.EmployeeName AND CAST(entrydate AS DATE) = CAST(@Date AS DATE) AND in_out = 'false') [OUTTIME]

SELECT CurrentDate,
       EmployeeName,
       InTime,
       OutTime,
       CASE WHEN OnLeave = 1 THEN 'Leave' ELSE 'Worked' END AS [On_Leave],
       WorkingTime,
       Attire
  FROM @vt_Employees