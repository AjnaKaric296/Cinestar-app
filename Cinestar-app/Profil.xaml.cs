using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cinestar_app;

public partial class Profil : ContentPage
{
    private bool _showingRegistration = false;
    private bool _isLoggedIn = false;

    public Profil()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        LoadUserSession();
        UpdateUI();
    }

    private void LoadUserSession()
    {
        _isLoggedIn = Preferences.Get("IsLoggedIn", false);
    }

    private void UpdateUI()
    {
        if (_isLoggedIn)
        {
            PrijavaButton.Text = "ODJAVA";
            PrijavaButton.BackgroundColor = Colors.Red;
            RegistracijaButton.IsVisible = false;
            RegistracijaForma.IsVisible = false;
            ProfilInfo.IsVisible = true;
            StatusLabel.Text = "DOBRODOSAO!";

            string savedName = Preferences.Get("UserName", "Korisnik");
            string savedEmail = Preferences.Get("UserEmail", "email@example.com");
            ImeLabel.Text = $"Ime: {savedName}";
            EmailProfilLabel.Text = $"Email: {savedEmail}";
        }
        else
        {
            PrijavaButton.Text = "Prijavi se";
            PrijavaButton.BackgroundColor = Colors.Gold;
            RegistracijaButton.IsVisible = true;
            RegistracijaForma.IsVisible = _showingRegistration;
            ProfilInfo.IsVisible = false;
            StatusLabel.Text = "PRIJAVA";
        }
    }

    private async void Prijava_Clicked(object sender, EventArgs e)
    {
        if (_isLoggedIn)
        {
            Preferences.Set("IsLoggedIn", false);
            _isLoggedIn = false;
            UpdateUI();
            await DisplayAlert("Odjava", "Uspjesno odjavljen!", "OK");
        }
        else
        {
            string email = await DisplayPromptAsync("Email", "Unesi email:", keyboard: Keyboard.Email);
            string password = await DisplayPromptAsync("Lozinka", "Unesi lozinku:", keyboard: Keyboard.Text);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Greska", "Unesi email i lozinku!", "OK");
                return;
            }

            _isLoggedIn = true;
            Preferences.Set("IsLoggedIn", true);
            Preferences.Set("UserEmail", email);
            Preferences.Set("UserName", email.Split('@')[0]);

            StatusLabel.Text = $"DOBRODOSAO {email.Split('@')[0].ToUpper()}!";
            UpdateUI();
            await DisplayAlert("Uspjeh", $"Dobrodosao {email.Split('@')[0]}!", "OK");
        }
    }

    private void ToggleRegistracija(object sender, EventArgs e)
    {
        _showingRegistration = !_showingRegistration;
        RegistracijaForma.IsVisible = _showingRegistration;
        StatusLabel.Text = _showingRegistration ? "REGISTRACIJA" : "PRIJAVA";
    }

    private async void RegistrujSe_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ImeEntry.Text) ||
            string.IsNullOrWhiteSpace(PrezimeEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(LozinkaEntry.Text))
        {
            await DisplayAlert("Greska", "Sva polja su obavezna!", "OK");
            return;
        }

        if (LozinkaEntry.Text != PotvrdaLozinkaEntry.Text)
        {
            await DisplayAlert("Greska", "Lozinke se ne podudaraju!", "OK");
            return;
        }

        Preferences.Set("IsLoggedIn", true);
        Preferences.Set("UserName", $"{ImeEntry.Text} {PrezimeEntry.Text}");
        Preferences.Set("UserEmail", EmailEntry.Text);

        _isLoggedIn = true;
        UpdateUI();

        ImeEntry.Text = PrezimeEntry.Text = EmailEntry.Text = LozinkaEntry.Text = PotvrdaLozinkaEntry.Text = "";
        _showingRegistration = false;
        RegistracijaForma.IsVisible = false;

        await DisplayAlert("Uspjeh", "Registracija uspjesna!", "OK");
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("IsLoggedIn", false);
        _isLoggedIn = false;
        UpdateUI();
        await DisplayAlert("Odjava", "Uspjesno odjavljen!", "OK");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateUI();
    }
}
