
CREATE PROCEDURE dbo.Insert_EmployeeInOut
    @Employee VARCHAR(MAX)
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
		       FROM OPENJSON(@Employee, '$')
		       WITH ([employeename]     VARCHAR(100)    '$.employeename',
                     [in_out]           VARCHAR(100)    '$.in_out',
                     [second]           INT             '$.second',
                     [attire]           VARCHAR(300)    '$.attire')

GO