using Cinestar_app;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app
{
    public partial class CityPickerPage : ContentPage
    {
        private readonly string[] _cities =
        {
            "Mostar", "Bihac", "Tuzla","Banja Luka", "Zenica",
            "Sarajevo", "Prijedor", "Gracanica"
        };

        public CityPickerPage()
        {
            InitializeComponent();
            CitiesListView.ItemsSource = _cities;
            CitiesListView.ItemSelected += OnCitySelected;
        }

        private async void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is string selectedCity)
            {
                Preferences.Set("SelectedCity", selectedCity);
                Application.Current.MainPage = new MainTabbedPage();


            }
        }
    }
}
