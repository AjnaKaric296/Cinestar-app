using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class MainTabbedPage : TabbedPage
{
    public HomePage Home { get; private set; }
    public Filmovi Filmovi { get; private set; }
    public MainTabbedPage(string selectedCity)
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        Home = new HomePage(selectedCity);
        Filmovi = new Filmovi(selectedCity);

        Children.Add(Home);
        Children.Add(Filmovi);
    }

}
