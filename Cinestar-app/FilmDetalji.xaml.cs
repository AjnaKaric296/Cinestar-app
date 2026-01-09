using Cinestar_app.Models;
using Microsoft.Maui.Controls;

namespace Cinestar_app
{
    public partial class FilmDetalji : ContentPage
    {
        public FilmDetalji(Film film)
        {
            InitializeComponent();

            TitleLabel.Text = film.Title;
            YearLabel.Text = film.Year;
            GenreLabel.Text = film.Genre;
            CityLabel.Text = film.City;
            PlotLabel.Text = film.Plot;
            PosterImage.Source = film.Poster;
        }
    }
}
