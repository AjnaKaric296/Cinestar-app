using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app;

public partial class CityPickerPage : ContentPage
{
    private readonly bool _isFirstLaunch;
    private readonly string[] _cities =
    {
        "Mostar", "Bihac", "Tuzla", "Banja Luka", "Zenica",
        "Sarajevo", "Prijedor", "Gracanica"
    };

    public CityPickerPage(bool isFirstLaunch = false)
    {
        InitializeComponent();
        _isFirstLaunch = isFirstLaunch;

        CitiesListView.ItemsSource = _cities;
        CitiesListView.ItemSelected += OnCitySelected;

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;

        string selectedCity = e.SelectedItem.ToString();
        Preferences.Set("SelectedCity", selectedCity);

        // 🔹 Pošalji signal svim subscriber-ima
        MessagingCenter.Send(this, "CityChanged", selectedCity);

        ((ListView)sender).SelectedItem = null;

        if (_isFirstLaunch)
        {
            Application.Current.MainPage = new MainTabbedPage(selectedCity);
        }
        else
        {
            await Navigation.PopAsync();
        }
    }
}
