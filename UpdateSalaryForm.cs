using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class UpdateSalaryForm : Form
    {
        private readonly int empId;
        private TextBox txtSalary;

        public UpdateSalaryForm(int empId)
        {
            this.empId = empId;
            DialogHelper.StyleDialog(this, $"Update Salary — Employee #{empId}", 420, 180);
            DialogHelper.AddHeader(this, "Update Salary");

            int y = 56;
            txtSalary = DialogHelper.AddTextField(this, "New Salary", y);
            y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary <= 0)
            {
                MessageBox.Show("Enter a valid positive salary."); return;
            }

            try
            {
                DbHelper.ExecuteProcedure("sp_UpdateSalary",
                    DbHelper.P("p_EmpID", empId),
                    DbHelper.P("p_NewSalary", salary));

                MessageBox.Show("Salary updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
