using Cinestar_app.Services;
using Microsoft.Maui;
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
        LoadUser();
    }

    private void LoadUser()
    {
        var user = UserSession.CurrentUser;

        if (user == null)
            return;

        ImeLabel.Text = $"Ime: {user.Ime}";
        PrezimeLabel.Text = $"Prezime: {user.Prezime}";
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        UserSession.Logout();
        await Navigation.PopToRootAsync();
    }
}
