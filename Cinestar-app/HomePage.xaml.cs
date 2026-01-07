using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using Cinestar_app; // Za Movie

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

        string savedCity = Preferences.Get("SelectedCity", "Izaberi grad");
        if (CityPickerButton != null)
            CityPickerButton.Text = savedCity;

        BindingContext = this;
        _ = LoadFeaturedMovies();
    }

    private async Task LoadFeaturedMovies()
    {
        var http = new HttpClient();
        FeaturedMovies.Clear();

        FeaturedMovies.Add(await GetMovie(http, "Inception"));
        FeaturedMovies.Add(await GetMovie(http, "Oppenheimer"));
        FeaturedMovies.Add(await GetMovie(http, "Dune"));
        FeaturedMovies.Add(await GetMovie(http, "Godfather"));
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

    private async void OnCittySelected(object sender, EventArgs e)
    {
        var cityPickerPage = new CityPickerPage();
        await Navigation.PushModalAsync(cityPickerPage);
    }
}
