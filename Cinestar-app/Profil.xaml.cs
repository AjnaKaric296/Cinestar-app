using Cinestar_app.Services;
using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class Profil : ContentPage
{
    private UserDatabase _db;

    public Profil()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void Prijava_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PrijavaPage());
    }

    private async void Registracija_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistracijaPage());
    }
}
