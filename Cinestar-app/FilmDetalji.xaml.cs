using Cinestar_app;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    public FilmDetalji(Movie movie = null)
    {
        InitializeComponent();
        if (movie != null)
        {
            // TODO: prikaži detalje filma
            Title = movie.Title;
        }
    }
}

