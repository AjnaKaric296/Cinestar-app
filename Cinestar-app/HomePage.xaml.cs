using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Cinestar_app;

public partial class HomePage : ContentPage
{
    public List<string> Images { get; set; } = new()
    {
        "film1.png", "film2.jpg", "film3.webp"
    };

    public HomePage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        string savedCity = Preferences.Get("SelectedCity", "Izaberi grad");
        CityPickerButton.Text = savedCity;

        BindingContext = this;
        _ = LoadFeaturedMovies();
    }

    private async Task LoadFeaturedMovies()
    {
        try
        {
            string city = Preferences.Get("SelectedCity", "Sarajevo");
            var http = new HttpClient();

            string[] cityGenres = city switch
            {
                "Sarajevo" => new[] { "action 2023", "drama 2023" },
                "Mostar" => new[] { "comedy 2023", "romance" },
                "Banja Luka" => new[] { "thriller 2023", "crime" },
                _ => new[] { "top 2023" }
            };

            var movies = new List<Movie>();
            foreach (string genre in cityGenres.Take(4))
            {
                try
                {
                    var json = await http.GetStringAsync($"http://www.omdbapi.com/?s={Uri.EscapeDataString(genre)}&apikey=75ace56d");
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    if (data.TryGetProperty("Response", out var response) && response.GetString() == "True")
                    {
                        var search = data.GetProperty("Search").EnumerateArray().FirstOrDefault();
                        if (search.ValueKind != JsonValueKind.Undefined)
                        {
                            movies.Add(new Movie
                            {
                                Title = search.GetProperty("Title").GetString() ?? genre,
                                Poster = search.GetProperty("Poster").GetString() ?? ""
                            });
                        }
                    }
                }
                catch { }
            }

            // ✅ POPUNI 4 Frame-a dinamički
            if (movies.Count > 0) { Movie1Image.Source = movies[0].Poster; Movie1Title.Text = movies[0].Title; }
            if (movies.Count > 1) { Movie2Image.Source = movies[1].Poster; Movie2Title.Text = movies[1].Title; }
            if (movies.Count > 2) { Movie3Image.Source = movies[2].Poster; Movie3Title.Text = movies[2].Title; }
            if (movies.Count > 3) { Movie4Image.Source = movies[3].Poster; Movie4Title.Text = movies[3].Title; }
        }
        catch { }
    }

    private async void OnCittySelected(object sender, EventArgs e)
    {
        var cityPickerPage = new Cinestar_app.CityPickerPage();
        await Navigation.PushModalAsync(cityPickerPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        string savedCity = Preferences.Get("SelectedCity", "Izaberi grad");
        CityPickerButton.Text = savedCity;
        _ = LoadFeaturedMovies();
    }
}
