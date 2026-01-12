
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
    private readonly FilmService filmService = new FilmService();


    private string selectedCity;
    public string SelectedCity
    {
        get => selectedCity;
        set
        {
            if (selectedCity != value)
            {
                selectedCity = value;
                OnPropertyChanged(); // Label ce se odmah updateovati
            }
        }
    }

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

        // 🔹 Subscribe na grad promjenu
        MessagingCenter.Subscribe<CityPickerPage, string>(this, "CityChanged", async (sender, newCity) =>
        {
            if (newCity != SelectedCity)
            {
                SelectedCity = newCity;
                CityLabel.Text = SelectedCity;

                OverlayGrid.IsVisible = true;
                await Task.Delay(50); // mali delay da overlay proradi

                LoadCarousel();
                await LoadFilmSections();

                OverlayGrid.IsVisible = false;
            }
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_isLoaded) return;
        _isLoaded = true;

        OverlayGrid.IsVisible = true;
        await Task.Delay(50);

        LoadCarousel();
        await LoadFilmSections();

        OverlayGrid.IsVisible = false;
    }

    private void LoadCarousel()
    {
        HeroCarousel.ItemsSource = new List<CarouselItem>
{
    new CarouselItem
    {
        Image="martysupreme.png",
        Title="Marty Supreme",
        Year="2026",
        Description="Avantura i humor koji spaja generacije u nezaboravnu filmsku odiseju."
    },
    new CarouselItem
    {
        Image="neboiznadzenice.png",
        Title="Nebo iznad Zenice",
        Year="2026",
        Description="Emotivna priča o ljubavi i životu u srcu Bosne i Hercegovine."
    },
    new CarouselItem
    {
        Image="spongebob.png",
        Title="Spužva Bob: Potraga za skockanim",
        Year="2025",
        Description="Animirana pustolovina puna smijeha i prijateljstva — za cijelu obitelj."
    },
    new CarouselItem
    {
        Image="testament.png",
        Title="Testament",
        Year="2026",
        Description="Duboka drama koja propituje vjeru, sudbinu i ljudsku snagu."
    }
};

    }

    private async Task LoadFilmSections()
    {
        if (!cityQueries.ContainsKey(SelectedCity)) return;

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

                if (sections.All(s => s.Value.Count >= 6)) break;
            }
        }

        ComedyCollection.ItemsSource = sections["Comedy"];
        AdventureCollection.ItemsSource = sections["Adventure"];
        ActionCollection.ItemsSource = sections["Action"];
    }

    private async void OnCityTapped(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new CityPickerPage(false));
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
            var fullFilm = await filmService.GetFilmFromApi(film.ImdbID);
            await Navigation.PushAsync(new FilmDetalji(fullFilm));
        }
    }

    private async void OnSeeAllClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new Filmovi(SelectedCity));
    }
}
