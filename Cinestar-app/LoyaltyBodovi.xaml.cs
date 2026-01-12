
using Microsoft.Maui.Controls;
﻿using Cinestar_app.Services;
namespace Cinestar_app;

public partial class LoyaltyBodovi : ContentPage
{

    public LoyaltyBodovi()
    {
        InitializeComponent();

    }
    private async void OnPrijavaButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Profil());
    }
}
