USE [payroll_services]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddEmployeeDetails]
	@EmpName varchar(50),
    @basic_pay float,
	@start_date datetime,
	@PhoneNumber bigint,
	@address varchar(50),
	@department varchar(50),
	@gender varchar(50)
as
begin
begin try
begin transaction
	insert into employee_payroll_services (EmployeeName, BasicPay, StartDate, PhoneNumber, Address, Department, Gender, Deductions, TaxablePay, Tax, NetPay)  values
	(@EmpName, @basic_pay, @start_date, @PhoneNumber, @address, @department ,@gender, (0.2*@basic_pay), (0.8 * @basic_pay), (0.1*0.8*@basic_pay), (0.92*@basic_pay));
commit transaction;
return -1;
end try
begin catch
rollback transaction;
end catch
end
GO