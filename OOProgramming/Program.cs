using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using OOProgramming;

namespace OOProgramming
{
    /// <summary>
    /// The main class, which runs all the programme
    /// </summary>
    static class Program
    {
        [STAThread]
        ///<summary>
        ///The main method which allows to run the programme
        ///</summary>
        static void Main()
        {
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    /// <summary>
    /// PaySlipclass allows programme to get and set the data from the file
    /// </summary>
    /// <remarks>
    /// That is how user's data is configurated correctly
    /// </remarks>
    public class PaySlip
    {
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal PayRate { get; set; }
        public bool HasTaxThreshold { get; set; }

        /// <summary>
        /// Here class getting and setting all the employees data
        /// </summary>
        /// <param name="employeeID">Employee's ID</param>
        /// <param name="firstName">Employee's first name</param>
        /// <param name="lastName">Employee's last name</param>
        /// <param name="payRate">Employee's pay rate</param>
        /// <param name="hasTaxThreshold">Employee's tax threshold availability</param>
        public PaySlip(string employeeID, string firstName, string lastName, decimal payRate, bool hasTaxThreshold)
        {
            EmployeeID = employeeID;
            FirstName = firstName;
            LastName = lastName;
            PayRate = payRate;
            HasTaxThreshold = hasTaxThreshold;

        }
        /// <summary>
        /// Gets the employee's full name
        /// </summary>
        public string FullName => $"{EmployeeID} {FirstName} {LastName}";

        /// <summary>
        /// This class is working with tax scv 
        /// </summary>
        /// <remarks>
        /// This class represents a tax rate. 
        /// It stores the lower limit, upper limit, rate, 
        /// and offset of the tax rate.
        /// </remarks>
        public class TaxRate
        {
            public decimal LowerLimit { get; set; }
            public decimal UpperLimit { get; set; }
            public decimal Rate { get; set; }
            public decimal Offset { get; set; }
            /// <summary>
            /// Reads and parses a CSV file containing tax rates and returns a list of TaxRate objects
            /// </summary>
            /// <param name="filePath">the path of the file</param>
            /// <returns>returns tax rate with own parameters</returns>
            public static List<TaxRate> LoadTaxRates(string filePath)
            {
                List<TaxRate> taxRates = new List<TaxRate>();
                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] values = line.Split(',');
                        TaxRate taxRate = new TaxRate
                        {
                            LowerLimit = decimal.Parse(values[0], CultureInfo.InvariantCulture),
                            UpperLimit = decimal.Parse(values[1], CultureInfo.InvariantCulture),
                            Rate = decimal.Parse(values[2], CultureInfo.InvariantCulture),
                            Offset = decimal.Parse(values[3], CultureInfo.InvariantCulture)
                        };
                        taxRates.Add(taxRate);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading tax rates: {ex.Message}");
                }
                return taxRates;
            }
        }
        /// <summary>
        /// This class allows to calculate pay slip
        /// </summary>
        /// <remarks>
        /// This class calculates the gross pay, tax, net pay, and superannuation for an employee
        /// </remarks>
        public class PayCalculator
        {
            /// <summary>
            /// call the tax list
            /// </summary>
            private List<TaxRate> taxRates;
            /// <summary>
            /// Calculates the gross pay for an employee given the number of hours worked and the pay rate.
            /// </summary>
            /// <param name="hoursWorked">user's input</param>
            /// <param name="payRate">parameter for the specific employee, taken from the scv file</param>
            /// <returns>gross pay</returns>
            public virtual decimal CalculateGrossPay(decimal hoursWorked, decimal payRate)
            {
                return hoursWorked * payRate;
            }
            /// <summary>
            /// Calculates the tax for an employee given the gross pay and tax threshold status.
            /// </summary>
            /// <param name="grossPay"> result from the previous method</param>
            /// <param name="hasTaxThreshold"> boolean from the specific employee data</param>
            /// <returns>tax ammount</returns>
            public virtual decimal CalculateTax(decimal grossPay, bool hasTaxThreshold)
            {
                if (hasTaxThreshold)
                {

                    taxRates = TaxRate.LoadTaxRates("taxrate-withthreshold.csv");
                }
                else
                {
                    taxRates = TaxRate.LoadTaxRates("taxrate-nothreshold.csv");
                }
                foreach (TaxRate rate in taxRates)
                {

                    if (grossPay <= rate.UpperLimit && grossPay >= rate.LowerLimit)
                    {
                        
                        return grossPay * rate.Rate - rate.Offset;
                    }

                }
                return 0;
            }
            /// <summary>
            /// Calculates the net pay for an employee given the gross pay and tax
            /// </summary>
            /// <param name="grossPay">result of the CalculateGrossPay method</param>
            /// <param name="tax">result of the CalculateTax method</param>
            /// <returns>returns the net pay</returns>
            public virtual decimal CalculateNetPay(decimal grossPay, decimal tax)
            {
                return grossPay - tax;
            }
            /// <summary>
            /// Calculates the superannuation for an employee given the gross pay.
            /// </summary>
            /// <param name="grossPay">result of the CalculateGrossPay method</param>
            /// <returns>return s the super ammount</returns>
            public virtual decimal CalculateSuperannuation(decimal grossPay)
            {
                return grossPay * 0.11m;
            }
        }
    }
}
