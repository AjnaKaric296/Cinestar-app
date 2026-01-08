using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Cinestar_app;

public partial class Filmovi : ContentPage
{
    public ObservableCollection<Movie> Movies { get; set; } = new();

    public Filmovi()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        FilmoviCollection.ItemsSource = Movies;
        _ = LoadMovies();
    }

    private async Task LoadMovies()
    {
        try
        {
            string city = Preferences.Get("SelectedCity", "Sarajevo");
            var http = new HttpClient();
            Movies.Clear();

            string[] cityGenres = city switch
            {
                "Sarajevo" => new[] { "action 2023", "drama 2023" },
                "Mostar" => new[] { "comedy 2023", "romance" },
                _ => new[] { "top 2023" }
            };

            foreach (string genre in cityGenres)
            {
                try
                {
                    var json = await http.GetStringAsync($"http://www.omdbapi.com/?s={Uri.EscapeDataString(genre)}&apikey=75ace56d");
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    if (data.TryGetProperty("Response", out var response) && response.GetString() == "True")
                    {
                        var search = data.GetProperty("Search").EnumerateArray().Take(5);
                        foreach (var movie in search)
                        {
                            Movies.Add(new Movie
                            {
                                Title = movie.GetProperty("Title").GetString() ?? "",
                                Poster = movie.GetProperty("Poster").GetString() ?? ""
                            });
                        }
                    }
                }
                catch { }
            }
        }
        catch { }
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
