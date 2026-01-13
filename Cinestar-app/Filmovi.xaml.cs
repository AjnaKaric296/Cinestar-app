
using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cinestar_app;

public partial class Filmovi : ContentPage, INotifyPropertyChanged
{
    private readonly OmdbService omdbService = new();
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly FilmService filmService = new FilmService();


    private string selectedCity;
    public string SelectedCity
    {
        get => selectedCity;
        set
        {
            if (selectedCity != value)
            {
                selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
                _ = ReloadFilmsForCity();
            }
        }
    }

    public ObservableCollection<Film> Films { get; set; } = new();

    private readonly Dictionary<string, string[]> cityQueries = new()
    {
        { "Zenica", new[] { "dream", "star" } },
        { "Banja Luka", new[] { "super", "iron" } },
        { "Sarajevo", new[] { "good", "dark" } },
        { "Mostar", new[] { "bad", "war" } },
        { "Bihac", new[] { "all", "life" } },
        { "Tuzla", new[] { "time", "future" } },
        { "Prijedor", new[] { "happy", "fun" } },
        { "Gracanica", new[] { "sad", "cry" } }
    };

    public Filmovi() : this("Sarajevo") { }

    public Filmovi(string city)
    {
        InitializeComponent();
        BindingContext = this;
        SelectedCity = city;

        FilmsCollectionView.ItemsSource = Films;

        MessagingCenter.Subscribe<CityPickerPage, string>(this, "CityChanged", (sender, newCity) =>
        {
            if (newCity != SelectedCity)
                SelectedCity = newCity;
        });

        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReloadFilmsForCity();
    }

    private async Task ReloadFilmsForCity()
    {
        LoadingOverlay.IsVisible = true;

        try
        {
            Films.Clear();
            if (!cityQueries.ContainsKey(SelectedCity)) return;

            var allFilms = new List<Film>();

            foreach (var query in cityQueries[SelectedCity])
            {
                var results = await omdbService.SearchMoviesAsync(query);
                foreach (var r in results.Take(5))
                {
                    var details = await omdbService.GetMovieDetailsAsync(r.imdbID);
                    if (details == null) continue;

                    var film = new Film
                    {
                        Title = details.Title,
                        Year = details.Year,
                        Genre = details.Genre,
                        Plot = details.Plot,
                        Poster = details.Poster == "N/A" ? "placeholder.png" : details.Poster,
                        ImdbID = details.imdbID,
                        Actors = (details.Actors ?? "")
                                    .Split(", ")
                                    .Where(a => !string.IsNullOrWhiteSpace(a) && a != "N/A")
                                    .Select(a => new Actor
                                    {
                                        Name = a,
                                        Photo = "https://thispersondoesnotexist.com/image"
                                    })
                                    .ToList(),
                        Showtimes = new List<string> { "12:00", "15:00", "18:00", "21:00" }
                    };

                    allFilms.Add(film);
                }
            }

            foreach (var f in allFilms)
                Films.Add(f);

            // Popuni GenrePicker
            var genres = Films
                .SelectMany(f => (f.Genre ?? "").Split(','))
                .Select(g => g.Trim())
                .Distinct()
                .ToList();
            genres.Insert(0, "Svi");
            GenrePicker.ItemsSource = genres;
            GenrePicker.SelectedIndex = 0;
        }
        catch (System.Exception ex)
        {
            await DisplayAlert("Greška", ex.Message, "OK");
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private void OnGenreChanged(object sender, System.EventArgs e)
    {
        var g = GenrePicker.SelectedItem?.ToString();
        if (g == "Svi")
            FilmsCollectionView.ItemsSource = Films;
        else
            FilmsCollectionView.ItemsSource = new ObservableCollection<Film>(
                Films.Where(f => f.Genre != null && f.Genre.Contains(g))
            );
    }

    private async void OnFilmTapped(object sender, System.EventArgs e)
    {
        if ((sender as Frame)?.BindingContext is Film film)
        {
            // Ako film Actors nisu popunjeni, uzmi puni film preko ImdbID
            if (film.Actors == null || film.Actors.Count == 0)
            {
                var fullFilm = await filmService.GetFilmFromApi(film.ImdbID);
                if (fullFilm != null)
                    film = fullFilm; // zamijeni sa punim
            }

            await Navigation.PushAsync(new FilmDetalji(film));
        }
    }
    private async void OnCityTapped(object sender, System.EventArgs e)
    {
        var cityPicker = new CityPickerPage(false);

        Application.Current.MainPage = cityPicker;

        MessagingCenter.Subscribe<CityPickerPage, string>(this, "CityChanged", (senderPage, selectedCity) =>
        {
            Preferences.Set("SelectedCity", selectedCity);

            Application.Current.MainPage = new MainTabbedPage(selectedCity);

            MessagingCenter.Unsubscribe<CityPickerPage, string>(this, "CityChanged");
        });
    }
    private async void OnReserveClicked(object sender, System.EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Film film)
        {
            await Navigation.PushAsync(new RezervacijaPage(film));
        }
    }

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
