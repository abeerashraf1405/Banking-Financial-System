using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class DepositForm : Form
    {
        private TextBox txtTransId, txtAccNo, txtAmount, txtEmpId, txtDesc;

        public DepositForm()
        {
            DialogHelper.StyleDialog(this, "New Deposit", 460, 350);
            DialogHelper.AddHeader(this, "Deposit Funds");

            int y = 56;
            txtTransId = DialogHelper.AddTextField(this, "Transaction ID", y);
            txtTransId.Text = DbHelper.NextId("Bank_Transaction", "TransID").ToString();
            y += DialogHelper.RowHeight;

            txtAccNo = DialogHelper.AddTextField(this, "Account No", y); y += DialogHelper.RowHeight;
            txtAmount = DialogHelper.AddTextField(this, "Amount", y); y += DialogHelper.RowHeight;
            txtEmpId = DialogHelper.AddTextField(this, "Employee ID (optional)", y); y += DialogHelper.RowHeight;
            txtDesc = DialogHelper.AddTextField(this, "Description", y); y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Text = "Deposit";
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtTransId.Text, out int transId)) { MessageBox.Show("Transaction ID must be a number."); return; }
            if (!int.TryParse(txtAccNo.Text, out int accNo)) { MessageBox.Show("Account No must be a number."); return; }
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0) { MessageBox.Show("Enter a valid positive amount."); return; }

            int? empId = null;
            if (!string.IsNullOrWhiteSpace(txtEmpId.Text))
            {
                if (!int.TryParse(txtEmpId.Text, out int e2)) { MessageBox.Show("Employee ID must be a number, or leave blank."); return; }
                empId = e2;
            }

            try
            {
                DbHelper.ExecuteProcedure("sp_Deposit",
                    DbHelper.P("p_TransID", transId),
                    DbHelper.P("p_AccNo", accNo),
                    DbHelper.P("p_Amount", amount),
                    DbHelper.P("p_EmpID", (object)empId ?? DBNull.Value),
                    DbHelper.P("p_Description", txtDesc.Text.Trim()));

                if (DbHelper.RowExists("Bank_Transaction", "TransID", transId))
                {
                    MessageBox.Show("Deposit processed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Deposit failed — check that the Account No exists.", "Not Processed",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
