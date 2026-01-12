using Cinestar_app.Services;
using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class Profil : ContentPage
{
    private readonly UserDatabase _db = new UserDatabase();

    public Profil()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void Prijava_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim().ToLower();
        string lozinka = LozinkaEntry.Text?.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(lozinka))
        {
            await DisplayAlert("Greška", "Morate unijeti email i lozinku", "OK");
            return;
        }

        var user = await _db.GetUserByEmailAsync(email);

        if (user == null)
        {
            await DisplayAlert("Greška", "Email nije registrovan", "OK");
            return;
        }

        if (user.Lozinka.Trim() != lozinka)
        {
            await DisplayAlert("Greška", "Lozinka nije tačna", "OK");
            return;
        }

        UserSession.Login(user);
        await DisplayAlert("Uspjeh", $"Dobrodošli, {user.Ime}!", "OK");

        await Navigation.PushAsync(new UserProfilPage());
    }

    private async void Registracija_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistracijaPage());
    }
}
