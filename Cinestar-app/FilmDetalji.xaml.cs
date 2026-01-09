using Cinestar_app.Models;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    public FilmDetalji(Film film)
    {
        InitializeComponent();

        Title = film.Title;
        PosterImage.Source = film.Poster;
        GenreLabel.Text = film.Genre;
        YearLabel.Text = film.Year;
        CityLabel.Text = film.City;
        PlotLabel.Text = film.Plot;
    }
}
