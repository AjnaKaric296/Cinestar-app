using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinestar_app;

public partial class HomePage : ContentPage
{
    private readonly OmdbService omdbService = new();
    private readonly List<Film> allFilms = new();

    public string SelectedCity { get; set; }
    public List<CarouselItem> CarouselImages { get; set; }

    private Dictionary<string, string[]> cityQueries = new()
    {
        { "Zenica", new[] { "dream", "star" } },
        { "Banja Luka", new[] { "super", "iron" } },
        { "Sarajevo", new[] { "good", "dark" } },
        { "Mostar", new[] { "bad", "war" } },
        { "Bihac", new[] { "all", "life" } },
        { "Tuzla", new[] { "time", "future" } },
        { "Prijedor", new[] { "happy", "fun" } },
        { "Gracanica", new[] { "sad", "cry" } }
    };

    public HomePage(string selectedCity)
    {
        InitializeComponent();
        SelectedCity = selectedCity;
        BindingContext = this;

        LoadCarouselImages();
        LoadFilms();
    }


    public void LoadCarouselImages()
    {
        CarouselImages = new()
        {
            new CarouselItem
            {
                Image = $"{SelectedCity.ToLower()}1.jpg",
                Title = "Greenland 2: Migracija",
                Description = "Borba za opstanak u novom svijetu."
            },
            new CarouselItem
            {
                Image = $"{SelectedCity.ToLower()}2.jpg",
                Title = "Veliki povratak",
                Description = "Spektakl koji se ne propusta."
            }
        };

        HeroCarousel.ItemsSource = CarouselImages;
    }

    private async Task LoadFilms()
    {
        allFilms.Clear();

        foreach (var query in cityQueries[SelectedCity])
        {
            var results = await omdbService.SearchMoviesAsync(query);

            foreach (var r in results)
            {
                var d = await omdbService.GetMovieDetailsAsync(r.imdbID);
                if (d == null) continue;

                allFilms.Add(new Film
                {
                    Title = d.Title,
                    Year = d.Year,
                    Genre = d.Genre,
                    Plot = d.Plot,
                    Poster = d.Poster == "N/A" ? "placeholder.png" : d.Poster,
                    Showtimes = new() { "12:00", "15:00", "18:00" }
                });

                if (allFilms.Count >= 6) break;
            }
            if (allFilms.Count >= 6) break;
        }

        FeaturedFilmsCollection.ItemsSource = null;
        FeaturedFilmsCollection.ItemsSource = allFilms;
    }

    private async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
    {
        var film = e.CurrentSelection.FirstOrDefault() as Film;
        if (film == null) return;

        await Navigation.PushAsync(new FilmDetalji(film));
        FeaturedFilmsCollection.SelectedItem = null;
    }

    private async void OnGoToFilmovi(object sender, EventArgs e)
    {
        var parent = this.Parent as TabbedPage;
        parent.CurrentPage = parent.Children[1]; // idi na Filmovi tab
    }
    private async void OnHomeTapped(object sender, EventArgs e)
    {
        // Otvori CityPickerPage
        await Navigation.PushAsync(new CityPickerPage());
    }
    private async void OnCityTapped(object sender, EventArgs e)
    {
        // Otvori CityPickerPage
        await Navigation.PushAsync(new CityPickerPage());
    }

}
