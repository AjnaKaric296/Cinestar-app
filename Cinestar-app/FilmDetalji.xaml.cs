using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    int currentRating = 0;
    
    private bool ratingConfirmed = false;
    private UserDatabase _db;

    public FilmDetalji(Film film)
    {
        InitializeComponent();
        BindingContext = film;
        RatingFrame.IsVisible = UserSession.IsLoggedIn;

       

        _db = new UserDatabase();

        if (UserSession.IsLoggedIn)
        {
            AddStarTap(Star1, 1);
            AddStarTap(Star2, 2);
            AddStarTap(Star3, 3);
            AddStarTap(Star4, 4);
            AddStarTap(Star5, 5);
        }

        PosterImage.Source = film.Poster;
        TitleLabel.Text = film.Title;
        PlotLabel.Text = film.Plot;

        // --- RANDOM GLUMCI IZ LOKALNE BAZE ---
        var random = new Random();
        var actors = new List<Actor>();
        var allActors = Cinestar_app.Data.ActorsDatabase.AllActors;

        while (actors.Count < 3)
        {
            var candidate = allActors[random.Next(allActors.Count)];
            if (!actors.Contains(candidate))
                actors.Add(candidate);
        }

        ActorsCollectionView.ItemsSource = actors;
    }

    void AddStarTap(Image star, int value)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => SetRating(value);
        star.GestureRecognizers.Add(tap);
    }

    void SetRating(int rating)
    {
        if (!UserSession.IsLoggedIn)
        {
            DisplayAlert("Prijava potrebna",
                         "Prijavi se ili registruj da bi ocijenio film 🎬",
                         "OK");
            return;
        }

        currentRating = rating;

        Star1.Source = rating >= 1 ? "star_filled.png" : "star_empty.png";
        Star2.Source = rating >= 2 ? "star_filled.png" : "star_empty.png";
        Star3.Source = rating >= 3 ? "star_filled.png" : "star_empty.png";
        Star4.Source = rating >= 4 ? "star_filled.png" : "star_empty.png";
        Star5.Source = rating >= 5 ? "star_filled.png" : "star_empty.png";

        RatingInfoLabel.Text = $"Dao/la si ocjenu {rating}/5 ⭐";
    }

    private async Task AddUserPoints()
    {
        if (UserSession.CurrentUser == null) return;

        // 1️⃣ povećaš GLOBALNE bodove
        UserSession.LoyaltyPoints += 1;

        // 2️⃣ snimiš u bazu
        await _db.UpdateUserLoyaltyAsync(
            UserSession.CurrentUser.Email,
            UserSession.LoyaltyPoints
        );

        // 3️⃣ obavijesti ostale stranice (opcionalno)
        Device.BeginInvokeOnMainThread(() =>
        {
            MessagingCenter.Send(this, "UpdateLoyaltyPoints");
        });
    }


    private async void OnConfirmRatingClicked(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn)
        {
            await DisplayAlert("Prijava potrebna",
                "Prijavi se ili registruj da bi ocijenio film 🎬",
                "OK");
            return;
        }

        if (currentRating == 0)
        {
            await DisplayAlert("Nije ocijenjeno",
                "Molimo odaberite broj zvjezdica prije potvrde.",
                "OK");
            return;
        }

        if (!ratingConfirmed)
        {
            await AddUserPoints();

            ratingConfirmed = true;

            await DisplayAlert(
                "Hvala!",
                $"Ocijenili ste film {currentRating}/5 ⭐ i osvojili bod! 🎉\nTrenutno imate {UserSession.LoyaltyPoints} bodova.",
                "OK");

        }
        else
        {
            await DisplayAlert("Već ocijenjeno", "Već ste ocijenili ovaj film.", "OK");
        }
    }
    private async void OnSeeAllActorsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GlumciPage());
    }

}
