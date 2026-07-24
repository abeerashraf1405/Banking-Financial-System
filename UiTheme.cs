using System.Drawing;
using System.Windows.Forms;

namespace dbProject
{
    /// <summary>Central place for colors/fonts so every screen looks consistent.</summary>
    public static class UiTheme
    {
        public static readonly Color NavyDark = Color.FromArgb(18, 32, 58);      // sidebar
        public static readonly Color NavyMid = Color.FromArgb(27, 46, 82);       // sidebar hover
        public static readonly Color Gold = Color.FromArgb(212, 160, 23);        // accent
        public static readonly Color BgLight = Color.FromArgb(244, 246, 250);    // content background
        public static readonly Color CardWhite = Color.White;
        public static readonly Color TextDark = Color.FromArgb(33, 37, 41);
        public static readonly Color TextMuted = Color.FromArgb(108, 117, 125);
        public static readonly Color Success = Color.FromArgb(40, 167, 69);
        public static readonly Color Danger = Color.FromArgb(220, 53, 69);
        public static readonly Color Border = Color.FromArgb(222, 226, 230);

        public static readonly Font FontTitle = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font FontHeading = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        public static readonly Font FontBody = new Font("Segoe UI", 10F);
        public static readonly Font FontBodyBold = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font FontSidebar = new Font("Segoe UI", 10.5F);

        public static Button MakeButton(string text, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = FontBodyBold,
                Height = 36,
                Width = 130,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.15f);
            return btn;
        }

        public static Button MakePrimaryButton(string text) => MakeButton(text, Gold, Color.White);
        public static Button MakeSecondaryButton(string text) => MakeButton(text, Color.FromArgb(233, 236, 239), TextDark);
        public static Button MakeDangerButton(string text) => MakeButton(text, Danger, Color.White);

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = CardWhite;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Font = FontBody;
            grid.RowTemplate.Height = 34;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = NavyDark;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = FontBodyBold;
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 244, 214);
            grid.DefaultCellStyle.SelectionForeColor = TextDark;
            grid.DefaultCellStyle.Padding = new Padding(4);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
        }

        public static Panel MakeCard()
        {
            var p = new Panel
            {
                BackColor = CardWhite,
                Padding = new Padding(16)
            };
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            return p;
        }

        public static Label MakeStatCard(Panel host, string title, string value, Color accent)
        {
            host.BackColor = CardWhite;
            host.Margin = new Padding(10);

            var bar = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = accent };
            var lblTitle = new Label
            {
                Text = title,
                Font = FontBody,
                ForeColor = TextMuted,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 26,
                Padding = new Padding(16, 12, 0, 0)
            };
            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 12)
            };
            host.Controls.Add(lblValue);
            host.Controls.Add(lblTitle);
            host.Controls.Add(bar);
            return lblValue;
        }
    }
}
