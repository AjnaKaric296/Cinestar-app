using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class MainTabbedPage : TabbedPage
{
    public MainTabbedPage(string selectedCity)
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        var homeTab = new NavigationPage(new HomePage(selectedCity))
        {
            Title = "Pocetna",
            IconImageSource = "home.png",
            BarBackgroundColor = Color.FromArgb("#051851")
        };

        Children.Add(homeTab);

        Children.Add(new NavigationPage(new Filmovi(selectedCity))
        {
            Title = "Filmovi",
            IconImageSource = "film.png",
            BarBackgroundColor = Color.FromArgb("#051851")
        });

        Children.Add(new NavigationPage(new LoyaltyBodovi())
        {
            Title = "Bodovi",
            IconImageSource = "bodovi.png",
            BarBackgroundColor = Color.FromArgb("#051851")
        });

        Children.Add(new NavigationPage(new Profil())
        {
            Title = "Profil",
            IconImageSource = "profil.png",
            BarBackgroundColor = Color.FromArgb("#051851")
        });

    
        CurrentPage = homeTab;
    }
}
