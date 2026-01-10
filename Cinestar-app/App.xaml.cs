using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Start app sa SplashPage
        MainPage = new SplashPage();
    }
}
