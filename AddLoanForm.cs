using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class AddLoanForm : Form
    {
        private TextBox txtLoanId, txtAmount, txtRate, txtTerm, txtCusId, txtBranchId;
        private ComboBox cboType;
        private Panel pnlExtra;
        private TextBox txtExtra1, txtExtra2, txtExtra3; // reused for type-specific fields

        public AddLoanForm()
        {
            DialogHelper.StyleDialog(this, "Add New Loan", 460, 470);
            DialogHelper.AddHeader(this, "Loan Details");

            int y = 56;
            txtLoanId = DialogHelper.AddTextField(this, "Loan ID", y);
            txtLoanId.Text = DbHelper.NextId("Loan", "LoanID").ToString();
            y += DialogHelper.RowHeight;

            txtAmount = DialogHelper.AddTextField(this, "Amount", y); y += DialogHelper.RowHeight;
            txtRate = DialogHelper.AddTextField(this, "Interest Rate (e.g. 0.05)", y); y += DialogHelper.RowHeight;
            txtTerm = DialogHelper.AddTextField(this, "Term (months)", y); y += DialogHelper.RowHeight;
            cboType = DialogHelper.AddComboField(this, "Loan Type", y, new[] { "Mortgage", "CarLoan", "PersonalLoan" });
            y += DialogHelper.RowHeight;
            txtCusId = DialogHelper.AddTextField(this, "Customer ID", y); y += DialogHelper.RowHeight;
            txtBranchId = DialogHelper.AddTextField(this, "Branch ID", y); y += DialogHelper.RowHeight;

            pnlExtra = new Panel { Location = new System.Drawing.Point(0, y), Size = new System.Drawing.Size(460, 50) };
            Controls.Add(pnlExtra);
            cboType.SelectedIndexChanged += (s, e) => BuildExtraFields();
            BuildExtraFields();
            y += 56;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Click += Save_Click;
        }

        private void BuildExtraFields()
        {
            pnlExtra.Controls.Clear();
            switch (cboType.SelectedItem.ToString())
            {
                case "Mortgage":
                    txtExtra1 = DialogHelper.AddTextField(pnlExtra, "Property Address", 0);
                    txtExtra2 = null; txtExtra3 = null;
                    break;
                case "CarLoan":
                    txtExtra1 = DialogHelper.AddTextField(pnlExtra, "VIN", 0);
                    txtExtra2 = DialogHelper.AddTextField(pnlExtra, "Make / Model", 46);
                    pnlExtra.Height = 92;
                    txtExtra3 = null;
                    break;
                case "PersonalLoan":
                    txtExtra1 = DialogHelper.AddTextField(pnlExtra, "Purpose", 0);
                    txtExtra2 = null; txtExtra3 = null;
                    break;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLoanId.Text, out int loanId)) { MessageBox.Show("Loan ID must be a number."); return; }
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0) { MessageBox.Show("Enter a valid positive amount."); return; }
            if (!decimal.TryParse(txtRate.Text, out decimal rate)) { MessageBox.Show("Enter a valid interest rate."); return; }
            if (!int.TryParse(txtTerm.Text, out int term)) { MessageBox.Show("Term must be a number of months."); return; }
            if (!int.TryParse(txtCusId.Text, out int cusId)) { MessageBox.Show("Customer ID must be a number."); return; }
            if (!int.TryParse(txtBranchId.Text, out int branchId)) { MessageBox.Show("Branch ID must be a number."); return; }

            string loanType = cboType.SelectedItem.ToString();

            try
            {
                DbHelper.ExecuteProcedure("sp_AddLoan",
                    DbHelper.P("p_LoanID", loanId),
                    DbHelper.P("p_Amount", amount),
                    DbHelper.P("p_Rate", rate),
                    DbHelper.P("p_Term", term),
                    DbHelper.P("p_LoanType", loanType),
                    DbHelper.P("p_CusID", cusId),
                    DbHelper.P("p_BranchID", branchId));

                if (!DbHelper.RowExists("Loan", "LoanID", loanId))
                {
                    MessageBox.Show("Could not add loan — check that the Customer ID and Branch ID exist.",
                        "Not Added", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Add type-specific detail row (base procedure only inserts into Loan itself).
                if (loanType == "Mortgage" && txtExtra1 != null && !string.IsNullOrWhiteSpace(txtExtra1.Text))
                {
                    DbHelper.ExecuteNonQuery(
                        "INSERT INTO Mortgage (LoanID, PropertyAddress) VALUES (:id, :addr)",
                        DbHelper.P("id", loanId), DbHelper.P("addr", txtExtra1.Text.Trim()));
                }
                else if (loanType == "CarLoan" && txtExtra1 != null)
                {
                    string vin = txtExtra1.Text.Trim();
                    string makeModel = txtExtra2?.Text.Trim() ?? "";
                    var parts = makeModel.Split(new[] { ' ' }, 2);
                    string make = parts.Length > 0 ? parts[0] : "";
                    string model = parts.Length > 1 ? parts[1] : "";
                    DbHelper.ExecuteNonQuery(
                        "INSERT INTO Car_Loan (LoanID, VIN, VehicleMake, Model) VALUES (:id, :vin, :make, :model)",
                        DbHelper.P("id", loanId), DbHelper.P("vin", vin), DbHelper.P("make", make), DbHelper.P("model", model));
                }
                else if (loanType == "PersonalLoan" && txtExtra1 != null && !string.IsNullOrWhiteSpace(txtExtra1.Text))
                {
                    DbHelper.ExecuteNonQuery(
                        "INSERT INTO Personal_Loan (LoanID, Purpose) VALUES (:id, :purpose)",
                        DbHelper.P("id", loanId), DbHelper.P("purpose", txtExtra1.Text.Trim()));
                }

                MessageBox.Show("Loan added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
