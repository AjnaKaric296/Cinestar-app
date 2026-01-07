using Microsoft.Maui.Controls;

namespace Cinestar_app.Pages;

public partial class Profil : ContentPage
{
    public Profil()
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
    }

   

    private async void Prijava_Clicked(object sender, EventArgs e)
    {
        if (!App.IsUserRegistered)
        {
            await DisplayAlert(
                "Prijava",
                "Nemate nalog. Molimo vas da se prvo registrujete.",
                "OK");
            return;
        }

        await DisplayAlert(
            "Prijava",
            "Uspješna prijava!",
            "OK");
    }

    private async void Registracija_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistracijaPage());
    }


}
