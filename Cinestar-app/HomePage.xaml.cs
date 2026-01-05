using Microsoft.Maui.Controls;
namespace Cinestar_app.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        string savedCity = Preferences.Get("SelectedCity", "Izaberi grad");
        CityPickerButton.Text = savedCity;
    }

    private async void OnCittySelected(object sender, EventArgs e)
    {
        var cityPickerPage = new CityPickerPage();
        await Navigation.PushModalAsync(cityPickerPage);
    }


}