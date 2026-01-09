using Cinestar_app.Models;
using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class Filmovi : ContentPage
{
    private OmdbService omdbService = new();
    private List<Film> allFilms = new();
    private List<string> cities = new() { "Mostar", "Bihac", "Tuzla", "Banja Luka", "Zenica", "Sarajevo", "Prijedor", "Gracanica" };

    public Filmovi()
    {
        InitializeComponent();
        LoadFilms();
    }

    private async void LoadFilms()
    {
        var queries = new[] { "The", "Love", "War", "Star", "Night", "Life", "Dark" };
        var rnd = new Random();

        foreach (var query in queries)
        {
            var results = await omdbService.SearchMoviesAsync(query);
            foreach (var film in results)
            {
                film.City = cities[rnd.Next(cities.Count)];

                var details = await omdbService.GetMovieDetailsAsync(film.ImdbID);
                film.Genre = details.Genre;
                film.Plot = details.Plot;

                allFilms.Add(film);
            }
        }

        // Dodaj "Svi" u Picker i sve zanrove
        var genres = allFilms
            .Where(f => !string.IsNullOrWhiteSpace(f.Genre))
            .SelectMany(f => f.Genre.Split(','))
            .Select(g => g.Trim())
            .Distinct()
            .ToList();

        genres.Insert(0, "Svi");
        GenrePicker.ItemsSource = genres;
        GenrePicker.SelectedIndex = 0;

        FilmsCollectionView.ItemsSource = allFilms;
    }

    private void OnGenreChanged(object sender, EventArgs e)
    {
        string selectedGenre = GenrePicker.SelectedItem?.ToString() ?? "Svi";

        if (selectedGenre == "Svi")
            FilmsCollectionView.ItemsSource = allFilms;
        else
            FilmsCollectionView.ItemsSource = allFilms.Where(f => f.Genre != null && f.Genre.Contains(selectedGenre)).ToList();
    }

    private async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Film selectedFilm)
        {
            await Navigation.PushAsync(new FilmDetalji(selectedFilm));
            ((CollectionView)sender).SelectedItem = null; // reset selection
        }
    }
}
