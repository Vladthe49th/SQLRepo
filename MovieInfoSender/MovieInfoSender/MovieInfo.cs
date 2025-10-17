using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoSender
{
    namespace MovieInfoSender
    {
        public class MovieInfo
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Genre { get; set; }
            public string Director { get; set; }
            public string Actors { get; set; }
            public string Plot { get; set; }
            public string Poster { get; set; }


            public string ToText()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Назва: {Title}");
                sb.AppendLine($"Рік: {Year}");
                sb.AppendLine($"Жанр: {Genre}");
                sb.AppendLine($"Режисер: {Director}");
                sb.AppendLine($"Актори: {Actors}");
                sb.AppendLine($"Опис: {Plot}");
                return sb.ToString();
            }

            public string ToHtml()
            {
                var sb = new StringBuilder();
                sb.AppendLine("<html><body style='font-family:Segoe UI;'>");
                sb.AppendLine($"<h2>{Title} ({Year})</h2>");
                if (!string.IsNullOrWhiteSpace(Poster) && Poster != "N/A")
                    sb.AppendLine($"<img src='{Poster}' width='200' style='float:right;margin-left:20px;'>");
                sb.AppendLine($"<p><b>Жанр:</b> {Genre}</p>");
                sb.AppendLine($"<p><b>Режисер:</b> {Director}</p>");
                sb.AppendLine($"<p><b>Актори:</b> {Actors}</p>");
                sb.AppendLine($"<p><b>Опис:</b> {Plot}</p>");
                sb.AppendLine("</body></html>");
                return sb.ToString();
            }
        }
    }

}
