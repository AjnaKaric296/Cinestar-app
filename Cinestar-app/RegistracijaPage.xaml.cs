using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Cinestar_app.Models;
using Cinestar_app.Services;

namespace Cinestar_app;

public partial class RegistracijaPage : ContentPage
{
    private UserDatabase _db;

    public RegistracijaPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        _db = new UserDatabase();
    }

    private async void RegistrujSe_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(LozinkaEntry.Text))
        {
            await DisplayAlert("Greska", "Sva polja su obavezna", "OK");
            return;
        }

        if (LozinkaEntry.Text != PotvrdaLozinkeEntry.Text)
        {
            await DisplayAlert("Greska", "Lozinke se ne poklapaju", "OK");
            return;
        }

        var existingUser = await _db.GetUserByEmailAsync(EmailEntry.Text);
        if (existingUser != null)
        {
            await DisplayAlert("Greska", "Korisnik s tim emailom veæ postoji", "OK");
            return;
        }

        var user = new User
        {
            Ime = ImeEntry.Text,
            Prezime = PrezimeEntry.Text,
            Email = EmailEntry.Text,
            Lozinka = LozinkaEntry.Text
        };

        await _db.AddUserAsync(user);

        // automatski login
        UserSession.Login(user);

        await DisplayAlert("Uspjeh", "Registracija uspjesna!", "OK");

        // direktno na profil
        await Navigation.PushAsync(new UserProfilPage());

    }
}
