using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Cinestar_app;

public partial class Filmovi : ContentPage, INotifyPropertyChanged
{
    public ObservableCollection<Movie> Movies { get; set; } = new();
    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    private string _currentCity = "Sarajevo";
    public string CurrentCity
    {
        get => _currentCity;
        set
        {
            _currentCity = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }

    public Filmovi()
    {
        InitializeComponent();
        BindingContext = this;
        NavigationPage.SetHasNavigationBar(this, false);

        RefreshCommand = new Command(async () => await LoadMovies());
        _ = LoadMovies();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        FilmoviCollection.ItemsSource = Movies;
        CurrentCity = Preferences.Get("SelectedCity", "Sarajevo");
        await LoadMovies();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadMovies();
    }

    private async Task LoadMovies()
    {
        IsRefreshing = true;
        try
        {
            CurrentCity = Preferences.Get("SelectedCity", "Sarajevo");
            System.Diagnostics.Debug.WriteLine($"🌍 Filmovi - GRAD: {CurrentCity}");

            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            Movies.Clear();

            string[] cityGenres = GetCityGenres(CurrentCity);

            foreach (string genre in cityGenres)
            {
                try
                {
                    // 1. SEARCH filmovi
                    string searchUrl = $"http://www.omdbapi.com/?s={Uri.EscapeDataString(genre)}&apikey=75ace56d";
                    System.Diagnostics.Debug.WriteLine($"🔍 SEARCH: {searchUrl}");

                    var searchJson = await http.GetStringAsync(searchUrl);
                    var searchData = JsonSerializer.Deserialize<JsonElement>(searchJson);

                    if (searchData.TryGetProperty("Response", out var response) &&
                        response.GetString() == "True" &&
                        searchData.TryGetProperty("Search", out var search))
                    {
                        foreach (var movieElement in search.EnumerateArray().Take(2))
                        {
                            var imdbId = movieElement.GetProperty("imdbID").GetString() ?? "";

                            // 2. Base movie data
                            var newMovie = new Movie
                            {
                                Title = movieElement.GetProperty("Title").GetString() ?? "",
                                Poster = movieElement.GetProperty("Poster").GetString() ?? "",
                                Year = movieElement.GetProperty("Year").GetString() ?? "",
                                ImdbID = imdbId
                            };

                            // 3. DETAILS za Plot + Rating
                            try
                            {
                                string detailsUrl = $"http://www.omdbapi.com/?i={imdbId}&apikey=75ace56d&plot=short";
                                System.Diagnostics.Debug.WriteLine($"📋 DETAILS: {detailsUrl}");

                                var detailsJson = await http.GetStringAsync(detailsUrl);
                                var detailsData = JsonSerializer.Deserialize<JsonElement>(detailsJson);

                                newMovie.Plot = detailsData.GetProperty("Plot").GetString() ?? "Nema opisa...";
                                newMovie.ImdbRating = detailsData.GetProperty("imdbRating").GetString() ?? "N/A";
                            }
                            catch (Exception detailsEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Details error: {detailsEx.Message}");
                                newMovie.Plot = "Nema detalja dostupnih...";
                                newMovie.ImdbRating = "N/A";
                            }

                            Movies.Add(newMovie);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ {genre}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"💥 LoadMovies: {ex.Message}");
            Movies.Clear();
            Movies.Add(new Movie
            {
                Title = "Greška pri učitavanju",
                Poster = "https://picsum.photos/200/300",
                Plot = "Provjeri internet vezu i pokušaj ponovo."
            });
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private string[] GetCityGenres(string city)
    {
        return city switch
        {
            "Sarajevo" => new[] { "action 2023", "drama 2023" },
            "Banja Luka" => new[] { "thriller 2023", "crime" },
            "Mostar" => new[] { "comedy 2023", "romance" },
            "Tuzla" => new[] { "horror 2023", "fantasy" },
            "Zenica" => new[] { "sci-fi 2023", "adventure" },
            "Bihać" => new[] { "animation 2023", "family" },
            "Prijedor" => new[] { "top 2023", "popular" },
            "Gračanica" => new[] { "music", "documentary" },
            _ => new[] { "top 2023" }
        };
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
