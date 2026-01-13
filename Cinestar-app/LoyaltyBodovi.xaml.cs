
using System.Collections.ObjectModel;
using Cinestar_app.Models;
﻿using Cinestar_app.Services;
using Microsoft.Maui.Controls;
namespace Cinestar_app;

public partial class LoyaltyBodovi : ContentPage
{
    private UserDatabase _db;
    public ObservableCollection<Reward> Rewards { get; set; }
    public LoyaltyBodovi()
    {
        InitializeComponent();
        Rewards = new ObservableCollection<Reward>
        {
            new Reward { Name = "Besplatne kokice", Image = "kokica.png", StarsRequired = 50 },
            new Reward { Name = "Popust na piće", Image = "sokk.png", StarsRequired = 30 },
            new Reward { Name = "Ulaznica za film", Image = "karte.png", StarsRequired = 100 },
            new Reward { Name = "VIP Zona", Image = "vip.png", StarsRequired = 120 },
        };

        BindingContext = this;

        _db =new UserDatabase();
        NavigationPage.SetHasNavigationBar(this, false);

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

            MessagingCenter.Subscribe<FilmDetalji>(this, "UpdateLoyaltyPoints", (sender) =>
            {
                UpdatePointsLabel();
            });
        }
        else
        {
            NonLoggedInLayout.IsVisible = true;
            LoggedInLayout.IsVisible = false;
        }
    }

    private async void UpdatePointsLabel()
    {
        var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
        BodoviLabel.Text = loyalty?.Bodovi.ToString() ?? "0";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<FilmDetalji>(this, "UpdateLoyaltyPoints");
    }

    private async void OnViewPointsClicked(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn) return;

        var loyalty = await _db.GetLoyaltyAsync(UserSession.CurrentUser.Email);
        await DisplayAlert("Tvoje zvjezdice", $"Trenutno imaš {loyalty?.Bodovi ?? 0} zvjezdica", "OK");
    }


}
