using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class AddAccountForm : Form
    {
        private TextBox txtAccNo, txtBalance, txtRate, txtCusId, txtBranchId;
        private ComboBox cboType;

        public AddAccountForm()
        {
            DialogHelper.StyleDialog(this, "Open New Account", 460, 400);
            DialogHelper.AddHeader(this, "Account Details");

            int y = 56;
            txtAccNo = DialogHelper.AddTextField(this, "Account No", y);
            txtAccNo.Text = DbHelper.NextId("Account", "AccNo").ToString();
            y += DialogHelper.RowHeight;

            txtBalance = DialogHelper.AddTextField(this, "Opening Balance", y); y += DialogHelper.RowHeight;
            txtRate = DialogHelper.AddTextField(this, "Interest Rate (e.g. 0.02)", y); y += DialogHelper.RowHeight;
            cboType = DialogHelper.AddComboField(this, "Account Type", y, new[] { "Checking", "Saving", "CD" });
            y += DialogHelper.RowHeight;
            txtCusId = DialogHelper.AddTextField(this, "Customer ID", y); y += DialogHelper.RowHeight;
            txtBranchId = DialogHelper.AddTextField(this, "Branch ID", y); y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAccNo.Text, out int accNo)) { MessageBox.Show("Account No must be a number."); return; }
            if (!decimal.TryParse(txtBalance.Text, out decimal balance)) { MessageBox.Show("Enter a valid opening balance."); return; }
            if (!decimal.TryParse(txtRate.Text, out decimal rate)) { MessageBox.Show("Enter a valid interest rate."); return; }
            if (!int.TryParse(txtCusId.Text, out int cusId)) { MessageBox.Show("Customer ID must be a number."); return; }
            if (!int.TryParse(txtBranchId.Text, out int branchId)) { MessageBox.Show("Branch ID must be a number."); return; }

            try
            {
                DbHelper.ExecuteProcedure("sp_AddAccount",
                    DbHelper.P("p_AccNo", accNo),
                    DbHelper.P("p_Balance", balance),
                    DbHelper.P("p_Rate", rate),
                    DbHelper.P("p_TypeCode", cboType.SelectedItem.ToString()),
                    DbHelper.P("p_CusID", cusId),
                    DbHelper.P("p_BranchID", branchId));

                if (DbHelper.RowExists("Account", "AccNo", accNo))
                {
                    MessageBox.Show("Account opened successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Could not open account — check that the Customer ID and Branch ID exist.",
                        "Not Added", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
