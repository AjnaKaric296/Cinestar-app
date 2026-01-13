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
        

        _db = new UserDatabase();
    }

    private async void RegistrujSe_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(LozinkaEntry.Text))
        {
            await DisplayAlert("Greška!", "Potrebno je popuniti sva polja! Molimo popunite sva polja.", "OK");
            return;
        }

        if (LozinkaEntry.Text != PotvrdaLozinkeEntry.Text)
        {
            await DisplayAlert("Greška!", "Lozinke koje ste unijeli se ne poklapaju! Molimo pokušajte ponovo ", "OK");
            return;
        }

        var existingUser = await _db.GetUserByEmailAsync(EmailEntry.Text);
        if (existingUser != null)
        {
            await DisplayAlert("Greška!", "Korisnièki e-mail veæ postoji. Molimo pokušajte ponovo.", "OK");
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

        await DisplayAlert("", " Uspješno ste Registrovani! ", "OK");

        // direktno na profil
        await Navigation.PushAsync(new UserProfilPage());

    }
}
