using Cinestar_app.Models;
using Cinestar_app.Services;

namespace Cinestar_app;

public partial class HomePage : ContentPage
{
    private OmdbService omdbService = new OmdbService();
    private List<Film> allFilms = new();
    private string[] cities = { "Mostar", "Bihac", "Tuzla", "Banja Luka", "Zenica", "Sarajevo", "Prijedor", "Gracanica" };
    private string[] popularMovies = { "Inception", "Frozen", "Titanic", "Avengers", "Joker", "Coco", "Interstellar", "Avatar", "The Dark Knight", "Shrek", "Spider-Man", "Up", "La La Land", "Guardians of the Galaxy", "The Lion King" };

    public List<string> Images { get; set; } = new();

    public HomePage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        string savedCity = Preferences.Get("SelectedCity", "Izaberi grad");
        CityPickerButton.Text = savedCity;

        BindingContext = this;

        LoadFilmsForCity(savedCity);
    }

    private async void LoadFilmsForCity(string city)
    {
        allFilms.Clear();
        var rnd = new Random();

        foreach (var title in popularMovies)
        {
            var results = await omdbService.SearchMoviesAsync(title);
            if (results.Count > 0)
            {
                var film = results[0];
                film.City = cities[rnd.Next(cities.Length)];
                allFilms.Add(film);
            }
        }

        // Filtriramo filmove po gradu i uzimamo bar 15
        var filtered = allFilms.Where(f => f.City == city).Take(15).ToList();

        Images.Clear();
        foreach (var f in filtered)
            Images.Add(f.Poster ?? "placeholder.png");

        BindingContext = null;
        BindingContext = this;
    }

    private async void OnCittySelected(object sender, EventArgs e)
    {
        var cityPickerPage = new CityPickerPage();
        await Navigation.PushModalAsync(cityPickerPage);
    }
}
