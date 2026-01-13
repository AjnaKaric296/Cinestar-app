
using Microsoft.Maui.Controls;
﻿using Cinestar_app.Services;
namespace Cinestar_app;

public partial class LoyaltyBodovi : ContentPage
{
    private UserDatabase _db;
    public LoyaltyBodovi()
    {
        InitializeComponent();
        _db=new UserDatabase();
        NavigationPage.SetHasNavigationBar(this, false);

    }
    private async void OnPrijavaButtonClicked(object sender, EventArgs e)
    {
        Navigation.InsertPageBefore(new Profil(), this);
        await Navigation.PopAsync();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (UserSession.IsLoggedIn)
        {
            NonLoggedInLayout.IsVisible = false;
            LoggedInLayout.IsVisible = true;

            // Dohvati bodove iz baze i prikazi u Label
            var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
            BodoviLabel.Text = loyalty?.Bodovi.ToString() ?? "0";

            PozdravLabel.Text = $"Dobrodošao/la, {UserSession.CurrentUser.Ime}!";
        }
        else
        {
            NonLoggedInLayout.IsVisible = true;
            LoggedInLayout.IsVisible = false;
        }
    }

    private async void OnViewPointsClicked(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn) return;

        var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
        await DisplayAlert("Tvoje zvjezdice", $"Trenutno imaš {loyalty?.Bodovi ?? 0} zvjezdica ", "OK");
    }


}
