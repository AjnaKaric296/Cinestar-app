using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app;

public partial class CityPickerPage : ContentPage
{
    private readonly string[] _cities =
    {
        "Mostar", "Bihac", "Tuzla", "Banja Luka", "Zenica",
        "Sarajevo", "Prijedor", "Gracanica"
    };

    public CityPickerPage()
    {
        InitializeComponent();

        CitiesListView.ItemsSource = _cities;
        CitiesListView.ItemSelected += OnCitySelected;

        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;

        string selectedCity = e.SelectedItem.ToString();
        Preferences.Set("SelectedCity", selectedCity);
        ((ListView)sender).SelectedItem = null;

        // Idi na MainTabbedPage
        Application.Current.MainPage = new MainTabbedPage(selectedCity);
    }

}
