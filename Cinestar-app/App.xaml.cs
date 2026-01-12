using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Dohvati prethodno odabrani grad
        var city = Preferences.Get("SelectedCity", null);

        if (string.IsNullOrEmpty(city))
        {
            // Prvo paljenje: CityPickerPage kao root unutar NavigationPage
            MainPage = new NavigationPage(new CityPickerPage(true));
        }
        else
        {
            // Ako je grad već odabran, odmah TabbedPage
            MainPage = new MainTabbedPage(city);
        }
    }
}
