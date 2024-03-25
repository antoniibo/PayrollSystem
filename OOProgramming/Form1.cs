using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static OOProgramming.PaySlip;
using СSVWriter;

namespace OOProgramming
{
    /// <summary>
    /// this class conecting code with WindowsForms
    /// </summary>
    /// <remarks>
    ///This class represents the main form of the application. 
    ///It is responsible for displaying a list of employees, 
    ///allowing the user to select an employee, 
    ///and displaying the pay slip for the selected employee.
    /// </remarks>
    public partial class Form1 : Form
    {
        /// <summary>
        /// A list of PaySlip objects representing the employees in the application.
        /// </summary>
        /// <remarks>
        /// For example each employee has their own wage and hours worked,
        /// so that part of code allows to save them correctly
        /// </remarks>
        private List<PaySlip> employeeList;
        public Form1()
        {
            InitializeComponent();
            LoadEmployeeData();
        }
        /// <summary>
        /// Reads and parses the CSV file containing employee data and populates the employeeList property.
        /// </summary>
        private void LoadEmployeeData()
        {
            employeeList = File.ReadAllLines("../../../../employee.csv")
                .Select(line => line.Split(','))
                .Select(data => new PaySlip(data[0], data[1], data[2], decimal.Parse(data[3]), data[4].Trim().ToUpper() == "Y"))
                .ToList();
            listBox1.DataSource = employeeList;
            listBox1.DisplayMember = "FullName";
        }
        /// <summary>
        /// Saves the pay slip data for the selected employee to a CSV file.
        /// </summary>
        /// <remarks>
        /// User cklick on the button "Save" employee's payslip is saving as scv file in specific folder 
        /// </remarks>
        private void SavePaymentDataToCSV()
        {
            PaySlip selectedEmployee = (PaySlip)listBox1.SelectedItem;

            if (selectedEmployee != null && decimal.TryParse(textBox1.Text, out decimal hoursWorked))
            {
                PayCalculator calculator = new PayCalculator();
                decimal grossPay = calculator.CalculateGrossPay(hoursWorked, selectedEmployee.PayRate);
                decimal tax = calculator.CalculateTax(grossPay, selectedEmployee.HasTaxThreshold);
                decimal netPay = calculator.CalculateNetPay(grossPay, tax);
                decimal superannuation = calculator.CalculateSuperannuation(grossPay);

                string fileName = $"PaySlip_{selectedEmployee.FullName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.csv";
                string directoryPath = "../../../../EmployeesPaySlip";
                string filePath = Path.Combine(directoryPath, fileName);
                string csvContent = $"EmployeeID: {selectedEmployee.EmployeeID}\n Full Name: {selectedEmployee.FullName}\n Hours Worked: {hoursWorked}\n Hourly Rate: ${selectedEmployee.PayRate}\n Tax Threshold: {(selectedEmployee.HasTaxThreshold ? "Yes" : "No")}\n Gross Pay: ${grossPay}\n Tax: {tax}\n Net Pay: ${netPay}\n Superannuation: ${superannuation}";


                CSVWriter.WriteFile(filePath, csvContent);

                MessageBox.Show($"Payment data saved to {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// Calculates the pay slip data for the selected employee and displays it in the text box
        /// </summary>
        /// <remarks>
        /// So when person clicking on "Calculate" button, the Pay Slip is calculating automatically
        /// </remarks>
        private void CalculateAndDisplayPayment()
        {
            PaySlip selectedEmployee = (PaySlip)listBox1.SelectedItem;

            if (selectedEmployee != null && decimal.TryParse(textBox1.Text, out decimal hoursWorked))
            {
                PayCalculator calculator = new PayCalculator();

                decimal grossPay = calculator.CalculateGrossPay(hoursWorked, selectedEmployee.PayRate);
                decimal tax = calculator.CalculateTax(grossPay, selectedEmployee.HasTaxThreshold);
                decimal netPay = calculator.CalculateNetPay(grossPay, tax);
                decimal superannuation = calculator.CalculateSuperannuation(grossPay);

                textBox2.Text = $"Employee ID: {selectedEmployee.EmployeeID}\r\n" +
                                $"Name: {selectedEmployee.FullName}\r\n" +
                                $"Hours Worked: {hoursWorked}\r\n" +
                                $"Hourly Rate: {selectedEmployee.PayRate}\r\n" +
                                $"Tax Threshold: {(selectedEmployee.HasTaxThreshold ? "Yes" : "No")}\r\n" +
                                $"Gross Pay: {grossPay}\r\n" +
                                $"Tax: {tax}\r\n" +
                                $"Net Pay: {netPay}\r\n" +
                                $"Superannuation: {superannuation}";
            }
        }
        /// <summary>
        /// Handles the click event for the "Calculate" button.
        /// </summary>
        /// <remarks>
        /// So when user add input in the textBox and press "Calculate" button,
        /// then input is validating (only numbers can be entered) and call the method
        /// If input is incorrect, the error would be displayed
        /// </remarks>
        /// <param name="sender">Identifies the object that raised the event</param>
        /// <param name="e">Provides additional information about the event, such as the type of event and any relevant data</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out decimal hoursWorked))
            {
                CalculateAndDisplayPayment();
                button2.Visible = true;
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a valid number for hours worked.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Handles the click event for the "Save" button
        /// </summary>
        /// <remarks>
        /// So when user add input in the textBox and press "Save" button,
        /// then input is validating (only numbers can be entered) and call the method.
        /// If input is incorrect, the error would be displayed
        /// </remarks>
        /// <param name="sender">Identifies the object that raised the event</param>
        /// <param name="e">Provides additional information about the event, such as the type of event and any relevant data</param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out decimal hoursWorked))
            {
                SavePaymentDataToCSV();
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a valid number for hours worked.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
