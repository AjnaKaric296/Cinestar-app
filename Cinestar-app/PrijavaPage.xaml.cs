using Microsoft.Maui.Controls;
using Cinestar_app.Services;
using Cinestar_app.Models;
using System;

namespace Cinestar_app.Pages
{
    public partial class PrijavaPage : ContentPage
    {
        private UserDatabase _db;

        public PrijavaPage()
        {
            InitializeComponent();
            _db = new UserDatabase();
        }

        private async void PrijaviSe_Clicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim().ToLower();
            string lozinka = LozinkaEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(lozinka))
            {
                await DisplayAlert("Greška", "Molimo popunite sva polja", "OK");
                return;
            }

            var user = await _db.GetUserByEmailAsync(email);

            if (user == null)
            {
                await DisplayAlert("Greška", "Ne postoji korisnik sa tim emailom", "OK");
                return;
            }

            if (user.Lozinka != lozinka)
            {
                await DisplayAlert("Greška", "Lozinka nije taèna", "OK");
                return;
            }

            // Uspješna prijava
            UserSession.Login(user);
            await DisplayAlert("Uspjeh", $"Dobrodošli, {user.Ime}!", "OK");

            await Navigation.PushAsync(new UserProfilPage());
        }
    }
}