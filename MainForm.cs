using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using dbProject.Forms;

namespace dbProject
{
    public class MainForm : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblPageTitle;
        private Button activeNavButton;

        public MainForm()
        {
            Text = "Prime Bank — Management System";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            Font = UiTheme.FontBody;
            BackColor = UiTheme.BgLight;
            MinimumSize = new Size(1100, 650);

            BuildShell();
            ShowDashboard();
        }

        // ---------------------------------------------------------------
        // SHELL: sidebar + header + content area
        // ---------------------------------------------------------------
        private void BuildShell()
        {
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = UiTheme.NavyDark };

            var lblLogo = new Label
            {
                Text = "🏦  PRIME BANK",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlSidebar.Controls.Add(lblLogo);

            var navPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Top = 70
            };
            navPanel.Location = new Point(0, 70);
            navPanel.Width = pnlSidebar.Width;

            AddNavButton(navPanel, "📊  Dashboard", ShowDashboard);
            AddNavButton(navPanel, "👤  Customers", ShowCustomers);
            AddNavButton(navPanel, "💳  Accounts", ShowAccounts);
            AddNavButton(navPanel, "🔁  Transactions", ShowTransactions);
            AddNavButton(navPanel, "💰  Loans", ShowLoans);
            AddNavButton(navPanel, "🪪  Cards", ShowCards);
            AddNavButton(navPanel, "🧑‍💼  Employees", ShowEmployees);
            AddNavButton(navPanel, "🏢  Branches", ShowBranches);

