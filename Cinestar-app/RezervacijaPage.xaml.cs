using Cinestar_app.Models;
using Cinestar_app.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

using System;
using System.Collections.Generic;

namespace Cinestar_app;

public partial class RezervacijaPage : ContentPage
{
    private Film _film;
    private FilmService _filmService;

    public ObservableCollection<Seat> Seats { get; set; }
    public ICommand SeatTappedCommand { get; }
    private int MaxSelectedSeats => (int)TicketStepper.Value;



    public RezervacijaPage(Film film)
    { 
        InitializeComponent();
        
        Seats = new ObservableCollection<Seat>();

        // npr. 20 sjedala
        for (int i = 0; i < 24; i++)
        {
            Seats.Add(new Seat
            {
                Image = "seat.png",
                IsSelected = false
            });
        }

        // command za klik
        SeatTappedCommand = new Command<Seat>(OnSeatTapped);

        BindingContext = this;

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

        int allowed = (int)e.NewValue;

        var selectedSeats = Seats.Where(s => s.IsSelected).ToList();

        while (selectedSeats.Count > allowed)
        {
            var seat = selectedSeats.Last();
            seat.IsSelected = false;
            seat.Image = "seat.png";
            selectedSeats.Remove(seat);
        }
    }



    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        var userDb = new UserDatabase();

        // Dohvati trenutno prijavljenog korisnika (Preferences)
        if (!UserSession.IsLoggedIn || UserSession.CurrentUser == null)
        {
            await DisplayAlert("Greška",
                "Morate se prijaviti da biste rezervirali kartu.",
                "OK");
            return;
        }

        var email = UserSession.CurrentUser.Email;


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
        // --- DODAJ 2 BODA KORISNIKU ---
        UserSession.LoyaltyPoints += 2;
        await userDb.UpdateUserLoyaltyAsync(email, UserSession.LoyaltyPoints);

        // --- Obavijesti LoyaltyBodovi stranicu da osvježi bodove ---
        Device.BeginInvokeOnMainThread(() =>
        {
            MessagingCenter.Send(this, "UpdateLoyaltyPoints");
        });

        await DisplayAlert("Uspjesno",
            $"Rezervacija je spremljena na vaš profil.\nOsvojili ste 2 boda! \nTrenutno imate {UserSession.LoyaltyPoints} bodova.",
            "OK");

        await Navigation.PopAsync();
    }

    private void OnSeatTapped(Seat seat)
    {
        if (seat == null)
            return;

        // uvijek dozvoli odznaèavanje
        if (seat.IsSelected)
        {
            seat.IsSelected = false;
            seat.Image = "seat.png";
            return;
        }

        // broj trenutno selektovanih
        int selectedCount = Seats.Count(s => s.IsSelected);

        if (selectedCount >= MaxSelectedSeats)
        {
            DisplayAlert("Limit",
                $"Možete odabrati najviše {MaxSelectedSeats} mjesta.",
                "OK");
            return;
        }


        // selektuj
        seat.IsSelected = true;
        seat.Image = "seat1.png";
    }




}
