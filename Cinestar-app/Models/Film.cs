namespace Cinestar_app.Models
{
    public class Film
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public string Poster { get; set; }
        public string City { get; set; } // grad
        public string Plot { get; set; }
        public string ImdbID { get; set; }
    }
}
