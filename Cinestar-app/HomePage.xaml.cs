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
    private bool _isLoaded;

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

        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_isLoaded)
            return;

        _isLoaded = true;
        await LoadDataWithOverlay();
    }

    private async Task LoadDataWithOverlay()
    {
        OverlayGrid.IsVisible = true;
        await Task.Delay(1); // samo da UI dobije frame

        LoadCarousel();
        await LoadFilmSections();

        OverlayGrid.IsVisible = false;
    }

    private void LoadCarousel()
    {
        HeroCarousel.ItemsSource = new List<CarouselItem>
        {
            new CarouselItem { Image="film1.png", Title="Film 1", Year="2023", Description="Opis 1" },
            new CarouselItem { Image="film2.jpg", Title="Film 2", Year="2022", Description="Opis 2" },
            new CarouselItem { Image="film3.jpg", Title="Film 3", Year="2024", Description="Opis 3" }
        };
    }

    private async Task LoadFilmSections()
    {
        if (!cityQueries.ContainsKey(SelectedCity))
            return;

        var sections = new Dictionary<string, List<Film>>
        {
            { "Comedy", new() },
            { "Adventure", new() },
            { "Action", new() }
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

                if (sections["Comedy"].Count < 6 && d.Genre?.ToLower().Contains("comedy") == true)
                    sections["Comedy"].Add(film);

                if (sections["Adventure"].Count < 6 && d.Genre?.ToLower().Contains("adventure") == true)
                    sections["Adventure"].Add(film);

                if (sections["Action"].Count < 6 && d.Genre?.ToLower().Contains("action") == true)
                    sections["Action"].Add(film);

                if (sections.All(s => s.Value.Count >= 6))
                    break;
            }
        }

        ComedyCollection.ItemsSource = sections["Comedy"];
        AdventureCollection.ItemsSource = sections["Adventure"];
        ActionCollection.ItemsSource = sections["Action"];
    }

    private async void OnCityTapped(object sender, System.EventArgs e)
    {
        var cityPage = new CityPickerPage();

        cityPage.Disappearing += async (_, _) =>
        {
            SelectedCity = Preferences.Get("SelectedCity", "Sarajevo");
            CityLabel.Text = SelectedCity;

            OverlayGrid.IsVisible = true;
            await Task.Delay(1);

            LoadCarousel();
            await LoadFilmSections();

            OverlayGrid.IsVisible = false;
        };

        await Navigation.PushAsync(cityPage);
    }

    private async void OnCarouselTapped(object sender, System.EventArgs e)
    {
        if ((sender as Grid)?.BindingContext is CarouselItem item)
        {
            await Navigation.PushAsync(new FilmDetalji(new Film
            {
                Title = item.Title,
                Year = item.Year,
                Plot = item.Description,
                Poster = item.Image
            }));
        }
    }

    private async void OnFilmTapped(object sender, System.EventArgs e)
    {
        if ((sender as Frame)?.BindingContext is Film film)
        {
            await Navigation.PushAsync(new FilmDetalji(film));
        }
    }

    private async void OnSeeAllClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new Filmovi(SelectedCity));
    }
}
