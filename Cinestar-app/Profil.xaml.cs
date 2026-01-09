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
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void Prijava_Clicked(object sender, EventArgs e)
    {
<<<<<<< HEAD
        NavigationPage.SetHasNavigationBar(this, false);

        string email = (await DisplayPromptAsync("Email", "Unesite svoj email"))?.Trim().ToLower();
        string lozinka = (await DisplayPromptAsync(
            "Lozinka",
            "Unesite lozinku",
            "OK",
            "Cancel",
            keyboard: Keyboard.Text))?.Trim();

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

        await DisplayAlert("Uspjeh", $"Dobrodošli, {user.Ime}!", "OK");

        UserSession.Login(user);

        await Navigation.PushAsync(new UserProfilPage());
=======
        await Navigation.PushAsync(new PrijavaPage());
>>>>>>> fca82010772c9ba67bbfa1ee01072578c2bdc4bd
    }

    private async void Registracija_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistracijaPage());
    }
}
