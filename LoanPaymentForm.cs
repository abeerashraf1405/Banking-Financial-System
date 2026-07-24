using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class LoanPaymentForm : Form
    {
        private readonly int loanId;
        private TextBox txtAmount;
        private Label lblCurrBal;

        public LoanPaymentForm(int loanId)
        {
            this.loanId = loanId;
            DialogHelper.StyleDialog(this, $"Record Payment — Loan #{loanId}", 420, 230);
            DialogHelper.AddHeader(this, "Loan Payment");

            int y = 56;
            lblCurrBal = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(24, y),
                Size = new System.Drawing.Size(370, 22),
                Font = UiTheme.FontBody,
                ForeColor = UiTheme.TextMuted
            };
            Controls.Add(lblCurrBal);
            y += DialogHelper.RowHeight;

            txtAmount = DialogHelper.AddTextField(this, "Payment Amount", y); y += DialogHelper.RowHeight + 10;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Text = "Pay";
            save.Click += Save_Click;

            LoadCurrentBalance();
        }

        private void LoadCurrentBalance()
        {
            try
            {
                var dt = DbHelper.ExecuteQuery("SELECT CurrBal FROM Loan WHERE LoanID = :id", DbHelper.P("id", loanId));
                lblCurrBal.Text = dt.Rows.Count > 0
                    ? $"Current balance: {Convert.ToDecimal(dt.Rows[0]["CURRBAL"]):N2}"
                    : "Loan not found.";
            }
            catch { /* non-critical */ }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Enter a valid positive payment amount."); return;
            }

            try
            {
                DbHelper.ExecuteProcedure("sp_UpdateLoanBalance",
                    DbHelper.P("p_LoanID", loanId),
                    DbHelper.P("p_PaymentAmount", amount));

                MessageBox.Show("Payment recorded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
