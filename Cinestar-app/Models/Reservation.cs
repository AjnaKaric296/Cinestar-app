using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
