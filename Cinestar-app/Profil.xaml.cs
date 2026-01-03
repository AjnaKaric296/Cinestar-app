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
        string email = await DisplayPromptAsync("Email", "Unesite svoj email");
        string lozinka = await DisplayPromptAsync("Lozinka", "Unesite lozinku", "OK", "Cancel", keyboard: Keyboard.Text);

        var user = await _db.GetUserByEmailAsync(email);

        if (user == null || user.Lozinka != lozinka)
        {
            await DisplayAlert("Greška", "Email ili lozinka nisu taèni", "OK");
            return;
        }

        await DisplayAlert("Uspjeh", $"Dobrodošli, {user.Ime}!", "OK");

        // Ovdje možeš navigirati dalje u app
    }

    private async void Registracija_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistracijaPage());
    }
}
