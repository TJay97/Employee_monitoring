
CREATE PROCEDURE dbo.Insert_EmployeeInOut
    @Employee01 VARCHAR(MAX),
    @Employee02 VARCHAR(MAX)
AS

    INSERT INTO EmployeeAttendence
                (
                    [employeename],
                    [in_out],
                    [second],
                    [attire]
                )
             SELECT [employeename],
                    [in_out],
                    [second],
                    [attire]
		       FROM OPENJSON(@Employee01, '$')
		       WITH ([employeename]     VARCHAR(100)    '$.employeename',
                     [in_out]           VARCHAR(100)    '$.in_out',
                     [second]           INT             '$.second',
                     [attire]           VARCHAR(300)    '$.attire')


    INSERT INTO EmployeeAttendence
                (
                    [employeename],
                    [in_out],
                    [second],
                    [attire]
                )
             SELECT [employeename],
                    [in_out],
                    [second],
                    [attire]
		       FROM OPENJSON(@Employee02, '$')
		       WITH ([employeename]     VARCHAR(100)    '$.employeename',
                     [in_out]           VARCHAR(100)    '$.in_out',
                     [second]           INT             '$.second',
                     [attire]           VARCHAR(300)    '$.attire')


GO