using SQLite;
using SQLite.Net.Attributes;

namespace Cinestar_app.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Ime { get; set; } = "";
        public string Prezime { get; set; } = "";
        public string Email { get; set; } = "";
        public string Lozinka { get; set; } = "";
    }
}
