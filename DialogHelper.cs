using System.Drawing;
using System.Windows.Forms;

namespace dbProject.Forms
{
    /// <summary>Lays out a label + input control pair on a form, one row at a time.</summary>
    public static class DialogHelper
    {
        public const int RowHeight = 46;
        public const int LabelWidth = 150;
        public const int FieldWidth = 260;
        public const int LeftMargin = 24;

        public static TextBox AddTextField(Control host, string label, int y, string placeholder = null)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(LeftMargin, y + 6),
                Size = new Size(LabelWidth, 22),
                Font = UiTheme.FontBody,
                ForeColor = UiTheme.TextDark
            };
            var tb = new TextBox
            {
                Location = new Point(LeftMargin + LabelWidth, y),
                Size = new Size(FieldWidth, 26),
                Font = UiTheme.FontBody
            };
            host.Controls.Add(lbl);
            host.Controls.Add(tb);
            return tb;
        }

        public static ComboBox AddComboField(Control host, string label, int y, string[] options)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(LeftMargin, y + 6),
                Size = new Size(LabelWidth, 22),
                Font = UiTheme.FontBody,
                ForeColor = UiTheme.TextDark
            };
            var cb = new ComboBox
            {
                Location = new Point(LeftMargin + LabelWidth, y),
                Size = new Size(FieldWidth, 26),
                Font = UiTheme.FontBody,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cb.Items.AddRange(options);
            if (options.Length > 0) cb.SelectedIndex = 0;
            host.Controls.Add(lbl);
            host.Controls.Add(cb);
            return cb;
        }

        public static DateTimePicker AddDateField(Control host, string label, int y)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(LeftMargin, y + 6),
                Size = new Size(LabelWidth, 22),
                Font = UiTheme.FontBody,
                ForeColor = UiTheme.TextDark
            };
            var dp = new DateTimePicker
            {
                Location = new Point(LeftMargin + LabelWidth, y),
                Size = new Size(FieldWidth, 26),
                Font = UiTheme.FontBody,
                Format = DateTimePickerFormat.Short
            };
            host.Controls.Add(lbl);
            host.Controls.Add(dp);
            return dp;
        }

        public static Label AddHeader(Control host, string text)
        {
            var lbl = new Label
            {
                Text = text,
                Font = UiTheme.FontHeading,
                ForeColor = UiTheme.TextDark,
                Location = new Point(LeftMargin, 16),
                AutoSize = true
            };
            host.Controls.Add(lbl);
            return lbl;
        }

        public static void AddButtons(Form host, int y, out Button save, out Button cancel)
        {
            save = UiTheme.MakePrimaryButton("Save");
            save.Location = new Point(LeftMargin + LabelWidth + FieldWidth - 210, y);
            save.Width = 100;
            save.DialogResult = DialogResult.None;

            cancel = UiTheme.MakeSecondaryButton("Cancel");
            cancel.Location = new Point(LeftMargin + LabelWidth + FieldWidth - 100, y);
            cancel.Width = 100;
            cancel.DialogResult = DialogResult.Cancel;

            host.Controls.Add(save);
            host.Controls.Add(cancel);
            host.AcceptButton = save;
            host.CancelButton = cancel;
        }

        public static void StyleDialog(Form f, string title, int width, int height)
        {
            f.Text = title;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.StartPosition = FormStartPosition.CenterParent;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            f.ClientSize = new Size(width, height);
            f.BackColor = Color.White;
            f.Font = UiTheme.FontBody;
        }
    }
}
