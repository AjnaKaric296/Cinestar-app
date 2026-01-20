namespace Cinestar_app;

public partial class PodaciORacunu : ContentPage
{
    public PodaciORacunu()
    {
        InitializeComponent();
        UcitajPodatkeKorisnika();
    }

    private void UcitajPodatkeKorisnika()
    {
        // Ovdje ubaciš podatke prijavljenog korisnika
        string ime = "Aida";
        string prezime = "Begagic";
        string email = "aida@example.com";
        int loyaltyBodovi = 120;
        DateTime datumRegistracije = new DateTime(2023, 5, 10);
        string profilnaSlika = "profil.png"; // lokalna slika u Resources/Images

        // Postavljanje podataka u UI
        ImePrezimeLabel.Text = $"{ime} {prezime}";
        EmailLabel.Text = email;
        BodoviLabel.Text = $"Loyalty bodovi: {loyaltyBodovi}";
        DatumLabel.Text = $"Pridružio se: {datumRegistracije:dd.MM.yyyy}";
        ProfilImage.Source = profilnaSlika;
    }

    private void UrediProfil_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Uredi profil", "Ovdje ide logika za ureðivanje profila", "OK");
    }

    private void Odjava_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Odjava", "Ovdje ide logika za odjavu", "OK");
    }
}