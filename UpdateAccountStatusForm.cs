using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class UpdateAccountStatusForm : Form
    {
        private readonly int accNo;
        private ComboBox cboStatus;

        public UpdateAccountStatusForm(int accNo)
        {
            this.accNo = accNo;
            DialogHelper.StyleDialog(this, $"Change Status — Account #{accNo}", 420, 180);
            DialogHelper.AddHeader(this, "Update Account Status");

            int y = 56;
            cboStatus = DialogHelper.AddComboField(this, "New Status", y, new[] { "Active", "Inactive", "Closed" });
            y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                DbHelper.ExecuteProcedure("sp_UpdateAccountStatus",
                    DbHelper.P("p_AccNo", accNo),
                    DbHelper.P("p_Status", cboStatus.SelectedItem.ToString()));

                MessageBox.Show("Account status updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
