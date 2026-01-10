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
    private OmdbService omdbService = new();
    public string SelectedCity { get; set; }

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

    public HomePage() : this("Sarajevo") { }

    public HomePage(string selectedCity)
    {
        InitializeComponent();
        SelectedCity = selectedCity;
        BindingContext = this;

        LoadCarousel();
        _ = LoadFilmSections();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    // Carousel sa 3 improvizovane slike
    private void LoadCarousel()
    {
        var carouselItems = new List<CarouselItem>
        {
            new CarouselItem { Image="film1.png", Title="Film 1", Year="2023", Description="Opis 1" },
            new CarouselItem { Image="film2.jpg", Title="Film 2", Year="2022", Description="Opis 2" },
            new CarouselItem { Image="film3.webp", Title="Film 3", Year="2024", Description="Opis 3" }
        };
        HeroCarousel.ItemsSource = carouselItems;
    }

    // Horizontalne sekcije po kategorijama
    private async Task LoadFilmSections()
    {
        if (!cityQueries.ContainsKey(SelectedCity)) return;

        var sections = new Dictionary<string, List<Film>>
        {
            { "Comedy", new List<Film>() },
            { "Adventure", new List<Film>() },
            { "Action", new List<Film>() }
        };

        foreach (var query in cityQueries[SelectedCity])
        {
            var results = await omdbService.SearchMoviesAsync(query);

            foreach (var r in results)
            {
                var d = await omdbService.GetMovieDetailsAsync(r.imdbID);
                if (d == null) continue;

                var film = new Film
                {
                    Title = d.Title,
                    Year = d.Year,
                    Genre = d.Genre,
                    Poster = d.Poster == "N/A" ? "placeholder.png" : d.Poster,
                    City = SelectedCity
                };

                if (sections["Comedy"].Count < 6 && (d.Genre?.ToLower().Contains("comedy") ?? false))
                    sections["Comedy"].Add(film);

                if (sections["Adventure"].Count < 6 && (d.Genre?.ToLower().Contains("adventure") ?? false))
                    sections["Adventure"].Add(film);

                if (sections["Action"].Count < 6 && (d.Genre?.ToLower().Contains("action") ?? false))
                    sections["Action"].Add(film);

                if (sections.All(s => s.Value.Count >= 6)) break;
            }
        }

        ComedyCollection.ItemsSource = sections["Comedy"];
        AdventureCollection.ItemsSource = sections["Adventure"];
        ActionCollection.ItemsSource = sections["Action"];
    }

    // Promjena grada
    private async void OnCityTapped(object sender, System.EventArgs e)
    {
        var cityPage = new CityPickerPage();
        cityPage.Disappearing += async (s, args) =>
        {
            SelectedCity = Preferences.Get("SelectedCity", "Sarajevo");
            CityLabel.Text = SelectedCity;
            LoadCarousel();
            await LoadFilmSections();
        };
        await Navigation.PushAsync(cityPage);
    }
}
