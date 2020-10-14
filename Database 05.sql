create table EmployeeAttendence
(
    id int IDENTITY(1,1),
    employeename varchar(100),
    entrydate datetime DEFAULT(GETDATE()),
    in_out varchar(20),
    second int,
    attire varchar(500)
)
