using Microsoft.Maui.Controls;

namespace Cinestar_app.Pages;

public partial class Profil : ContentPage
{
    public Profil()
    {
        InitializeComponent();
    }

    private async void Prijava_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Prijava",
            "Nemate nalog. Molimo vas da se prvo registrujete.",
            "OK");
    }

    private void Registracija_Clicked(object sender, EventArgs e)
    {
        RegistracijaForma.IsVisible = !RegistracijaForma.IsVisible;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}
