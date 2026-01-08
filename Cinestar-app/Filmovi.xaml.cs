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

        _ = LoadMovies();
    }

    private async Task LoadMovies()
    {
        string city = Preferences.Get("SelectedCity", "Sarajevo");
        var http = new HttpClient();
        Movies.Clear();

        string[] cityGenres = city switch
        {
            "Sarajevo" => new[] { "action 2023", "drama 2023", "top 2023" },
            "Mostar" => new[] { "comedy 2023", "romance", "family" },
            "Banja Luka" => new[] { "thriller 2023", "crime", "mystery" },
            _ => new[] { "top 2023", "action", "drama" }
        };

        foreach (string genre in cityGenres)
        {
            try
            {
                var json = await http.GetStringAsync($"http://www.omdbapi.com/?s={Uri.EscapeDataString(genre)}&apikey=75ace56d");
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (data.TryGetProperty("Response", out var response) && response.GetString() == "True")
                {
                    var search = data.GetProperty("Search").EnumerateArray();
                    foreach (var movie in search.Take(3))
                    {
                        Movies.Add(new Movie
                        {
                            Title = movie.GetProperty("Title").GetString() ?? "",
                            Poster = movie.GetProperty("Poster").GetString() ?? "",
                            Year = movie.GetProperty("Year").GetString() ?? "",
                            ImdbRating = "N/A"
                        });
                    }
                }
            }
            catch { }
        }
    }

    private static async Task<Movie> GetMovie(HttpClient http, string title)
    {
        try
        {
            var json = await http.GetStringAsync($"http://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey=75ace56d");
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            return new Movie
            {
                Title = data.GetProperty("Title").GetString() ?? title,
                Poster = data.GetProperty("Poster").GetString() ?? "",
                Year = data.GetProperty("Year").GetString() ?? "",
                ImdbRating = data.GetProperty("imdbRating").GetString() ?? ""
            };
        }
        catch
        {
            return new Movie { Title = title };
        }
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
        _ = LoadMovies();
    }
}
