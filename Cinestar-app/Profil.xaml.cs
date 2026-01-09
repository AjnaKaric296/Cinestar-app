using Cinestar_app.Services;
using Microsoft.Maui.Controls;

namespace Cinestar_app.Pages;

public partial class Profil : ContentPage
{
    private UserDatabase _db;
    public Profil()
    {
        InitializeComponent();
        _db = new UserDatabase();

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
