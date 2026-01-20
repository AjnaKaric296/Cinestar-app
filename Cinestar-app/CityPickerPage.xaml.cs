using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinestar_app
{
    public partial class CityPickerPage : ContentPage
    {
        private readonly bool _isFirstLaunch;
        private readonly int _returnTabIndex;

        public CityPickerPage(bool isFirstLaunch = false, int returnTabIndex = 0)
        {
            InitializeComponent();
            _isFirstLaunch = isFirstLaunch;
            _returnTabIndex = returnTabIndex;

            CitiesListView.ItemsSource = new[]
            {
                "Mostar", "Bihac", "Tuzla", "Banja Luka", "Zenica",
                "Sarajevo", "Prijedor", "Gracanica"
            };
            CitiesListView.ItemSelected += OnCitySelected;

            NavigationPage.SetHasNavigationBar(this, false);
        }

        private List<Cinema> cinemas = new List<Cinema>
        {
            new Cinema { City = "Mostar", Latitude = 43.3438, Longitude = 17.8078 },
            new Cinema { City = "Bihac", Latitude = 44.8167, Longitude = 15.8700 },
            new Cinema { City = "Tuzla", Latitude = 44.5326, Longitude = 18.6673 },
            new Cinema { City = "Banja Luka", Latitude = 44.7722, Longitude = 17.1910 },
            new Cinema { City = "Zenica", Latitude = 44.2036, Longitude = 17.9066 },
            new Cinema { City = "Sarajevo", Latitude = 43.8563, Longitude = 18.4131 },
            new Cinema { City = "Prijedor", Latitude = 44.9815, Longitude = 16.7154 },
            new Cinema { City = "Gracanica", Latitude = 44.5333, Longitude = 18.6667 }
        };

        private async void OnUseLocationTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            if (label == null) return;

           
            var originalColor = label.TextColor;
            label.TextColor = Color.FromArgb("#AAAAAA"); 
            await Task.Delay(100); 
            label.TextColor = originalColor;

           
            var location = await GetUserLocationAsync();
            if (location != null)
            {
                var nearestCinema = GetNearestCinema(location);
                if (nearestCinema != null)
                {
                    Preferences.Set("SelectedCity", nearestCinema.City);
                    Application.Current.MainPage = new MainTabbedPage(nearestCinema.City);
                }
            }
            else
            {
                await DisplayAlert("Greška", "Ne mogu dohvatiti vašu lokaciju.", "OK");
            }
        }



     
        private async Task<Location?> GetUserLocationAsync()
        {
            try
            {
                var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                if (location != null)
                    return location;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Greška", "Ne mogu dohvatiti lokaciju: " + ex.Message, "OK");
            }
            return null;
        }

        private Cinema? GetNearestCinema(Location userLocation)
        {
            return cinemas
                .OrderBy(c => Location.CalculateDistance(userLocation, new Location(c.Latitude, c.Longitude), DistanceUnits.Kilometers))
                .FirstOrDefault();
        }

        private void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            string selectedCity = e.SelectedItem.ToString();
            Preferences.Set("SelectedCity", selectedCity);

            ((ListView)sender).SelectedItem = null;

            if (_isFirstLaunch)
            {
                Application.Current.MainPage = new MainTabbedPage(selectedCity);
            }
            else
            {
                Application.Current.MainPage = new MainTabbedPage(selectedCity)
                {
                    CurrentPage = Application.Current.MainPage is MainTabbedPage main ? main.Children[_returnTabIndex] : null
                };
            }
        }
    }

    public class Cinema
    {
        public string City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
