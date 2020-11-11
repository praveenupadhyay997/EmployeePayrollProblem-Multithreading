// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeePayrollRepository.cs" company="Bridgelabz">
//   Copyright © 2018 Company
// </copyright>
// <creator Name="Praveen Kumar Upadhyay"/>
// --------------------------------------------------------------------------------------------------------------------
namespace EmployeePayrollMultithreading
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class EmployeePayrollRepository
    {
        /// <summary>
        /// For ensuring the established connection using the Sql Connection specifying the property
        /// </summary>
        public static SqlConnection connectionToServer { get; set; }
        /// <summary>
        /// Adding to the Employee payroll Services using the stored procedure
        /// Note that we are not passing the value of all colums as they are dependent on basic_pay and are auto-computed
        /// </summary>
        /// <param name="employeeModel"></param>
        public bool AddDataToEmployeePayrollDB(EmployeePayrollModel employeeModel)
        {
            /// Creates a new connection for every method to avoid "ConnectionString property not initialized" exception
            DBConnection dbc = new DBConnection();
            /// Calling the Get connection method to establish the connection to the Sql Server
            connectionToServer = dbc.GetConnection();
            try
            {
                /// Using the connection established
                using (connectionToServer)
                {
                    /// Implementing the stored procedure
                    SqlCommand command = new SqlCommand("dbo.AddEmployeeDetails", connectionToServer);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmpName", employeeModel.EmployeeName);
                    command.Parameters.AddWithValue("@basic_pay", employeeModel.BasicPay);
                    command.Parameters.AddWithValue("@start_date", employeeModel.StartDate);
                    command.Parameters.AddWithValue("@PhoneNumber", employeeModel.PhoneNumber);
                    command.Parameters.AddWithValue("@address", employeeModel.Address);
                    command.Parameters.AddWithValue("@department", employeeModel.Department);
                    command.Parameters.AddWithValue("@gender", employeeModel.Gender);

                    /// Opening the connection
                    connectionToServer.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    connectionToServer.Close();
                    if (rowsAffected != 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            /// Catching any type of exception generated during the run time
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connectionToServer.Close();
            }
        }
        /// <summary>
        /// UC1 -- Adding the multiple employees record to data base without the threads.
        /// Expected to have low performance than with multithreads
        /// </summary>
        /// <param name="employeeModelList">The employee list.</param>
        public bool AddEmployeeListToEmployeePayrollDataBase(List<EmployeePayrollModel> employeeModelList)
        {
            /// Iterating over all the employee model list and point individual employee detail of the list
            foreach (var employeeDetails in employeeModelList)
            {
                /// Indicating Message For the employee detail addition
                Console.WriteLine("Employee being added:" + employeeDetails.EmployeeName);
                /// Calling the method to add the data to the address book database
                /// Flag = Getting the boolean equivalent of state of data addition
                bool flag = AddDataToEmployeePayrollDB(employeeDetails);

                Console.WriteLine("Thread Execution: " + Thread.CurrentThread.ManagedThreadId);
                /// Indicating mesasage to end of data addition
                Console.WriteLine("Employee added:" + employeeDetails.EmployeeName);
                /// Check for flag status
                if (flag == false)
                    return false;
            }
            /// In case the flag status is default
            return true;
        }
        /// <summary>
        /// UC2 -- Adding the multiple employees record to data base with random threads allocation from task thread pool
        /// Expected to have high performance than without multithreads
        /// </summary>
        /// <param name="employeeModelList">The employee list.</param>
        public void AddEmployeeListToEmployeePayrollDataBaseWithThread(List<EmployeePayrollModel> employeeModelList)
        {
            /// Iterating over all the employee model list and point individual employee detail of the list
            employeeModelList.ForEach(employeeDetails =>
            {
                /// Task is a fine way of implementing multithreading using asynchronous operation
                /// Task utilises a thread pool and breaks down the program into smaller chunks and allocates a thread to it
                /// Here chunks of code can be each iteration of loop
                Task thread = new Task(() =>
                {
                    /// Indicating Message For the employee detail addition
                    Console.WriteLine("Employee Being added" + employeeDetails.EmployeeName);
                    /// Printing the current thread id being utilised
                    Console.WriteLine("Current thread id: " + Thread.CurrentThread.ManagedThreadId);
                    /// Calling the method to add the data to the address book database
                    this.AddDataToEmployeePayrollDB(employeeDetails);
                    /// Indicating mesasage to end of data addition
                    Console.WriteLine("Employee added:" + employeeDetails.EmployeeName);
                });
                thread.Start();
            });
        }
    }
}
