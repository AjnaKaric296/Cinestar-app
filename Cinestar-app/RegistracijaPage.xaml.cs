using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Cinestar_app;

public partial class RegistracijaPage : ContentPage
{
    public RegistracijaPage()
    {
        InitializeComponent();
    }

    private async void RegistrujSe_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(LozinkaEntry.Text))
        {
            await DisplayAlert("Greška", "Sva polja su obavezna", "OK");
            return;
        }

        if (LozinkaEntry.Text != PotvrdaLozinkeEntry.Text)
        {
            await DisplayAlert("Greška", "Lozinke se ne poklapaju", "OK");
            return;
        }

        // OZNAÈAVAMO DA JE KORISNIK REGISTROVAN
        App.IsUserRegistered = true;

        await DisplayAlert("Uspjeh", "Registracija uspješna!", "OK");

        // Povratak na Profil
        await Navigation.PopAsync();
    }
}
