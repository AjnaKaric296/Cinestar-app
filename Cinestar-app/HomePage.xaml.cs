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
    public partial class HomePage : ContentPage
    {
        private OmdbService omdbService = new OmdbService();
        public ObservableCollection<Film> allFilms { get; private set; } = new ObservableCollection<Film>();
        private string selectedCity;

        private string[] cities = new[] { "Zenica", "Banja Luka", "Sarajevo", "Mostar", "Bihac", "Tuzla", "Prijedor", "Gracanica" };

        private string[] commonQueries = new[] { "man", "love", "hero" };
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
            CityPicker.ItemsSource = cities;
            CityPicker.SelectedItem = selectedCity;

            CarouselFilms.ItemsSource = allFilms;
            MoreFilmsCollectionView.ItemsSource = allFilms;

            _ = LoadFilmsAsync();
        }

        private async Task LoadFilmsAsync()
        {
            var tempFilms = new List<Film>();
            var queries = new List<string>();
            queries.AddRange(commonQueries);
            if (cityQueries.ContainsKey(selectedCity))
                queries.AddRange(cityQueries[selectedCity]);

            // Dohvati filmove paralelno po queryjima
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

                    if (films.Count >= 5) break; // limit po queryju da ne učitava previše
                }
                return films;
            });

            var resultsPerQuery = await Task.WhenAll(tasks);

            tempFilms = resultsPerQuery.SelectMany(f => f).Take(20).ToList(); // ukupno 20 filmova

            // Update ObservableCollection sigurno za Android
            allFilms.Clear();
            foreach (var f in tempFilms)
                allFilms.Add(f);
        }

        private void OnCityChanged(object sender, EventArgs e)
        {
            selectedCity = CityPicker.SelectedItem?.ToString() ?? "Sarajevo";
            Preferences.Set("SelectedCity", selectedCity);

            _ = LoadFilmsAsync(); // refresh HomePage
        }

        private async void OnCarouselSelected(object sender, SelectionChangedEventArgs e)
        {
            var film = e.CurrentSelection.FirstOrDefault() as Film;
            if (film == null) return;

            await Navigation.PushAsync(new FilmDetalji(film));
            CarouselFilms.SelectedItem = null;
        }

        private async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
        {
            var film = e.CurrentSelection.FirstOrDefault() as Film;
            if (film == null) return;

            await Navigation.PushAsync(new FilmDetalji(film));
            MoreFilmsCollectionView.SelectedItem = null;
        }
    }
}
