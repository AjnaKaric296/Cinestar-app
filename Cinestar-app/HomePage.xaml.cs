using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;

namespace Cinestar_app;

public partial class HomePage : ContentPage
{
    public ObservableCollection<Movie> Movies { get; set; } = new();

    public class Movie
    {
        public string Title { get; set; } = "";
        public string Poster { get; set; } = "";
    }

    public HomePage()
    {
        InitializeComponent();
        BindingContext = this;
        // ❌ UKLONIO NavigationPage.SetHasNavigationBar() - koristi Shell!
        _ = LoadMoviesAsync();
    }

    private async Task LoadMoviesAsync()
    {
        try
        {
            string city = Preferences.Get("SelectedCity", "Sarajevo");
            CityPickerButton.Text = city;

            using var http = new HttpClient();
            string[] genres = city switch
            {
                "Mostar" => new[] { "comedy 2023", "romance", "top 2023" },
                _ => new[] { "top 2023", "action 2023", "drama 2023" }
            };

            Movies.Clear();
            var featuredMovies = new List<Movie>();

            foreach (string genre in genres.Take(7))
            {
                string url = $"http://www.omdbapi.com/?s={Uri.EscapeDataString(genre)}&apikey=75ace56d";
                var json = await http.GetStringAsync(url);
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (data.TryGetProperty("Search", out var search) && search.GetArrayLength() > 0)
                {
                    var movie = search.EnumerateArray().First();
                    var poster = movie.GetProperty("Poster").GetString();
                    if (poster != "N/A")
                    {
                        var newMovie = new Movie
                        {
                            Title = movie.GetProperty("Title").GetString() ?? genre,
                            Poster = poster
                        };
                        Movies.Add(newMovie);
                        featuredMovies.Add(newMovie);
                    }
                }
            }

            // Update featured movies
            Device.BeginInvokeOnMainThread(() =>
            {
                UpdateFeaturedMovies(featuredMovies);
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void UpdateFeaturedMovies(List<Movie> movies)
    {
        Movie1Title.Text = movies.Count > 0 ? movies[0].Title : "Film 1";
        Movie1Image.Source = movies.Count > 0 ? movies[0].Poster : null;

        Movie2Title.Text = movies.Count > 1 ? movies[1].Title : "Film 2";
        Movie2Image.Source = movies.Count > 1 ? movies[1].Poster : null;

        Movie3Title.Text = movies.Count > 2 ? movies[2].Title : "Film 3";
        Movie3Image.Source = movies.Count > 2 ? movies[2].Poster : null;

        Movie4Title.Text = movies.Count > 3 ? movies[3].Title : "Film 4";
        Movie4Image.Source = movies.Count > 3 ? movies[3].Poster : null;
    }

    private async void OnCitySelected(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new CityPickerPage());
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadMoviesAsync();
    }
}
