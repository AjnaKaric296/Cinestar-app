using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cinestar_app
{
    public partial class Filmovi : ContentPage
    {
        private OmdbService omdbService = new OmdbService();
        private ObservableCollection<Film> allFilms = new ObservableCollection<Film>();
        private string selectedCity;

        private string[] commonQueries = new[] { "zenice" };
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

        public Filmovi()
        {
            InitializeComponent();

            selectedCity = Preferences.Get("SelectedCity", "Sarajevo");

            FilmsCollectionView.ItemsSource = allFilms;

            _ = LoadFilmsAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // refresh po trenutnom gradu iz Preferences
            var city = Preferences.Get("SelectedCity", "Sarajevo");
            if (city != selectedCity)
            {
                selectedCity = city;
                _ = LoadFilmsAsync();
            }
        }

        private async Task LoadFilmsAsync()
        {
            var tempFilms = new List<Film>();
            var queries = new List<string>();
            queries.AddRange(commonQueries);
            if (cityQueries.ContainsKey(selectedCity))
                queries.AddRange(cityQueries[selectedCity]);

            var tasks = queries.Select(async query =>
            {
                var results = await omdbService.SearchMoviesAsync(query);
                var films = new List<Film>();
                foreach (var item in results)
                {
                    var details = await omdbService.GetMovieDetailsAsync(item.imdbID);
                    if (details == null) continue;

                    films.Add(new Film
                    {
                        Title = details.Title,
                        Year = details.Year,
                        Genre = details.Genre,
                        Plot = details.Plot,
                        Poster = string.IsNullOrEmpty(details.Poster) || details.Poster == "N/A" ? "placeholder.png" : details.Poster,
                        City = selectedCity,
                        ImdbID = details.imdbID,
                        Showtimes = new List<string> { "12:00", "15:00", "18:00" }
                    });

                    if (films.Count >= 5) break;
                }
                return films;
            });

            var resultsPerQuery = await Task.WhenAll(tasks);

            tempFilms = resultsPerQuery.SelectMany(f => f).Take(20).ToList();

            allFilms.Clear();
            foreach (var f in tempFilms)
                allFilms.Add(f);
        }

        void OnGenreChanged(object sender, EventArgs e)
        {
            string selectedGenre = GenrePicker.SelectedItem?.ToString() ?? "Svi";
            if (selectedGenre == "Svi")
            {
                FilmsCollectionView.ItemsSource = allFilms;
            }
            else
            {
                FilmsCollectionView.ItemsSource = new ObservableCollection<Film>(
                    allFilms.Where(f => f.Genre != null && f.Genre.Contains(selectedGenre))
                );
            }
        }

        async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
        {
            var film = e.CurrentSelection.FirstOrDefault() as Film;
            if (film == null) return;

            await Navigation.PushAsync(new FilmDetalji(film));
            FilmsCollectionView.SelectedItem = null;
        }

        async void OnShowtimeClicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                var film = btn.BindingContext as Film;
                if (film != null)
                    await DisplayAlert("Termin", $"{film.Title} - {btn.Text}", "OK");
            }
        }
    }
}
