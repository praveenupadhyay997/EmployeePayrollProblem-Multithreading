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
        /// Adding NLog Class to feed the log details for proper monitoring
        NLog nLog = new NLog();
        /// <summary>
        /// Mutex is a synchronization primitive to implement interthread execution synchronization
        /// Which means a thread can be locked i.e. Untill the thread completes it's execution new thread will not be permitted the entry
        /// </summary>       
        private static Mutex threadMute = new Mutex();
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
                nLog.LogDebug("Adding the Employee: " + employeeDetails.EmployeeName + "via ThreadID: " + Thread.CurrentThread.ManagedThreadId);
                /// Indicating Message For the employee detail addition
                Console.WriteLine("Employee being added:" + employeeDetails.EmployeeName);
                /// Calling the method to add the data to the address book database
                /// Flag = Getting the boolean equivalent of state of data addition
                bool flag = AddDataToEmployeePayrollDB(employeeDetails);

                Console.WriteLine("Thread Execution: " + Thread.CurrentThread.ManagedThreadId);
                /// Indicating mesasage to end of data addition
                Console.WriteLine("Employee added:" + employeeDetails.EmployeeName);
                nLog.LogInfo("Employee Successfully added in Database via ThreadId: " + Thread.CurrentThread.ManagedThreadId);
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
                nLog.LogDebug("Adding the Employee: " + employeeDetails.EmployeeName + "via ThreadID: " + Thread.CurrentThread.ManagedThreadId);
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
                    nLog.LogInfo("Employee Successfully added in Database via ThreadId: " + Thread.CurrentThread.ManagedThreadId);
                });
                thread.Start();
            });
        }
        /// <summary>
        /// UC3 -- Adding the multiple employees record to data base with synchronised threads allocation from task thread pool
        /// Expected to have slightly lower performance than with asynchronised thread operation
        /// But the synchronized thread execution is much safe to execute and does not pose a real-time conflict threat for CRUD operation on Database
        /// </summary>
        /// <param name="employeeModelList">The employee list.</param>
        public void AddEmployeeListToEmployeePayrollDataBaseWithThreadSynchronization(List<EmployeePayrollModel> employeeModelList)
        {
            /// Iterating over all the employee model list and point individual employee detail of the list
            employeeModelList.ForEach(employeeDetails =>
            {
                nLog.LogDebug("Adding the Employee: " + employeeDetails.EmployeeName + "via ThreadID: " + Thread.CurrentThread.ManagedThreadId);
                /// Task is a fine way of implementing multithreading using asynchronous operation
                /// Task utilises a thread pool and breaks down the program into smaller chunks and allocates a thread to it
                /// Here chunks of code can be each iteration of loop
                Task thread = new Task(() =>
                {
                    /// This will block any incoming thread unless the thread execution completes
                    threadMute.WaitOne();
                    /// Indicating Message For the employee detail addition
                    Console.WriteLine("Employee Being added:" + employeeDetails.EmployeeName);
                    /// Printing the current thread id being utilised
                    Console.WriteLine("Current thread id: " + Thread.CurrentThread.ManagedThreadId);
                    /// Calling the method to add the data to the address book database
                    this.AddDataToEmployeePayrollDB(employeeDetails);
                    /// Indicating mesasage to end of data addition
                    Console.WriteLine("Employee added:" + employeeDetails.EmployeeName);
                    nLog.LogInfo("Employee Successfully added in Database via ThreadId: " + Thread.CurrentThread.ManagedThreadId);
                    /// It marks the end of execution of thread and passes a signal to WaitHandle to allow execution of other thread
                    threadMute.ReleaseMutex();
                });
                thread.Start();
                thread.Wait();
            });
        }
        /// <summary>
        /// UC5 -- Adding the multiple employees record to data base with inter-related schemas i.e. payroll db is related to schema of employee data
        /// Since we are inserting the data to multiple table simultaneously we need to lock the code
        /// But the synchronized thread execution is much safe to execute and does not pose a real-time conflict threat for CRUD operation on Database
        /// </summary>
        /// <param name="employeeModelList">The employee list.</param>
        public void AddEmployeeListToMultipleTableWithThreadSynchronization(List<EmployeePayrollModel> employeeModelList)
        {
            /// Iterating over all the employee model list and point individual employee detail of the list
            employeeModelList.ForEach(employeeDetails =>
            {
                nLog.LogDebug("Adding the Employee: " + employeeDetails.EmployeeName + "via ThreadID: " + Thread.CurrentThread.ManagedThreadId);
                /// Task is a fine way of implementing multithreading using asynchronous operation
                /// Task utilises a thread pool and breaks down the program into smaller chunks and allocates a thread to it
                /// Here chunks of code can be each iteration of loop
                Task thread = new Task(() =>
                {
                    /// This will block any incoming thread unless the thread execution completes
                    threadMute.WaitOne();
                    /// Indicating Message For the employee detail addition
                    Console.WriteLine("Employee Being added:" + employeeDetails.EmployeeName);
                    /// Printing the current thread id being utilised
                    Console.WriteLine("Current thread id: " + Thread.CurrentThread.ManagedThreadId);
                    /// Calling the method to add the data to the address book database
                    this.AddToMultipleTableAndPayrollTableAtOnce(employeeDetails);
                    /// Indicating mesasage to end of data addition
                    Console.WriteLine("Employee added:" + employeeDetails.EmployeeName);
                    nLog.LogInfo("Employee Successfully added in Database via ThreadId: " + Thread.CurrentThread.ManagedThreadId);
                    /// It marks the end of execution of thread and passes a signal to WaitHandle to allow execution of other thread
                    threadMute.ReleaseMutex();
                });
                thread.Start();
                thread.Wait();
            });
        }
        /// <summary>
        /// UC5 -- Adding to the multiple tables at once using the stored procedure
        /// Implementing the schema condition for computation of employee wage breakout
        /// </summary>
        /// <param name="employeeModel"></param>
        public bool AddToMultipleTableAndPayrollTableAtOnce(EmployeePayrollModel employeeModel)
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
                    SqlCommand command = new SqlCommand("dbo.AddEmployeeDetailsMultipleTable", connectionToServer);
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
        /// UC6 -- Function to update the basic pay of the employee and all the other properties related to it
        /// deduction = 0.2* basic_pay, taxable pay = 0.8*basic_pay, tax = 0.08*basic-pay, net pay = 0.98*basic_pay
        /// </summary>
        /// <param name="empName"></param>
        /// <returns></returns>
        public bool UpdateDataForEmployee(string empName, double newBasicPay)
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
                    /// Opening the connection
                    connectionToServer.Open();
                    /// Update query  for the table and binding with the parameter passed
                    string query = @"update dbo.employee_payroll_services set BasicPay= @parameter1, Deductions=0.2*@parameter1,
                                     TaxablePay = 0.8*@parameter1, Tax = 0.08*@parameter1, NetPay = 0.92*@parameter1 where EmployeeName = @parameter2";
                    /// Impementing the command on the connection fetched database table
                    SqlCommand command = new SqlCommand(query, connectionToServer);
                    /// Adding the log message to the log file
                    nLog.LogDebug($"Updating the Employee : {empName} Basic Pay to : {newBasicPay} ");
                    /// Binding the parameter to the formal parameters
                    command.Parameters.AddWithValue("@parameter1", newBasicPay);
                    command.Parameters.AddWithValue("@parameter2", empName);
                    /// Storing the result of the executed query
                    var result = command.ExecuteNonQuery();
                    connectionToServer.Close();
                    if (result != 0)
                    {
                        nLog.LogInfo("Employee detail Successfully updated in Database via ThreadId: " + Thread.CurrentThread.ManagedThreadId);
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
    }
}
