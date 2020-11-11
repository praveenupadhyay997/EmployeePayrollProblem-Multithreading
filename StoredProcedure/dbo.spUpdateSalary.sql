CREATE PROCEDURE dbo.spUpdateSalary	
	@salary int,
	@name varchar(50)
AS
BEGIN
SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRANSACTION;
Update employee_payroll_services set BasicPay = @salary
where EmployeeName = @name;
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
select ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
if(XACT_STATE())=-1
BEGIN
PRINT N'The transaction is in an uncommitable state.'+'Rolling back transaction.'
ROLLBACK TRANSACTION;
END;
if(XACT_STATE())=1
BEGIN
PRINT N'The transaction is commitable state.'+'committing transaction.'
COMMIT TRANSACTION;
END;
END CATCH
END