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
            CityPickerButton.Text = savedCity;

        BindingContext = this;
        _ = LoadFeaturedMovies();
    }

    private async Task LoadFeaturedMovies()
    {
        string city = Preferences.Get("SelectedCity", "Sarajevo");
        var http = new HttpClient();
        FeaturedMovies.Clear();

        string[] cityMovies = city switch
        {
            "Sarajevo" => new[] { "action 2023", "drama 2023" },
            "Mostar" => new[] { "comedy 2023", "romance" },
            "Banja Luka" => new[] { "thriller 2023", "crime" },
            "Zenica" => new[] { "horror 2023", "fantasy" },
            "Tuzla" => new[] { "top 2024" },
            _ => new[] { "top 2023", "action", "drama" }
        };

        foreach (string searchTerm in cityMovies.Take(4))
        {
            try
            {
                var json = await http.GetStringAsync($"http://www.omdbapi.com/?s={Uri.EscapeDataString(searchTerm)}&apikey=75ace56d");
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (data.TryGetProperty("Response", out var response) && response.GetString() == "True")
                {
                    var search = data.GetProperty("Search").EnumerateArray().FirstOrDefault();
                    if (search.ValueKind != JsonValueKind.Undefined)
                    {
                        FeaturedMovies.Add(new Movie
                        {
                            Title = search.GetProperty("Title").GetString() ?? searchTerm,
                            Poster = search.GetProperty("Poster").GetString() ?? "",
                            Year = search.GetProperty("Year").GetString() ?? "",
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

    private async void OnCittySelected(object sender, EventArgs e)
    {
        var cityPickerPage = new CityPickerPage();
        await Navigation.PushModalAsync(cityPickerPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        string savedCity = Preferences.Get("SelectedCity", "Sarajevo");
        if (CityPickerButton != null)
            CityPickerButton.Text = savedCity;
        _ = LoadFeaturedMovies();
    }
}
