using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovieInfoSender.MovieInfoSender;
using Org.BouncyCastle.Asn1.Cmp;

namespace MovieInfoSender
{
    public partial class SendEmailForm : Form
    {
        private readonly MovieInfo _movie;
        private readonly string _htmlPath;

        public SendEmailForm(MovieInfo movie, string htmlPath)
        {
            InitializeComponent();
            _movie = movie ?? throw new ArgumentNullException(nameof(movie));
            _htmlPath = htmlPath;
            txtFrom.Text = ""; // можна підставити значення за замовчуванням
            txtTo.Text = "";   // або залишити пустими
            txtSubject.Text = $"Інформація про фільм: {_movie.Title}";
            txtMessage.Text = string.Empty;
            lblStatus.Text = "Готово";
            chkAttach.Checked = true;
            txtPassword.UseSystemPasswordChar = true; // маскування паролю
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            // Простий валідаційний блок
            if (string.IsNullOrWhiteSpace(txtFrom.Text))
            {
                MessageBox.Show("Вкажіть адресу відправника (From).");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Вкажіть пароль від поштової скриньки (або app password).");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTo.Text))
            {
                MessageBox.Show("Вкажіть адресу одержувача (To).");
                return;
            }

            btnSend.Enabled = false;
            lblStatus.Text = "Відправлення...";

            try
            {
                string from = txtFrom.Text.Trim();
                string password = txtPassword.Text;
                string to = txtTo.Text.Trim();
                string subject = string.IsNullOrWhiteSpace(txtSubject.Text) ? $"Інформація: {_movie.Title}" : txtSubject.Text.Trim();
                string userMessage = txtMessage.Text ?? string.Empty;

                // Згенерувати HTML з даними фільму
                string htmlBody = HtmlFormatter.GenerateMovieHtml(_movie);

                // Якщо користувач хоче додати текст — додаємо під HTML
                if (!string.IsNullOrWhiteSpace(userMessage))
                {
                    htmlBody += $"<hr/><p>{System.Net.WebUtility.HtmlEncode(userMessage).Replace("\n", "<br/>")}</p>";
                }

                string? attachment = chkAttach.Checked && File.Exists(_htmlPath) ? _htmlPath : null;

                // Якщо потрібно використовувати інший SMTP сервер (не Gmail),
                // можна додати текстове поле для хоста/порта в designer і передавати сюди.
                await EmailService.SendEmailAsync(from, password, to, subject, htmlBody, attachment);

                lblStatus.Text = "Лист надіслано!";
                MessageBox.Show("Лист успішно відправлено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Помилка при відправці";
                MessageBox.Show($"Помилка при відправленні листа:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSend.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Додаткова зручність: показ / приховати пароль
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
    }
}
