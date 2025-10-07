namespace ClientApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBoxCategories;
        private System.Windows.Forms.CheckBox chkNews;
        private System.Windows.Forms.CheckBox chkAnnouncements;
        private System.Windows.Forms.CheckBox chkTechnical;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.RichTextBox txtMessages;
        private System.Windows.Forms.Label lblEmergency;

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

       
        private void InitializeComponent()
        {
            this.groupBoxCategories = new System.Windows.Forms.GroupBox();
            this.chkNews = new System.Windows.Forms.CheckBox();
            this.chkAnnouncements = new System.Windows.Forms.CheckBox();
            this.chkTechnical = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtMessages = new System.Windows.Forms.RichTextBox();
            this.lblEmergency = new System.Windows.Forms.Label();
            this.groupBoxCategories.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCategories
            // 
            this.groupBoxCategories.Controls.Add(this.chkTechnical);
            this.groupBoxCategories.Controls.Add(this.chkAnnouncements);
            this.groupBoxCategories.Controls.Add(this.chkNews);
            this.groupBoxCategories.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCategories.Name = "groupBoxCategories";
            this.groupBoxCategories.Size = new System.Drawing.Size(200, 110);
            this.groupBoxCategories.TabIndex = 0;
            this.groupBoxCategories.TabStop = false;
            this.groupBoxCategories.Text = "Категорії повідомлень";
            // 
            // chkNews
            // 
            this.chkNews.AutoSize = true;
            this.chkNews.Location = new System.Drawing.Point(12, 22);
            this.chkNews.Name = "chkNews";
            this.chkNews.Size = new System.Drawing.Size(67, 19);
            this.chkNews.TabIndex = 0;
            this.chkNews.Text = "Новини";
            this.chkNews.UseVisualStyleBackColor = true;
            // 
            // chkAnnouncements
            // 
            this.chkAnnouncements.AutoSize = true;
            this.chkAnnouncements.Location = new System.Drawing.Point(12, 48);
            this.chkAnnouncements.Name = "chkAnnouncements";
            this.chkAnnouncements.Size = new System.Drawing.Size(90, 19);
            this.chkAnnouncements.TabIndex = 1;
            this.chkAnnouncements.Text = "Оголошення";
            this.chkAnnouncements.UseVisualStyleBackColor = true;
            // 
            // chkTechnical
            // 
            this.chkTechnical.AutoSize = true;
            this.chkTechnical.Location = new System.Drawing.Point(12, 74);
            this.chkTechnical.Name = "chkTechnical";
            this.chkTechnical.Size = new System.Drawing.Size(83, 19);
            this.chkTechnical.TabIndex = 2;
            this.chkTechnical.Text = "Технічні";
            this.chkTechnical.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(230, 35);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(120, 30);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Підключитися";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(230, 80);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(330, 23);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Статус";
            // 
            // txtMessages
            // 
            this.txtMessages.Location = new System.Drawing.Point(12, 130);
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.Size = new System.Drawing.Size(548, 280);
            this.txtMessages.TabIndex = 5;
            this.txtMessages.Text = "";
            // 
            // lblEmergency
            // 
            this.lblEmergency.AutoSize = true;
            this.lblEmergency.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblEmergency.Location = new System.Drawing.Point(12, 423);
            this.lblEmergency.Name = "lblEmergency";
            this.lblEmergency.Size = new System.Drawing.Size(210, 20);
            this.lblEmergency.TabIndex = 6;
            this.lblEmergency.Text = "⚠ ЕКСТРЕННЕ ПОВІДОМЛЕННЯ!";
            this.lblEmergency.Visible = false;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(572, 460);
            this.Controls.Add(this.lblEmergency);
            this.Controls.Add(this.txtMessages);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.groupBoxCategories);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "ClientApp — Повідомлення компанії";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBoxCategories.ResumeLayout(false);
            this.groupBoxCategories.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
