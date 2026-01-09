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
        // 1️⃣ UNOS EMAILA
        string email = await DisplayPromptAsync(
            "Email",
            "Unesite svoj email",
            "OK",
            "Cancel",
            keyboard: Keyboard.Email

        );

        // ❌ Ako je Cancel ili prazno → odmah prekini, nema lozinke
        if (string.IsNullOrWhiteSpace(email))
            return;

        email = email.Trim().ToLower();

        // 2️⃣ PROVJERA DA LI EMAIL POSTOJI
        var user = await _db.GetUserByEmailAsync(email);

        if (user == null)
        {
            await DisplayAlert("Greška", "Ne postoji korisnik sa tim emailom", "OK");
            return; // ❌ nema lozinke
        }

        // 3️⃣ UNOS LOZINKE (SAMO AKO EMAIL POSTOJI)
        string lozinka = await DisplayPromptAsync(
            "Lozinka",
            "Unesite lozinku",
            "OK",
            "Cancel",
            keyboard: Keyboard.Text
        );

        // ❌ Ako je Cancel ili prazno
        if (string.IsNullOrWhiteSpace(lozinka))
            return;

        lozinka = lozinka.Trim();

        // 4️⃣ PROVJERA LOZINKE
        if (user.Lozinka.Trim() != lozinka)
        {
            await DisplayAlert("Greška", "Lozinka nije tačna", "OK");
            return;
        }

        // 5️⃣ USPJEŠNA PRIJAVA
        await DisplayAlert("Uspjeh", $"Dobrodošli, {user.Ime}!", "OK");

        UserSession.Login(user);

        await Navigation.PushAsync(new UserProfilPage());
    }



    private async void Registracija_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistracijaPage());
    }


}
