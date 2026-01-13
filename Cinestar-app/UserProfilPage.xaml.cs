using Cinestar_app.Services;
using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class UserProfilPage : ContentPage
{
    public UserProfilPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (UserSession.CurrentUser != null)
            BindingContext = UserSession.CurrentUser;
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool potvrda = await DisplayAlert(
        "Odjava",
        "Da li ste sigurni da želite da se odjavite?",
        "Da",
        "Ne"
    );

        if (potvrda)
        {
          
            UserSession.Logout();

            Microsoft.Maui.Storage.Preferences.Remove("LoggedInEmail");

            await Navigation.PopToRootAsync();
        }

    }

    private async void LoyaltyBodovi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoyaltyBodovi());
    }

    private async void Podaci_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PodaciORacunu());
    }

    private async void MojeRezervacije_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MojeRezervacije());
    }

}
