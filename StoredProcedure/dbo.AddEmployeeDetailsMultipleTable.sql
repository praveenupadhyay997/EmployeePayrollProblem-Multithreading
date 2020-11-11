
CREATE PROCEDURE [dbo].[AddEmployeeDetailsMultipleTable]
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
	insert into employee_payroll(employee_name, start_date, gender, phoneNumber, address)
values (@EmpName, @start_date, @gender, @PhoneNumber, @address)
DECLARE @lastEmpId AS int;
	SET @lastEmpId = @@IDENTITY;
select @lastEmpId;
insert into employee_department(employee_id, department, basic_pay)
values(@lastEmpId, @department, @basic_pay);
insert into payroll
values(@basic_pay, 0.2*@basic_pay, 0.8*@basic_pay, 0.08*@basic_pay, 0.92*@basic_pay);
commit transaction;
return -1;
end try
begin catch
rollback transaction;
end catch
end

EXEC AddEmployeeDetailsMultipleTable @EmpName= 'Deepika', @basic_pay = 30000, @start_date= '2018-07-05',
	@PhoneNumber = 95655975, @address = 'Punjab', @department = 'HR', @gender = 'F';