            pnlSidebar.Controls.Add(navPanel);
            pnlSidebar.Controls.SetChildIndex(navPanel, 0);

            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White };
            lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = UiTheme.FontTitle,
                ForeColor = UiTheme.TextDark,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0)
            };
            pnlHeader.Controls.Add(lblPageTitle);
            var headerBorder = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = UiTheme.Border };
            pnlHeader.Controls.Add(headerBorder);

            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.BgLight, Padding = new Padding(24) };

            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);
        }

        private void AddNavButton(FlowLayoutPanel host, string text, Action onClick)
        {
            var btn = new Button
            {
                Text = "   " + text,
                Width = pnlSidebar.Width,
                Height = 46,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.NavyDark,
                ForeColor = Color.Gainsboro,
                Font = UiTheme.FontSidebar,
                Cursor = Cursors.Hand,
                Tag = onClick
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = UiTheme.NavyMid;
            btn.Click += (s, e) =>
            {
                SetActiveNav(btn);
                onClick();
            };
            host.Controls.Add(btn);
        }

        private void SetActiveNav(Button btn)
        {
            if (activeNavButton != null)
            {
                activeNavButton.BackColor = UiTheme.NavyDark;
                activeNavButton.ForeColor = Color.Gainsboro;
            }
            btn.BackColor = UiTheme.Gold;
            btn.ForeColor = Color.White;
            activeNavButton = btn;
        }

        // ---------------------------------------------------------------
        // Generic list-page builder used by every module below
        // ---------------------------------------------------------------
        private DataGridView BuildListPage(string title, string sql, Action<FlowLayoutPanel, DataGridView> toolbarBuilder)
        {
            lblPageTitle.Text = title;
            pnlContent.Controls.Clear();

            var card = UiTheme.MakeCard();
            card.Dock = DockStyle.Fill;

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 0, 0, 10)
            };

            var grid = new DataGridView { Dock = DockStyle.Fill };
            UiTheme.StyleGrid(grid);

            void Refresh()
            {
                try { grid.DataSource = DbHelper.ExecuteQuery(sql); }
                catch (Exception ex) { MessageBox.Show("Load failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }

            var btnRefresh = UiTheme.MakeSecondaryButton("⟳ Refresh");
            btnRefresh.Width = 110;
            btnRefresh.Click += (s, e) => Refresh();
            toolbar.Controls.Add(btnRefresh);

            toolbarBuilder?.Invoke(toolbar, grid);

            card.Controls.Add(grid);
            card.Controls.Add(toolbar);
            pnlContent.Controls.Add(card);

            Refresh();
            // expose refresh via Tag so callers (add dialogs) can trigger it after insert
            grid.Tag = (Action)Refresh;
            return grid;
        }

        private void RefreshGrid(DataGridView grid) => ((Action)grid.Tag)?.Invoke();

        private object SelectedValue(DataGridView grid, string column)
        {
            if (grid.CurrentRow == null) return null;
            return grid.CurrentRow.Cells[column]?.Value;
        }

        // ---------------------------------------------------------------
        // DASHBOARD
        // ---------------------------------------------------------------
        private void ShowDashboard()
        {
            lblPageTitle.Text = "Dashboard";
            pnlContent.Controls.Clear();

            var statsRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                ColumnCount = 4,
                RowCount = 1
            };
            for (int i = 0; i < 4; i++)
                statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            var c1 = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 12, 0) };
            var c2 = new Panel { Dock = DockStyle.Fill, Margin = new Padding(12, 0, 12, 0) };
            var c3 = new Panel { Dock = DockStyle.Fill, Margin = new Padding(12, 0, 12, 0) };
            var c4 = new Panel { Dock = DockStyle.Fill, Margin = new Padding(12, 0, 0, 0) };
            statsRow.Controls.Add(c1, 0, 0);
            statsRow.Controls.Add(c2, 1, 0);
            statsRow.Controls.Add(c3, 2, 0);
            statsRow.Controls.Add(c4, 3, 0);

            var lblCustomers = UiTheme.MakeStatCard(c1, "TOTAL CUSTOMERS", "—", UiTheme.Gold);
            var lblBalance = UiTheme.MakeStatCard(c2, "TOTAL DEPOSITS", "—", UiTheme.Success);
            var lblLoans = UiTheme.MakeStatCard(c3, "ACTIVE LOANS", "—", Color.FromArgb(220, 53, 69));
            var lblCards = UiTheme.MakeStatCard(c4, "ACTIVE CARDS", "—", Color.FromArgb(0, 123, 255));

            var recentCard = UiTheme.MakeCard();
            recentCard.Dock = DockStyle.Fill;
            recentCard.Margin = new Padding(0, 16, 0, 0);
            var lblRecent = new Label { Text = "Recent Transactions", Font = UiTheme.FontHeading, Dock = DockStyle.Top, Height = 34 };
            var grid = new DataGridView { Dock = DockStyle.Fill };
            UiTheme.StyleGrid(grid);
            recentCard.Controls.Add(grid);
            recentCard.Controls.Add(lblRecent);

            var wrapper = new Panel { Dock = DockStyle.Fill };
            wrapper.Controls.Add(recentCard);
            wrapper.Controls.Add(statsRow);

            pnlContent.Controls.Add(wrapper);

            try
            {
                var dt1 = DbHelper.ExecuteQuery("SELECT COUNT(*) CNT FROM Customer");
                lblCustomers.Text = dt1.Rows[0]["CNT"].ToString();

                var dt2 = DbHelper.ExecuteQuery("SELECT NVL(SUM(Balance),0) TOTAL FROM Account WHERE Status='Active'");
                lblBalance.Text = "$" + Convert.ToDecimal(dt2.Rows[0]["TOTAL"]).ToString("N0");

                var dt3 = DbHelper.ExecuteQuery("SELECT COUNT(*) CNT FROM Loan WHERE Status='Active'");
                lblLoans.Text = dt3.Rows[0]["CNT"].ToString();

                var dt4 = DbHelper.ExecuteQuery("SELECT COUNT(*) CNT FROM Card WHERE Status='Active'");
                lblCards.Text = dt4.Rows[0]["CNT"].ToString();

                grid.DataSource = DbHelper.ExecuteQuery(
                    "SELECT * FROM vw_Transaction_History ORDER BY Trans_DateTime DESC FETCH FIRST 10 ROWS ONLY");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard load failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------------
        // CUSTOMERS
        // ---------------------------------------------------------------
        private void ShowCustomers()
        {
            BuildListPage("Customers", "SELECT * FROM Customer ORDER BY CusID", (toolbar, grid) =>
            {
                var btnAdd = UiTheme.MakePrimaryButton("+ Add Customer");
                btnAdd.Width = 150;
                btnAdd.Click += (s, e) =>
                {
                    using (var f = new AddCustomerForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };
                toolbar.Controls.Add(btnAdd);
            });
        }

        // ---------------------------------------------------------------
        // ACCOUNTS
        // ---------------------------------------------------------------
        private void ShowAccounts()
        {
            BuildListPage("Accounts", "SELECT * FROM vw_Customer_Accounts ORDER BY AccNo", (toolbar, grid) =>
            {
                var btnAdd = UiTheme.MakePrimaryButton("+ New Account");
                btnAdd.Width = 150;
                btnAdd.Click += (s, e) =>
                {
                    using (var f = new AddAccountForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                var btnStatus = UiTheme.MakeSecondaryButton("Change Status");
                btnStatus.Width = 140;
                btnStatus.Click += (s, e) =>
                {
                    var accNo = SelectedValue(grid, "ACCNO");
                    if (accNo == null) { MessageBox.Show("Select an account row first."); return; }
                    using (var f = new UpdateAccountStatusForm(Convert.ToInt32(accNo)))
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                toolbar.Controls.Add(btnAdd);
                toolbar.Controls.Add(btnStatus);
            });
        }

        // ---------------------------------------------------------------
        // TRANSACTIONS
        // ---------------------------------------------------------------
        private void ShowTransactions()
        {
            BuildListPage("Transactions",
                "SELECT * FROM vw_Transaction_History ORDER BY Trans_DateTime DESC", (toolbar, grid) =>
            {
                var btnDeposit = UiTheme.MakePrimaryButton("+ Deposit");
                btnDeposit.Width = 110;
                btnDeposit.Click += (s, e) =>
                {
                    using (var f = new DepositForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                var btnWithdraw = UiTheme.MakeButton("− Withdraw", Color.FromArgb(0, 123, 255), Color.White);
                btnWithdraw.Width = 120;
                btnWithdraw.Click += (s, e) =>
                {
                    using (var f = new WithdrawalForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                var btnTransfer = UiTheme.MakeButton("⇄ Transfer", Color.FromArgb(108, 92, 231), Color.White);
                btnTransfer.Width = 120;
                btnTransfer.Click += (s, e) =>
                {
                    using (var f = new TransferForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                toolbar.Controls.Add(btnDeposit);
                toolbar.Controls.Add(btnWithdraw);
                toolbar.Controls.Add(btnTransfer);
            });
        }

        // ---------------------------------------------------------------
        // LOANS
        // ---------------------------------------------------------------
        private void ShowLoans()
        {
            BuildListPage("Loans", "SELECT * FROM vw_Active_Loans ORDER BY LoanID", (toolbar, grid) =>
            {
                var btnAdd = UiTheme.MakePrimaryButton("+ New Loan");
                btnAdd.Width = 130;
                btnAdd.Click += (s, e) =>
                {
                    using (var f = new AddLoanForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                var btnPay = UiTheme.MakeSecondaryButton("Record Payment");
                btnPay.Width = 150;
                btnPay.Click += (s, e) =>
                {
                    var loanId = SelectedValue(grid, "LOANID");
                    if (loanId == null) { MessageBox.Show("Select a loan row first."); return; }
                    using (var f = new LoanPaymentForm(Convert.ToInt32(loanId)))
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };

                toolbar.Controls.Add(btnAdd);
                toolbar.Controls.Add(btnPay);
            });
        }

        // ---------------------------------------------------------------
        // CARDS
        // ---------------------------------------------------------------
        private void ShowCards()
        {
            BuildListPage("Cards", "SELECT * FROM vw_Card_Holders ORDER BY CardNo", (toolbar, grid) =>
            {
                var btnAdd = UiTheme.MakePrimaryButton("+ Issue Card");
                btnAdd.Width = 130;
                btnAdd.Click += (s, e) =>
                {
                    using (var f = new AddCardForm())
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };
                toolbar.Controls.Add(btnAdd);
            });
        }

        // ---------------------------------------------------------------
        // EMPLOYEES
        // ---------------------------------------------------------------
        private void ShowEmployees()
        {
            BuildListPage("Employees", "SELECT * FROM vw_Employee_Branch ORDER BY EmpID", (toolbar, grid) =>
            {
                var btnSalary = UiTheme.MakeSecondaryButton("Update Salary");
                btnSalary.Width = 140;
                btnSalary.Click += (s, e) =>
                {
                    var empId = SelectedValue(grid, "EMPID");
                    if (empId == null) { MessageBox.Show("Select an employee row first."); return; }
                    using (var f = new UpdateSalaryForm(Convert.ToInt32(empId)))
                        if (f.ShowDialog() == DialogResult.OK) RefreshGrid(grid);
                };
                toolbar.Controls.Add(btnSalary);
            });
        }

        // ---------------------------------------------------------------
        // BRANCHES
        // ---------------------------------------------------------------
        private void ShowBranches()
        {
            BuildListPage("Branches", "SELECT * FROM Bank_Branch ORDER BY BranchID", null);
        }
    }
}
