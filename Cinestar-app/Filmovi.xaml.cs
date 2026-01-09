using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinestar_app
{
    public partial class Filmovi : ContentPage
    {
        private OmdbService omdbService = new OmdbService();
        private List<Film> allFilms = new();
        private string selectedCity;

        public Filmovi()
        {
            InitializeComponent();
            selectedCity = Preferences.Get("SelectedCity", "Sarajevo"); // grad iz CityPicker
            LoadFilms();
        }

        private async void LoadFilms()
        {
            allFilms.Clear();

            var queries = new[] { "a", "e", "i", "o", "u", "love", "star", "night" }; // više query-a za više filmova

            var tempFilms = new List<Film>();

            foreach (var query in queries)
            {
                var results = await omdbService.SearchMoviesAsync(query);
                foreach (var item in results)
                {
                    // Uzimamo samo filmove za odabrani grad
                    var film = new Film
                    {
                        Title = item.Title,
                        Year = item.Year,
                        ImdbID = item.ImdbID,
                        City = selectedCity,
                        Poster = item.Poster
                    };

                    // Dohvat detalja filma
                    var details = await omdbService.GetMovieDetailsAsync(film.ImdbID);
                    if (details != null)
                    {
                        film.Genre = details.Genre;
                        film.Plot = details.Plot;
                    }

                    tempFilms.Add(film);
                    if (tempFilms.Count(f => f.City == selectedCity) >= 20) break; // limit 20 filmova po gradu
                }
                if (tempFilms.Count(f => f.City == selectedCity) >= 20) break;
            }

            allFilms = tempFilms;

            // Postavljanje zanrova
            var genres = allFilms.SelectMany(f => (f.Genre ?? "").Split(','))
                                 .Select(g => g.Trim())
                                 .Where(g => !string.IsNullOrEmpty(g))
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
            }
        }
    }
}
