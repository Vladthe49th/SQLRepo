using System.Text;
using MovieInfoSender.MovieInfoSender;

namespace MovieInfoSender
{
    public static class HtmlFormatter
    {
        public static string GenerateMovieHtml(MovieInfo movie)
        {
            if (movie == null) return "<html><body><p>No movie data</p></body></html>";

            var sb = new StringBuilder();
            sb.AppendLine("<!doctype html>");
            sb.AppendLine("<html lang='uk'><head>");
            sb.AppendLine("<meta charset='utf-8'/>");
            sb.AppendLine("<meta name='viewport' content='width=device-width,initial-scale=1'/>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{font-family:Segoe UI, Arial, sans-serif;background:#f4f4f4;padding:20px;color:#222}");
            sb.AppendLine(".card{background:#fff;border-radius:10px;padding:20px;box-shadow:0 4px 12px rgba(0,0,0,0.08);max-width:900px;margin:auto;overflow:hidden}");
            sb.AppendLine("img.poster{float:right;margin-left:20px;border-radius:6px;max-width:220px}");
            sb.AppendLine("h1{color:#1f6feb;margin-top:0}");
            sb.AppendLine(".meta{margin-bottom:10px;color:#555}");
            sb.AppendLine(".clear{clear:both}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head><body>");
            sb.AppendLine("<div class='card'>");

            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                // Якщо масив-постер — в OMDb часто буває "N/A"
                sb.AppendLine($"<img class='poster' src='{movie.Poster}' alt='Poster'/>");
            }

            sb.AppendLine($"<h1>{Escape(movie.Title)}{(string.IsNullOrWhiteSpace(movie.Year) ? "" : $" ({Escape(movie.Year)})")}</h1>");
            sb.AppendLine("<div class='meta'>");
            if (!string.IsNullOrWhiteSpace(movie.Genre)) sb.AppendLine($"<div><b>Жанр:</b> {Escape(movie.Genre)}</div>");
            if (!string.IsNullOrWhiteSpace(movie.Director)) sb.AppendLine($"<div><b>Режисер:</b> {Escape(movie.Director)}</div>");
            if (!string.IsNullOrWhiteSpace(movie.Actors)) sb.AppendLine($"<div><b>Актори:</b> {Escape(movie.Actors)}</div>");
            sb.AppendLine("</div>");
            if (!string.IsNullOrWhiteSpace(movie.Plot))
            {
                sb.AppendLine($"<h3>Опис</h3><p>{Escape(movie.Plot).Replace("\n", "<br/>")}</p>");
            }

            sb.AppendLine("<div class='clear'></div>");
            sb.AppendLine("</div>"); // card
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static string Escape(string? s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return System.Net.WebUtility.HtmlEncode(s);
        }
    }
}
