using Microsoft.Maui.Controls;
using Cinestar_app.Services;

namespace Cinestar_app;

public partial class PodaciORacunu : ContentPage
{
    public PodaciORacunu()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Postavi BindingContext na trenutnog korisnika
        if (UserSession.CurrentUser != null)
        {
            BindingContext = UserSession.CurrentUser;
        }
    }

    private async void ChangePassword_Clicked(object sender, EventArgs e)
    {
        string oldPassword = await DisplayPromptAsync("Promijeni lozinku", "Unesite staru lozinku:", "Dalje", "Otkaži", placeholder: "Stara lozinka", maxLength: 50, keyboard: Keyboard.Text);
        if (string.IsNullOrWhiteSpace(oldPassword)) return;

        if (oldPassword != UserSession.CurrentUser.Lozinka)
        {
            await DisplayAlert("Greška", "Pogrešna stara lozinka!", "OK");
            return;
        }

        string newPassword = await DisplayPromptAsync("Promijeni lozinku", "Unesite novu lozinku:", "Dalje", "Otkaži", placeholder: "Nova lozinka", maxLength: 50, keyboard: Keyboard.Text);
        if (string.IsNullOrWhiteSpace(newPassword)) return;

        string confirmPassword = await DisplayPromptAsync("Promijeni lozinku", "Ponovite novu lozinku:", "Dalje", "Otkaži", placeholder: "Ponovi novu lozinku", maxLength: 50, keyboard: Keyboard.Text);
        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Greška", "Lozinke se ne poklapaju!", "OK");
            return;
        }

        var db = new UserDatabase();
        UserSession.CurrentUser.Lozinka = newPassword;
        await db.UpdateUserPasswordAsync(UserSession.CurrentUser.Email, newPassword);

        await DisplayAlert("Uspješno", "Lozinka je promijenjena!", "OK");
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
