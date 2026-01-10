using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // SplashPage je startna stranica
        MainPage = new SplashPage();
    }

    public void OpenHomePage()
    {
        // MainTabbedPage uvijek sa navigation barom
        MainPage = new MainTabbedPage();
    }
}
