using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;

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
    }

    private void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;

        string selectedCity = e.SelectedItem.ToString();
        Preferences.Set("SelectedCity", selectedCity);

        // Ovdje otvaramo MainTabbedPage sa navigation barom
        ((App)Application.Current).OpenHomePage();
    }
}
