using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using Cinestar_app; // Za Movie

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
        var http = new HttpClient();
        Movies.Clear();
        Movies.Add(await GetMovie(http, "Inception"));
        Movies.Add(await GetMovie(http, "Oppenheimer"));
        Movies.Add(await GetMovie(http, "Dune"));
    }

    private static async Task<Movie> GetMovie(HttpClient http, string title)
    {
        try
        {
            var json = await http.GetStringAsync($"http://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey=5d3a9b7a");
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
}
