using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using Cinestar_app;

namespace Cinestar_app;

public partial class HomePage : ContentPage
{
    public List<string> Images { get; set; } = new()
    {
        "film1.png", "film2.jpg", "film3.webp"
    };

    public ObservableCollection<Movie> FeaturedMovies { get; set; } = new();

    public HomePage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        string savedCity = Preferences.Get("SelectedCity", "Sarajevo");
        if (CityPickerButton != null)
            CityPickerButton.Text = $"{savedCity} ✓";

        BindingContext = this;
        _ = LoadFeaturedMoviesByCity();
    }

    private async Task LoadFeaturedMoviesByCity()
    {
        string city = Preferences.Get("SelectedCity", "Sarajevo");
        var cityMovies = await GetMoviesByCity(city, 8);

        FeaturedMovies.Clear();
        foreach (var movie in cityMovies)
        {
            FeaturedMovies.Add(movie);
        }
    }

    private static async Task<List<Movie>> GetMoviesByCity(string city, int count)
    {
        var http = new HttpClient();
        var movies = new List<Movie>();

        string[] cityGenres = city switch
        {
            "Sarajevo" => new[] { "action 2023", "drama 2023", "top 2023" },
            "Mostar" => new[] { "comedy 2023", "romance", "family" },
            "Banja Luka" => new[] { "thriller 2023", "crime", "mystery" },
            "Zenica" => new[] { "horror 2023", "fantasy", "sci-fi" },
            "Tuzla" => new[] { "top 2024", "blockbuster" },
            "Bihać" => new[] { "adventure", "action" },
            "Prijedor" => new[] { "drama", "war" },
            "Gračanica" => new[] { "comedy", "romance" },
            _ => new[] { "top 2023", "action", "drama" }
        };

        foreach (string genre in cityGenres)
        {
            try
            {
                var json = await http.GetStringAsync($"http://www.omdbapi.com/?s={Uri.EscapeDataString(genre)}&apikey=5d3a9b7a");
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (data.TryGetProperty("Response", out var responseProp) && responseProp.GetString() == "True")
                {
                    var searchResults = data.GetProperty("Search").EnumerateArray();
                    foreach (var movie in searchResults.Take(3))
                    {
                        var imdbId = movie.GetProperty("imdbID").GetString() ?? "";
                        var title = movie.GetProperty("Title").GetString() ?? "";

                        var detailsJson = await http.GetStringAsync($"http://www.omdbapi.com/?i={imdbId}&apikey=5d3a9b7a");
                        var details = JsonSerializer.Deserialize<JsonElement>(detailsJson);

                        movies.Add(new Movie
                        {
                            Title = details.GetProperty("Title").GetString() ?? title,
                            Poster = details.GetProperty("Poster").GetString() ?? "",
                            Year = details.GetProperty("Year").GetString() ?? "",
                            ImdbRating = details.GetProperty("imdbRating").GetString() ?? "",
                            ImdbID = imdbId
                        });

                        if (movies.Count >= count) break;
                    }
                }
            }
            catch { }
            if (movies.Count >= count) break;
        }
        return movies;
    }

    private async void OnCittySelected(object sender, EventArgs e)
    {
        var cityPickerPage = new CityPickerPage();
        await Navigation.PushModalAsync(cityPickerPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // AUTO REFRESH kad se vratiš na stranicu
        string savedCity = Preferences.Get("SelectedCity", "Sarajevo");
        if (CityPickerButton != null)
            CityPickerButton.Text = $"{savedCity} ✓";
        _ = LoadFeaturedMoviesByCity();
    }
}
