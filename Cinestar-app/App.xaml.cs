using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Startaj sa SplashPage
        MainPage = new SplashPage();
    }
}
