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

    public Filmovi(string city)
    {
        InitializeComponent();
        selectedCity = city;
        LoadFilms();
    }

    private async void LoadFilms()
    {
        allFilms.Clear();

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
                    Showtimes = new() { "12:00", "15:00", "18:00" } // placeholder rezervacije
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
}
