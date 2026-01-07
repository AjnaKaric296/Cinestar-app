using Cinestar_app;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    public Movie SelectedMovie { get; set; } = new Movie();

    public FilmDetalji(Movie movie = null)
    {
        InitializeComponent();
        SelectedMovie = movie ?? new Movie();
        BindingContext = this;
    }
}
