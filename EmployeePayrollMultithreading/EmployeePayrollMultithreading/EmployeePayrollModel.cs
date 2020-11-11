// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeePayrollModel.cs" company="Bridgelabz">
//   Copyright © 2018 Company
// </copyright>
// <creator Name="Praveen Kumar Upadhyay"/>
// --------------------------------------------------------------------------------------------------------------------
namespace EmployeePayrollMultithreading
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// Class to map the relational data base model to a entity
    /// Containd fiels mimicing the exact replica of that of the table level
    /// </summary>
    public class EmployeePayrollModel
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public long PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public double BasicPay { get; set; }
        public int BasicPayAsIntegral { get; set; }
        public double Deductions { get; set; }
        public double TaxablePay { get; set; }
        public double Tax { get; set; }
        public double NetPay { get; set; }
        public DateTime StartDate { get; set; }
    }
}
