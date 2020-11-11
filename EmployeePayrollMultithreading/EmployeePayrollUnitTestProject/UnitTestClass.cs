// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestClass.cs" company="Bridgelabz">
//   Copyright © 2018 Company
// </copyright>
// <creator Name="Praveen Kumar Upadhyay"/>
// --------------------------------------------------------------------------------------------------------------------
namespace EmployeePayrollUnitTestProject
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EmployeePayrollMultithreading;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System;

    [TestClass]
    public class UnitTestClass
    {
        /// <summary>
        /// Creating an instance of the employee payroll class  so as to invoke the adding methods for records
        /// </summary>
        public static EmployeePayrollRepository employeePayroll = new EmployeePayrollRepository();
        /// <summary>
        /// TC1 -- To test for addition of multiple data to the list without using the multi-thread
        /// By default cprogram execution is single threaded
        /// Calculating the elapsed time too
        /// </summary>
        [TestMethod]
        public void AddingMultipleDataWithoutThreads_GettingTimeOfExecution()
        {
            //Arrange
            /// Creating the list of the employee records with data attributes
            List<EmployeePayrollModel> employeeList = new List<EmployeePayrollModel>();
            employeeList.Add(new EmployeePayrollModel { EmployeeName = "Ali", BasicPay =40000, StartDate = new System.DateTime(2020, 01, 01), PhoneNumber =78945678, Address = "Delhi", Department = "Sales", Gender = "M"});
            employeeList.Add(new EmployeePayrollModel { EmployeeName = "Kabir", BasicPay = 50000, StartDate = new System.DateTime(2019, 02, 01), PhoneNumber = 98545678, Address = "Hyderabad", Department = "Marketing", Gender = "M" });
            employeeList.Add(new EmployeePayrollModel { EmployeeName = "Deepti", BasicPay = 45000, StartDate = new System.DateTime(2019, 11, 06), PhoneNumber = 98785678, Address = "Mumbai", Department = "Accounts", Gender = "F" });
            employeeList.Add(new EmployeePayrollModel { EmployeeName = "Archana", BasicPay = 60000, StartDate = new System.DateTime(2018, 02, 01), PhoneNumber = 72061678, Address = "Mumbai", Department = "Sales", Gender = "F" });
            employeeList.Add(new EmployeePayrollModel { EmployeeName = "Tarun", BasicPay = 50000, StartDate = new System.DateTime(2018, 12, 12), PhoneNumber = 89781678, Address = "Delhi", Department = "HR", Gender = "M" });
            bool expected = true;
            /// To measure the time of execution from System.Diagonostic namespace
            Stopwatch timeCounter = new Stopwatch();
            /// Act - invoke the method to get the actual value
            timeCounter.Start();
            /// Getting the boolean comparison of the number of rows affected froom the ExecuteNonQuery
            /// if rowsaffected > 0 then true (symbolising the insertion of data)
            bool actual = employeePayroll.AddEmployeeListToEmployeePayrollDataBase(employeeList);
            timeCounter.Stop();
            Console.WriteLine("Elapsed time without using the threads: " + timeCounter.ElapsedMilliseconds);
            /// Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
