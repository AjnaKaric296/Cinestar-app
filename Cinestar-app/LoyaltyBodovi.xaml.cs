
using Microsoft.Maui.Controls;
﻿using Cinestar_app.Services;
namespace Cinestar_app;

public partial class LoyaltyBodovi : ContentPage
{


    private UserDatabase _db;

    public LoyaltyBodovi()
    {
        InitializeComponent();
        _db = new UserDatabase();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (UserSession.IsLoggedIn)
        {
            GuestView.IsVisible = false;
            UserView.IsVisible = true;

            var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
            BodoviLabel.Text = loyalty?.Bodovi.ToString() ?? "0";
        }
        else
        {
            GuestView.IsVisible = true;
            UserView.IsVisible = false;
        }
    }

    private async void BuyTickets_Clicked(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn)
            return;

        await _db.AddPointsAsync(UserSession.CurrentUser.Email, 10);

        var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
        BodoviLabel.Text = loyalty?.Bodovi.ToString() ?? "0";

        await DisplayAlert("Bravo 🎉", "Dobili ste 10 loyalty bodova!", "OK");
    }

    private async void ReviewMovie_Clicked(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn)
            return;

        await _db.AddPointsAsync(UserSession.CurrentUser.Email, 5);

        var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
        BodoviLabel.Text = loyalty?.Bodovi.ToString() ?? "0";

        await DisplayAlert("Hvala ⭐", "Dobili ste 5 loyalty bodova!", "OK");
    }
}

