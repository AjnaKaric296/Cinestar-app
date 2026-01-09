using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class CityPickerPage : ContentPage
{
    public CityPickerPage()
    {
        InitializeComponent();

        CitiesListView.ItemsSource = new string[]
        {
        "Sarajevo", "Banja Luka", "Mostar", "Tuzla",
        "Zenica", "Bihać", "Prijedor", "Gračanica"
        };
    }


    private async void OnCitySelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is string selectedCity)
        {
            Preferences.Set("SelectedCity", selectedCity);

            await Navigation.PopModalAsync();
        }
    }

}
