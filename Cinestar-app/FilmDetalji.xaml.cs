using Cinestar_app.Models;
using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    public FilmDetalji(Film film)
    {
        InitializeComponent();
        BindingContext = film;
        


        PosterImage.Source = film.Poster;
        TitleLabel.Text = film.Title;
        PlotLabel.Text = film.Plot;

        ActorsCollectionView.ItemsSource = film.Actors; // ovo je ključno
    }

}
