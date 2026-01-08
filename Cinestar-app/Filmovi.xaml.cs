using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using Cinestar_app;

namespace Cinestar_app;

public partial class Filmovi : ContentPage
{
    public ObservableCollection<Movie> Movies { get; set; } = new();

    public Filmovi()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = this;

        if (FilmoviList != null)
            FilmoviList.BindingContext = this;

        _ = LoadMoviesByCity();
    }

    private async Task LoadMoviesByCity()
    {
        string city = Preferences.Get("SelectedCity", "Sarajevo");
        var cityMovies = await GetMoviesByCity(city, 20);

        Movies.Clear();
        foreach (var movie in cityMovies)
        {
            Movies.Add(movie);
        }
    }

    private static async Task<List<Movie>> GetMoviesByCity(string city, int count)
    {
        var http = new HttpClient();
        var movies = new List<Movie>();

        string[] cityGenres = city switch
        {
            "Sarajevo" => new[] { "action 2023", "drama 2023", "war", "top 2023" },
            "Mostar" => new[] { "comedy 2023", "romance", "family", "musical" },
            "Banja Luka" => new[] { "thriller 2023", "crime", "mystery", "detective" },
            "Zenica" => new[] { "horror 2023", "fantasy", "sci-fi", "adventure" },
            "Tuzla" => new[] { "top 2024", "blockbuster", "action 2024" },
            "Bihać" => new[] { "adventure 2023", "action", "drama" },
            "Prijedor" => new[] { "war", "drama 2023", "history" },
            "Gračanica" => new[] { "comedy", "romance 2023", "family" },
            _ => new[] { "top 2023", "action", "drama", "thriller" }
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
                    foreach (var movie in searchResults.Take(5))
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
        return movies.Take(count).ToList();
    }

    private async void IdiNaFIlmDetalji(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Movie movie)
        {
            await Navigation.PushAsync(new FilmDetalji(movie));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // AUTO REFRESH kad tab postane active
        _ = LoadMoviesByCity();
    }
}
