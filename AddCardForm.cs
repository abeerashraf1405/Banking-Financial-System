using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace dbProject.Forms
{
    public class AddCardForm : Form
    {
        private TextBox txtCardNo, txtCvv, txtCusId, txtAccNo;
        private ComboBox cboType;
        private DateTimePicker dtExp;
        private Panel pnlExtra;
        private TextBox txtExtra1, txtExtra2, txtExtra3;

        public AddCardForm()
        {
            DialogHelper.StyleDialog(this, "Issue New Card", 460, 480);
            DialogHelper.AddHeader(this, "Card Details");

            int y = 56;
            txtCardNo = DialogHelper.AddTextField(this, "Card Number", y); y += DialogHelper.RowHeight;
            txtCvv = DialogHelper.AddTextField(this, "CVV", y); y += DialogHelper.RowHeight;
            cboType = DialogHelper.AddComboField(this, "Card Type", y, new[] { "Credit", "Debit" });
            y += DialogHelper.RowHeight;
            dtExp = DialogHelper.AddDateField(this, "Expiry Date", y);
            dtExp.Value = DateTime.Today.AddYears(3);
            y += DialogHelper.RowHeight;
            txtCusId = DialogHelper.AddTextField(this, "Customer ID", y); y += DialogHelper.RowHeight;
            txtAccNo = DialogHelper.AddTextField(this, "Account No", y); y += DialogHelper.RowHeight;

            pnlExtra = new Panel { Location = new System.Drawing.Point(0, y), Size = new System.Drawing.Size(460, 92) };
            Controls.Add(pnlExtra);
            cboType.SelectedIndexChanged += (s, e) => BuildExtraFields();
            BuildExtraFields();
            y += 98;

            DialogHelper.AddButtons(this, y, out Button save, out Button cancel);
            save.Text = "Issue Card";
            save.Click += Save_Click;
        }

        private void BuildExtraFields()
        {
            pnlExtra.Controls.Clear();
            if (cboType.SelectedItem.ToString() == "Credit")
            {
                txtExtra1 = DialogHelper.AddTextField(pnlExtra, "Credit Limit", 0);
                txtExtra2 = DialogHelper.AddTextField(pnlExtra, "Min Payment", 46);
                txtExtra3 = null;
            }
            else
            {
                txtExtra1 = DialogHelper.AddTextField(pnlExtra, "PIN", 0);
                txtExtra2 = null; txtExtra3 = null;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCardNo.Text)) { MessageBox.Show("Enter a card number."); return; }
            if (string.IsNullOrWhiteSpace(txtCvv.Text)) { MessageBox.Show("Enter a CVV."); return; }
            if (!int.TryParse(txtCusId.Text, out int cusId)) { MessageBox.Show("Customer ID must be a number."); return; }
            if (!int.TryParse(txtAccNo.Text, out int accNo)) { MessageBox.Show("Account No must be a number."); return; }

            string cardNo = txtCardNo.Text.Trim();
            string type = cboType.SelectedItem.ToString();

            try
            {
                DbHelper.ExecuteNonQuery(
                    @"INSERT INTO Card (CardNo, CVV, Type, ExpDate, Status, CusID, AccNo)
                      VALUES (:cn, :cvv, :type, :exp, 'Active', :cus, :acc)",
                    DbHelper.P("cn", cardNo), DbHelper.P("cvv", txtCvv.Text.Trim()),
                    DbHelper.P("type", type), DbHelper.P("exp", dtExp.Value.Date),
                    DbHelper.P("cus", cusId), DbHelper.P("acc", accNo));

                if (type == "Credit")
                {
                    decimal.TryParse(txtExtra1?.Text, out decimal limit);
                    decimal.TryParse(txtExtra2?.Text, out decimal minPay);
                    DbHelper.ExecuteNonQuery(
                        "INSERT INTO Credit_Card (CardNo, CreditLimit, MinPayment, APR) VALUES (:cn, :lim, :min, :apr)",
                        DbHelper.P("cn", cardNo), DbHelper.P("lim", limit), DbHelper.P("min", minPay), DbHelper.P("apr", 0.1999));
                }
                else
                {
                    DbHelper.ExecuteNonQuery(
                        "INSERT INTO Debit_Card (CardNo, PIN) VALUES (:cn, :pin)",
                        DbHelper.P("cn", cardNo), DbHelper.P("pin", txtExtra1?.Text.Trim() ?? ""));
                }

                MessageBox.Show("Card issued successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
