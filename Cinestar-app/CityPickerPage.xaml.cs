using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app;

public partial class CityPickerPage : ContentPage
{
    private readonly bool _isFirstLaunch;

    // dodaj novi parametar: tabIndex na koji se vraća
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

    private void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;

        string selectedCity = e.SelectedItem.ToString();
        Preferences.Set("SelectedCity", selectedCity);
        MessagingCenter.Send(this, "CityChanged", selectedCity);

        ((ListView)sender).SelectedItem = null;

        if (_isFirstLaunch)
        {
            Application.Current.MainPage = new MainTabbedPage(selectedCity);
        }
        else
        {
            // vraćamo MainTabbedPage i postavljamo pravi tab
            Application.Current.MainPage = new MainTabbedPage(selectedCity)
            {
                CurrentPage = Application.Current.MainPage is MainTabbedPage main ? main.Children[_returnTabIndex] : null
            };
        }
    }
}
