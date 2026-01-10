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
        LoadFilms();
        NavigationPage.SetHasNavigationBar(this, false);

    }


    private async void LoadFilms()
    {
        allFilms.Clear();

        if (!cityQueries.ContainsKey(selectedCity))
            return;

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

        var genres = allFilms
            .SelectMany(f => (f.Genre ?? "").Split(','))
            .Select(g => g.Trim())
            .Distinct()
            .ToList();

        genres.Insert(0, "Svi");
        GenrePicker.ItemsSource = genres;
        GenrePicker.SelectedIndex = 0;
    }

    private void OnGenreChanged(object sender, System.EventArgs e)
    {
        var g = GenrePicker.SelectedItem?.ToString();
        FilmsCollectionView.ItemsSource =
            g == "Svi" ? allFilms : allFilms.Where(f => f.Genre.Contains(g)).ToList();
    }

    private async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
    {
        var film = e.CurrentSelection.FirstOrDefault() as Film;
        if (film == null) return;

        await Navigation.PushAsync(new FilmDetalji(film));
        FilmsCollectionView.SelectedItem = null;
    }

    // **Samo jedan OnCityTapped**
    private async void OnCityTapped(object sender, EventArgs e)
    {
        var cityPage = new CityPickerPage();
        cityPage.Disappearing += (s, args) =>
        {
            // Kada se vrati sa CityPickerPage, update grad i reload filmove
            selectedCity = Preferences.Get("SelectedCity", "Sarajevo");
            LoadFilms();
        };

        await Navigation.PushAsync(cityPage);
    }

    // OnReserveClicked sa alertom
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var city = Preferences.Get("SelectedCity", null);
        if (!string.IsNullOrEmpty(city) && city != selectedCity)
        {
            selectedCity = city;
            LoadFilms();
        }
    }
}
