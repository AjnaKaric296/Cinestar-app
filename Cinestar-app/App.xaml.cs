using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var city = Preferences.Get("SelectedCity", null);

        if (string.IsNullOrEmpty(city))
        {
            MainPage = new NavigationPage(new CityPickerPage(true));
        }
        else
        {
           
            MainPage = new MainTabbedPage(city);
        }
    }
}
