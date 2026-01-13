using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    int currentRating = 0;

    public FilmDetalji(Film film)
    {
        InitializeComponent();
        BindingContext = film;
        RatingFrame.IsVisible = UserSession.IsLoggedIn;

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

        ActorsCollectionView.ItemsSource = film.Actors; // ovo je ključno
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

        AddUserPoints(rating);
    }


    void AddUserPoints(int rating)
    {
        var user = UserSession.CurrentUser;
        if (user == null) return;

        // Dodaj bodove u user objekt
        user.LoyaltyPoints += 1; // 1 bod po ocjeni

        // Snimi u bazu
        var db = new UserDatabase();
        _ = db.UpdateUserLoyaltyAsync(user.Email, user.LoyaltyPoints); // asinhrono, ne blokira
    }

    private bool ratingConfirmed = false;

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
            AddUserPoints(currentRating); // update baze i user objekt
            ratingConfirmed = true;

            await DisplayAlert("Hvala!", $"Ocijenili ste film {currentRating}/5 ⭐ i osvojili bodove!", "OK");

            // Ako je LoyaltyBodovi trenutno otvorena, možeš odmah osvježiti labelu
            MessagingCenter.Send(this, "UpdateLoyaltyPoints");
        }
        else
        {
            await DisplayAlert("Već ocijenjeno", "Već ste ocijenili ovaj film.", "OK");
        }
    }

}





