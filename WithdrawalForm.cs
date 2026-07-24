using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class WithdrawalForm : Form
    {
        private TextBox txtTransId, txtAccNo, txtAmount, txtPin;

        public WithdrawalForm()
        {
            DialogHelper.StyleDialog(this, "New Withdrawal", 460, 310);
            DialogHelper.AddHeader(this, "Withdraw Funds");

            int y = 56;
            txtTransId = DialogHelper.AddTextField(this, "Transaction ID", y);
            txtTransId.Text = DbHelper.NextId("Bank_Transaction", "TransID").ToString();
            y += DialogHelper.RowHeight;

            txtAccNo = DialogHelper.AddTextField(this, "Account No", y); y += DialogHelper.RowHeight;
            txtAmount = DialogHelper.AddTextField(this, "Amount", y); y += DialogHelper.RowHeight;
            txtPin = DialogHelper.AddTextField(this, "PIN", y); y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Text = "Withdraw";
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtTransId.Text, out int transId)) { MessageBox.Show("Transaction ID must be a number."); return; }
            if (!int.TryParse(txtAccNo.Text, out int accNo)) { MessageBox.Show("Account No must be a number."); return; }
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0) { MessageBox.Show("Enter a valid positive amount."); return; }

            try
            {
                DbHelper.ExecuteProcedure("sp_Withdrawal",
                    DbHelper.P("p_TransID", transId),
                    DbHelper.P("p_AccNo", accNo),
                    DbHelper.P("p_Amount", amount),
                    DbHelper.P("p_PIN", txtPin.Text.Trim()));

                if (DbHelper.RowExists("Bank_Transaction", "TransID", transId))
                {
                    MessageBox.Show("Withdrawal processed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Withdrawal failed — check the account exists and has sufficient funds/overdraft limit.",
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
