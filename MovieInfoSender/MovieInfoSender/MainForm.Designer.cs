using System.Windows.Forms;
using System.Drawing;

namespace MovieInfoSender
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtTitle;
        private Button btnSearch;
        private Label lblStatus;
        private PictureBox picPoster;
        private Label lblTitle;
        private Label lblYear;
        private Label lblGenre;
        private Label lblDirector;
        private Label lblActors;
        private TextBox txtPlot;
        private Button btnSendEmail;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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

            txtTitle = new TextBox();
            btnSearch = new Button();
            lblStatus = new Label();
            picPoster = new PictureBox();
            lblTitle = new Label();
            lblYear = new Label();
            lblGenre = new Label();
            lblDirector = new Label();
            lblActors = new Label();
            txtPlot = new TextBox();
            btnSendEmail = new Button();

            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(20, 20);
            txtTitle.Size = new Size(300, 23);
            txtTitle.Name = "txtTitle";

            // 
            // btnSearch
            // 
            btnSearch.Text = "Пошук";
            btnSearch.Location = new Point(330, 20);
            btnSearch.Size = new Size(100, 25);
            btnSearch.Name = "btnSearch";
            btnSearch.Click += btnSearch_Click;

            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(20, 50);
            lblStatus.AutoSize = true;
            lblStatus.Name = "lblStatus";
            lblStatus.Text = "Готово";

            // 
            // picPoster
            // 
            picPoster.Location = new Point(20, 80);
            picPoster.Size = new Size(180, 260);
            picPoster.SizeMode = PictureBoxSizeMode.StretchImage;
            picPoster.Name = "picPoster";
            picPoster.BorderStyle = BorderStyle.FixedSingle;

            // 
            // lblTitle
            // 
            lblTitle.Location = new Point(220, 80);
            lblTitle.AutoSize = true;
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Назва:";

            // 
            // lblYear
            // 
            lblYear.Location = new Point(220, 110);
            lblYear.AutoSize = true;
            lblYear.Name = "lblYear";
            lblYear.Text = "Рік:";

            // 
            // lblGenre
            // 
            lblGenre.Location = new Point(220, 140);
            lblGenre.AutoSize = true;
            lblGenre.Name = "lblGenre";
            lblGenre.Text = "Жанр:";

            // 
            // lblDirector
            // 
            lblDirector.Location = new Point(220, 170);
            lblDirector.AutoSize = true;
            lblDirector.Name = "lblDirector";
            lblDirector.Text = "Режисер:";

            // 
            // lblActors
            // 
            lblActors.Location = new Point(220, 200);
            lblActors.AutoSize = true;
            lblActors.Name = "lblActors";
            lblActors.Text = "Актори:";

            // 
            // txtPlot
            // 
            txtPlot.Location = new Point(220, 230);
            txtPlot.Size = new Size(350, 110);
            txtPlot.Multiline = true;
            txtPlot.ReadOnly = true;
            txtPlot.ScrollBars = ScrollBars.Vertical;
            txtPlot.Name = "txtPlot";

            // 
            // btnSendEmail
            // 
            btnSendEmail.Text = "Надіслати на пошту";
            btnSendEmail.Location = new Point(20, 360);
            btnSendEmail.Size = new Size(150, 30);
            btnSendEmail.Name = "btnSendEmail";
            btnSendEmail.Click += btnSendEmail_Click;

            // 
            // MainForm
            // 
            this.Text = "Movie Info Sender";
            this.ClientSize = new Size(600, 420);
            this.Controls.AddRange(new Control[]
            {
                txtTitle, btnSearch, lblStatus, picPoster, lblTitle,
                lblYear, lblGenre, lblDirector, lblActors, txtPlot, btnSendEmail
            });
        }
    }
}
