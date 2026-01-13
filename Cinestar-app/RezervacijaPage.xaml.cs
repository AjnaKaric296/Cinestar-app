using Cinestar_app.Models;
using Cinestar_app.Services;

using System;
using System.Collections.Generic;

namespace Cinestar_app;

public partial class RezervacijaPage : ContentPage
{
    private Film _film;
    private FilmService _filmService;

    public RezervacijaPage(Film film)
    {
        InitializeComponent();

        _film = film;
        _filmService = new FilmService();

        // Postavi film podatke
        PosterImage.Source = _film.Poster;
        FilmTitleLabel.Text = _film.Title;

        // Postavi default vrijednosti
        TicketStepper.ValueChanged += TicketStepper_ValueChanged;
        TicketCountLabel.Text = ((int)TicketStepper.Value).ToString();

        // Dodaj dummy termine (kasnije se mogu dohvatiti iz API-ja)
        TimePicker.ItemsSource = new List<string> { "16:30", "18:00", "20:15", "22:00" };
        TimePicker.SelectedIndex = 0;
    }

    private void TicketStepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        TicketCountLabel.Text = ((int)e.NewValue).ToString();
    }


    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        var userDb = new UserDatabase();

        // Dohvati trenutno prijavljenog korisnika (Preferences)
        var email = Microsoft.Maui.Storage.Preferences.Get("LoggedInEmail", string.Empty);
        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Greška", "Morate se prijaviti da biste rezervirali kartu.", "OK");
            return;
        }

        var reservation = new Reservation
        {
            UserEmail = email,
            FilmTitle = _film.Title,
            FilmId = _film.ImdbID,
            Date = DatePicker.Date,
            Time = TimePicker.SelectedItem?.ToString() ?? "",
            TicketCount = (int)TicketStepper.Value
        };

        await userDb.AddReservationAsync(reservation);

        await DisplayAlert("Uspješno", "Rezervacija je spremljena na vaš profil.", "OK");
        await Navigation.PopAsync();
    }

}
