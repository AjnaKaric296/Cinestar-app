using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinestar_app;

public partial class Filmovi : ContentPage
{
    private OmdbService omdbService = new();
    private List<Film> allFilms = new();
    private string selectedCity;

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

    public Filmovi() : this("Sarajevo") { }

    public Filmovi(string selectedCity)
    {
        InitializeComponent();
        this.selectedCity = selectedCity;
        BindingContext = this;
        LoadFilmsWithOverlay();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    // ================= Loading overlay + filmovi =================
    private async Task LoadFilmsWithOverlay()
    {
        LoadingOverlay.IsVisible = true;
        await Task.Delay(100); // kratki delay da se overlay prikaže

        allFilms.Clear();

        if (!cityQueries.ContainsKey(selectedCity))
        {
            LoadingOverlay.IsVisible = false;
            return;
        }

        foreach (var q in cityQueries[selectedCity])
        {
            var res = await omdbService.SearchMoviesAsync(q);
            foreach (var r in res)
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

                if (allFilms.Count >= 20) break;
            }
            if (allFilms.Count >= 20) break;
        }

        FilmsCollectionView.ItemsSource = allFilms;

        // zanrovi
        var genres = allFilms
            .SelectMany(f => (f.Genre ?? "").Split(','))
            .Select(g => g.Trim())
            .Distinct()
            .ToList();
        genres.Insert(0, "Svi");
        GenrePicker.ItemsSource = genres;
        GenrePicker.SelectedIndex = 0;

        LoadingOverlay.IsVisible = false;
    }

    private void OnGenreChanged(object sender, System.EventArgs e)
    {
        var g = GenrePicker.SelectedItem?.ToString();
        FilmsCollectionView.ItemsSource =
            g == "Svi" ? allFilms : allFilms.Where(f => f.Genre.Contains(g)).ToList();
    }

    private async void OnFilmTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame?.BindingContext is Film film)
        {
            await Navigation.PushAsync(new FilmDetalji(film));
        }
    }

    private async void OnCityTapped(object sender, EventArgs e)
    {
        var cityPage = new CityPickerPage();
        cityPage.Disappearing += async (s, args) =>
        {
            var city = Preferences.Get("SelectedCity", "Sarajevo");
            if (city != selectedCity)
            {
                selectedCity = city;
                await LoadFilmsWithOverlay();
            }
        };

        await Navigation.PushAsync(cityPage);
    }

    private async void OnReserveClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        var film = button.BindingContext as Film;
        if (film == null) return;

        string selectedTime = button.Text;

        await DisplayAlert("Rezervacija",
            $"Film: {film.Title}\nTermin: {selectedTime}",
            "OK");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var city = Preferences.Get("SelectedCity", null);
        if (!string.IsNullOrEmpty(city) && city != selectedCity)
        {
            selectedCity = city;
            await LoadFilmsWithOverlay();
        }
    }
}
