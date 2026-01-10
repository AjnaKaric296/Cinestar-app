using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Cinestar_app;

public partial class HomePage : ContentPage
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

    public HomePage()
    {
        InitializeComponent();

        selectedCity = Preferences.Get("SelectedCity", "Sarajevo");
        CityPicker.ItemsSource = cityQueries.Keys.ToList();
        CityPicker.SelectedItem = selectedCity;

        LoadFilms();
    }

    private async void LoadFilms()
    {
        allFilms.Clear();

        foreach (var query in cityQueries[selectedCity])
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

                if (allFilms.Count >= 15) break;
            }
            if (allFilms.Count >= 15) break;
        }

        CarouselFilms.ItemsSource = allFilms.Take(5).ToList();
        MoreFilmsCollectionView.ItemsSource = allFilms.Skip(5).ToList();
    }

    private void OnCityChanged(object sender, System.EventArgs e)
    {
        selectedCity = CityPicker.SelectedItem.ToString();
        Preferences.Set("SelectedCity", selectedCity);
        LoadFilms();
    }

    private async void OnCarouselSelected(object sender, SelectionChangedEventArgs e)
    {
        var film = e.CurrentSelection.FirstOrDefault() as Film;
        if (film == null) return;

        await Navigation.PushAsync(new FilmDetalji(film));
    }

    private async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
    {
        var film = e.CurrentSelection.FirstOrDefault() as Film;
        if (film == null) return;

        await Navigation.PushAsync(new FilmDetalji(film));
        MoreFilmsCollectionView.SelectedItem = null;
    }
}
