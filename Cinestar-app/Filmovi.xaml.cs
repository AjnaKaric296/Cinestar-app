using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
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
                _ = LoadFilmsWithOverlay(); // automatski refresh kada se grad promijeni
            }
        }
    }

    public ObservableCollection<Film> Films { get; set; } = new();

    private Dictionary<string, string[]> cityQueries = new()
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

        // Subscribe na promjenu grada iz CityPickerPage
        MessagingCenter.Subscribe<CityPickerPage, string>(this, "CityChanged", (sender, newCity) =>
        {
            if (newCity != SelectedCity)
            {
                SelectedCity = newCity; // ovo automatski poziva LoadFilmsWithOverlay
            }
        });

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async Task LoadFilmsWithOverlay()
    {
        LoadingOverlay.IsVisible = true;
        await Task.Delay(50);
        await LoadFilms();
        LoadingOverlay.IsVisible = false;
    }

    private async Task LoadFilms()
    {
        Films.Clear();

        if (!cityQueries.ContainsKey(SelectedCity))
            return;

        var allFilmsTemp = new List<Film>();

        foreach (var query in cityQueries[SelectedCity])
        {
            var results = await omdbService.SearchMoviesAsync(query);
            foreach (var r in results)
            {
                var d = await omdbService.GetMovieDetailsAsync(r.imdbID);
                if (d == null) continue;

                allFilmsTemp.Add(new Film
                {
                    Title = d.Title,
                    Year = d.Year,
                    Genre = d.Genre,
                    Plot = d.Plot,
                    Poster = d.Poster == "N/A" ? "placeholder.png" : d.Poster,
                    Showtimes = new() { "12:00", "15:00", "18:00" }
                });

                if (allFilmsTemp.Count >= 20) break;
            }
            if (allFilmsTemp.Count >= 20) break;
        }

        foreach (var f in allFilmsTemp)
            Films.Add(f);

        // Zanrovi za picker
        var genres = Films
            .SelectMany(f => (f.Genre ?? "").Split(','))
            .Select(g => g.Trim())
            .Distinct()
            .ToList();
        genres.Insert(0, "Svi");
        GenrePicker.ItemsSource = genres;
        GenrePicker.SelectedIndex = 0;
    }

    private void OnGenreChanged(object sender, System.EventArgs e)
    {
        var g = GenrePicker.SelectedItem?.ToString();
        if (g == "Svi")
            FilmsCollectionView.ItemsSource = Films;
        else
            FilmsCollectionView.ItemsSource = new ObservableCollection<Film>(Films.Where(f => f.Genre.Contains(g)));
    }

    private async void OnFilmTapped(object sender, System.EventArgs e)
    {
        if ((sender as Frame)?.BindingContext is Film film)
        {
            await Navigation.PushAsync(new FilmDetalji(film));
        }
    }

    private async void OnCityTapped(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new CityPickerPage(false));
    }

    private async void OnReserveClicked(object sender, System.EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Film film)
        {
            string selectedTime = (sender as Button).Text;
            await DisplayAlert("Rezervacija",
                $"Film: {film.Title}\nTermin: {selectedTime}",
                "OK");
        }
    }

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
