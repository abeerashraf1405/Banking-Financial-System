using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class TransferForm : Form
    {
        private TextBox txtTransId, txtFromAcc, txtToAcc, txtAmount, txtEmpId;

        public TransferForm()
        {
            DialogHelper.StyleDialog(this, "New Transfer", 460, 360);
            DialogHelper.AddHeader(this, "Transfer Funds");

            int y = 56;
            txtTransId = DialogHelper.AddTextField(this, "Transaction ID", y);
            txtTransId.Text = DbHelper.NextId("Bank_Transaction", "TransID").ToString();
            y += DialogHelper.RowHeight;

            txtFromAcc = DialogHelper.AddTextField(this, "From Account No", y); y += DialogHelper.RowHeight;
            txtToAcc = DialogHelper.AddTextField(this, "To Account No", y); y += DialogHelper.RowHeight;
            txtAmount = DialogHelper.AddTextField(this, "Amount", y); y += DialogHelper.RowHeight;
            txtEmpId = DialogHelper.AddTextField(this, "Employee ID (optional)", y); y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Text = "Transfer";
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtTransId.Text, out int transId)) { MessageBox.Show("Transaction ID must be a number."); return; }
            if (!int.TryParse(txtFromAcc.Text, out int fromAcc)) { MessageBox.Show("From Account No must be a number."); return; }
            if (!int.TryParse(txtToAcc.Text, out int toAcc)) { MessageBox.Show("To Account No must be a number."); return; }
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0) { MessageBox.Show("Enter a valid positive amount."); return; }

            int? empId = null;
            if (!string.IsNullOrWhiteSpace(txtEmpId.Text))
            {
                if (!int.TryParse(txtEmpId.Text, out int e2)) { MessageBox.Show("Employee ID must be a number, or leave blank."); return; }
                empId = e2;
            }

            try
            {
                DbHelper.ExecuteProcedure("sp_Transfer",
                    DbHelper.P("p_TransID", transId),
                    DbHelper.P("p_FromAccNo", fromAcc),
                    DbHelper.P("p_ToAccNo", toAcc),
                    DbHelper.P("p_Amount", amount),
                    DbHelper.P("p_EmpID", (object)empId ?? DBNull.Value));

                if (DbHelper.RowExists("Bank_Transaction", "TransID", transId))
                {
                    MessageBox.Show("Transfer processed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Transfer failed — check both accounts exist and the source has sufficient balance.",
                        "Not Processed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
