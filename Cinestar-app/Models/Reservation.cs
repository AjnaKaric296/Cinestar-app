using SQLite;
using System;

namespace Cinestar_app.Models
{
    public class Reservation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string UserEmail { get; set; }
        public string FilmTitle { get; set; }
        public string FilmId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int TicketCount { get; set; }

        // Jedinstveni kod rezervacije
        public string ReservationCode { get; set; } = Guid.NewGuid().ToString();

        // QR link na Netlify stranicu sa parametrima rezervacije
        public string ReservationUrl
        {
            get
            {
                var filmEncoded = System.Net.WebUtility.UrlEncode(FilmTitle);
                var baseUrl = "https://splendorous-mousse-e5ac88.netlify.app/index.html";
                return $"{baseUrl}?film={filmEncoded}&date={Date:dd.MM.yyyy}&time={Time}&tickets={TicketCount}&code={ReservationCode}";
            }
        }
    }
}
