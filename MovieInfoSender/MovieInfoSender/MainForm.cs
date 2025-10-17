using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovieInfoSender.MovieInfoSender;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace MovieInfoSender
{
    public partial class MainForm : Form
    {
        private readonly OmdbService _omdbService;
        private MovieInfo? currentMovie; // поточний знайдений фільм

        public MainForm()
        {
            InitializeComponent();
            _omdbService = new OmdbService("YOUR_OMDB_API_KEY");
        }

        private async Task SearchMovieAsync()
        {
            var title = txtTitle.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Будь ласка, введіть назву фільму!");
                return;
            }

            lblStatus.Text = "Пошук...";
            var movie = await _omdbService.GetMovieInfoAsync(title);

            if (movie == null)
            {
                lblStatus.Text = "Фільм не знайдено.";
                ClearMovieDisplay();
                return;
            }

            currentMovie = movie;
            DisplayMovieInfo(movie);
            lblStatus.Text = "Знайдено!";
        }

        private void DisplayMovieInfo(MovieInfo movie)
        {
            lblTitle.Text = $"Назва: {movie.Title}";
            lblYear.Text = $"Рік: {movie.Year}";
            lblGenre.Text = $"Жанр: {movie.Genre}";
            lblDirector.Text = $"Режисер: {movie.Director}";
            lblActors.Text = $"Актори: {movie.Actors}";
            txtPlot.Text = movie.Plot ?? string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(movie.Poster))
                    picPoster.Load(movie.Poster);
                else
                    picPoster.Image = null;
            }
            catch
            {
                picPoster.Image = null;
            }
        }

        private void ClearMovieDisplay()
        {
            lblTitle.Text = "Назва:";
            lblYear.Text = "Рік:";
            lblGenre.Text = "Жанр:";
            lblDirector.Text = "Режисер:";
            lblActors.Text = "Актори:";
            txtPlot.Text = string.Empty;
            picPoster.Image = null;
            currentMovie = null;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            if (currentMovie == null)
            {
                MessageBox.Show("Спочатку знайдіть фільм, щоб надіслати його.");
                return;
            }






            try
            {
                // згенерувати HTML і зберегти у тимчасовий файл
                var html = HtmlFormatter.GenerateMovieHtml(currentMovie);
                string tempPath = Path.Combine(Path.GetTempPath(), $"{SanitizeFileName(currentMovie.Title)}.html");
                File.WriteAllText(tempPath, html);

                var form = new SendEmailForm(currentMovie, tempPath);
                form.ShowDialog();

                // Відкрити у браузері (UseShellExecute = true)
                var psi = new ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                };
                Process.Start(psi);

                MessageBox.Show($"HTML збережено та відкрито: {tempPath}\nДалі можна прикріпити цей файл до листа або вставити HTML у тіло повідомлення.", "Готово");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при генерації HTML: {ex.Message}");
            }
        }

        private static string SanitizeFileName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "movie";
            foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return name;
        }


        private void ExportToPdf(string html, string outputPath)
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                using (var doc = new Document(PageSize.A4, 50, 50, 50, 50))
                {
                    PdfWriter.GetInstance(doc, stream);
                    doc.Open();

                    using (var reader = new StringReader(html))
                    {
                        var parser = new HTMLWorker(doc);
                        parser.Parse(reader);
                    }

                    doc.Close();
                }
            }
        }
    }
}
