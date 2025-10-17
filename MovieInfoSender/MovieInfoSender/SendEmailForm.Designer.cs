using System.Drawing;
using System.Windows.Forms;

namespace MovieInfoSender
{
    partial class SendEmailForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtFrom;
        private TextBox txtPassword;
        private TextBox txtTo;
        private TextBox txtSubject;
        private TextBox txtMessage;
        private CheckBox chkAttach;
        private Button btnSend;
        private Button btnCancel;
        private Label lblStatus;
        private Label lblFrom;
        private Label lblPassword;
        private Label lblTo;
        private Label lblSubject;
        private Label lblMessage;
        private CheckBox chkShowPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            lblFrom = new Label();
            lblPassword = new Label();
            lblTo = new Label();
            lblSubject = new Label();
            lblMessage = new Label();

            txtFrom = new TextBox();
            txtPassword = new TextBox();
            txtTo = new TextBox();
            txtSubject = new TextBox();
            txtMessage = new TextBox();

            chkAttach = new CheckBox();
            chkShowPassword = new CheckBox();
            btnSend = new Button();
            btnCancel = new Button();
            lblStatus = new Label();

            // 
            // Labels
            // 
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(12, 15);
            lblFrom.Text = "From (ваша пошта):";
            lblFrom.Name = "lblFrom";

            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(12, 50);
            lblPassword.Text = "Пароль:";
            lblPassword.Name = "lblPassword";

            lblTo.AutoSize = true;
            lblTo.Location = new Point(12, 85);
            lblTo.Text = "To (одержувач):";
            lblTo.Name = "lblTo";

            lblSubject.AutoSize = true;
            lblSubject.Location = new Point(12, 120);
            lblSubject.Text = "Тема:";
            lblSubject.Name = "lblSubject";

            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(12, 155);
            lblMessage.Text = "Повідомлення (опціонально):";
            lblMessage.Name = "lblMessage";

            // 
            // txtFrom
            // 
            txtFrom.Location = new Point(140, 12);
            txtFrom.Size = new Size(340, 23);
            txtFrom.Name = "txtFrom";

            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(140, 47);
            txtPassword.Size = new Size(260, 23);
            txtPassword.Name = "txtPassword";
            txtPassword.UseSystemPasswordChar = true;

            // 
            // chkShowPassword
            // 
            chkShowPassword.Location = new Point(410, 47);
            chkShowPassword.Size = new Size(70, 24);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Text = "Показати";
            chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);

            // 
            // txtTo
            // 
            txtTo.Location = new Point(140, 82);
            txtTo.Size = new Size(340, 23);
            txtTo.Name = "txtTo";

            // 
            // txtSubject
            // 
            txtSubject.Location = new Point(140, 117);
            txtSubject.Size = new Size(340, 23);
            txtSubject.Name = "txtSubject";

            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(15, 180);
            txtMessage.Size = new Size(465, 120);
            txtMessage.Multiline = true;
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.Name = "txtMessage";

            // 
            // chkAttach
            // 
            chkAttach.Location = new Point(15, 310);
            chkAttach.Size = new Size(260, 24);
            chkAttach.Text = "Прикріпити HTML з результатом";
            chkAttach.Name = "chkAttach";
            chkAttach.Checked = true;

            // 
            // btnSend
            // 
            btnSend.Location = new Point(320, 345);
            btnSend.Size = new Size(75, 30);
            btnSend.Text = "Відправити";
            btnSend.Name = "btnSend";
            btnSend.Click += new System.EventHandler(this.btnSend_Click);

            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(405, 345);
            btnCancel.Size = new Size(75, 30);
            btnCancel.Text = "Скасувати";
            btnCancel.Name = "btnCancel";
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(12, 350);
            lblStatus.Size = new Size(290, 24);
            lblStatus.Text = "Готово";
            lblStatus.Name = "lblStatus";

            // 
            // SendEmailForm
            // 
            this.Text = "Відправити лист";
            this.ClientSize = new Size(500, 390);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            this.Controls.Add(lblFrom);
            this.Controls.Add(txtFrom);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(chkShowPassword);
            this.Controls.Add(lblTo);
            this.Controls.Add(txtTo);
            this.Controls.Add(lblSubject);
            this.Controls.Add(txtSubject);
            this.Controls.Add(lblMessage);
            this.Controls.Add(txtMessage);
            this.Controls.Add(chkAttach);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnCancel);
            this.Controls.Add(lblStatus);
        }
    }
}
