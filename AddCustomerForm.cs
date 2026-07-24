using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class AddCustomerForm : Form
    {
        private TextBox txtCusId, txtSsn, txtFName, txtLName, txtEmail, txtPhone, txtAddress;
        private DateTimePicker dtDob;

        public AddCustomerForm()
        {
            DialogHelper.StyleDialog(this, "Add New Customer", 460, 430);
            DialogHelper.AddHeader(this, "Customer Details");

            int y = 56;
            txtCusId = DialogHelper.AddTextField(this, "Customer ID", y);
            txtCusId.Text = DbHelper.NextId("Customer", "CusID").ToString();
            y += DialogHelper.RowHeight;

            txtSsn = DialogHelper.AddTextField(this, "SSN", y); y += DialogHelper.RowHeight;
            txtFName = DialogHelper.AddTextField(this, "First Name", y); y += DialogHelper.RowHeight;
            txtLName = DialogHelper.AddTextField(this, "Last Name", y); y += DialogHelper.RowHeight;
            dtDob = DialogHelper.AddDateField(this, "Date of Birth", y); y += DialogHelper.RowHeight;
            txtEmail = DialogHelper.AddTextField(this, "Email", y); y += DialogHelper.RowHeight;
            txtPhone = DialogHelper.AddTextField(this, "Phone No", y); y += DialogHelper.RowHeight;
            txtAddress = DialogHelper.AddTextField(this, "Address", y); y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCusId.Text, out int cusId))
            {
                MessageBox.Show("Customer ID must be a number."); return;
            }
            if (string.IsNullOrWhiteSpace(txtFName.Text) || string.IsNullOrWhiteSpace(txtLName.Text) ||
                string.IsNullOrWhiteSpace(txtSsn.Text))
            {
                MessageBox.Show("SSN, First Name and Last Name are required."); return;
            }

            try
            {
                DbHelper.ExecuteProcedure("sp_AddCustomer",
                    DbHelper.P("p_CusID", cusId),
                    DbHelper.P("p_SSN", txtSsn.Text.Trim()),
                    DbHelper.P("p_FName", txtFName.Text.Trim()),
                    DbHelper.P("p_LName", txtLName.Text.Trim()),
                    DbHelper.P("p_DOB", dtDob.Value.Date),
                    DbHelper.P("p_Email", txtEmail.Text.Trim()),
                    DbHelper.P("p_PhoneNo", txtPhone.Text.Trim()),
                    DbHelper.P("p_Address", txtAddress.Text.Trim()));

                if (DbHelper.RowExists("Customer", "CusID", cusId))
                {
                    MessageBox.Show("Customer added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Could not add customer — the Customer ID or SSN may already exist.",
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
