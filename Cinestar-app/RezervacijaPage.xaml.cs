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

    public ObservableCollection<Seat> SeatsLeft { get; set; }
    public ObservableCollection<Seat> SeatsRight { get; set; }

    public ICommand SeatTappedCommand { get; }

    private int _ticketCount = 1;
    private int MaxSelectedSeats => _ticketCount;
    public Film Film { get; set; }

    public RezervacijaPage(Film film)
    {
        InitializeComponent();
        Film = film;
        SeatsLeft = new ObservableCollection<Seat>();
        SeatsRight = new ObservableCollection<Seat>();

        for (int i = 0; i < 12; i++)
        {
            SeatsLeft.Add(new Seat { Image = "seat.png", IsSelected = false });
            SeatsRight.Add(new Seat { Image = "seat.png", IsSelected = false });
        }

        SeatTappedCommand = new Command<Seat>(OnSeatTapped);
        BindingContext = this;

        _film = film;
        _filmService = new FilmService();

        PosterImage.Source = _film.Poster;
        FilmTitleLabel.Text = _film.Title;

        TicketCountLabel.Text = _ticketCount.ToString();

        TimePicker.ItemsSource = new List<string>
        {
            "16:30", "18:00", "20:15", "22:00"
        };
        TimePicker.SelectedIndex = 0;
    }

    private void PlusClicked(object sender, EventArgs e)
    {
        if (_ticketCount < 10)
            _ticketCount++;

        UpdateTicketCount();
    }

    private void MinusClicked(object sender, EventArgs e)
    {
        if (_ticketCount > 1)
            _ticketCount--;

        UpdateTicketCount();
    }

    private void UpdateTicketCount()
    {
        TicketCountLabel.Text = _ticketCount.ToString();

        var selectedSeats = SeatsLeft.Concat(SeatsRight)
                                     .Where(s => s.IsSelected)
                                     .ToList();

        while (selectedSeats.Count > _ticketCount)
        {
            var seat = selectedSeats.Last();
            seat.IsSelected = false;
            seat.Image = "seat.png";
            selectedSeats.Remove(seat);
        }
    }


    private void OnSeatTapped(Seat seat)
    {
        if (seat == null)
            return;

        if (seat.IsSelected)
        {
            seat.IsSelected = false;
            seat.Image = "seat.png";
        }
        else
        {
            int selectedCount = SeatsLeft.Concat(SeatsRight)
                                         .Count(s => s.IsSelected);

            if (selectedCount >= MaxSelectedSeats)
            {
                DisplayAlert("Limit",
                    $"Možete odabrati najviše {MaxSelectedSeats} mjesta.",
                    "OK");
                return;
            }

            seat.IsSelected = true;
            seat.Image = "seat1.png";
        }

     
        ConfirmButton.IsEnabled =
            SeatsLeft.Concat(SeatsRight).Any(s => s.IsSelected);

        ConfirmButton.Opacity = ConfirmButton.IsEnabled ? 1 : 0.5;
    }



    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        var userDb = new UserDatabase();

        if (!UserSession.IsLoggedIn || UserSession.CurrentUser == null)
        {
            await DisplayAlert("Greška",
                "Morate se prijaviti da biste rezervirali kartu.",
                "OK");
            return;
        }

        var reservation = new Reservation
        {
            UserEmail = UserSession.CurrentUser.Email,
            FilmTitle = _film.Title,
            FilmId = _film.ImdbID,
            Date = DatePicker.Date,
            Time = TimePicker.SelectedItem?.ToString() ?? "",
            TicketCount = _ticketCount
        };

        await userDb.AddReservationAsync(reservation);

        UserSession.LoyaltyPoints += 2;
        await userDb.UpdateUserLoyaltyAsync(
            UserSession.CurrentUser.Email,
            UserSession.LoyaltyPoints);

        MessagingCenter.Send(this, "UpdateLoyaltyPoints");

        await DisplayAlert("Uspješno",
            $"Rezervacija je spremljena.\nOsvojili ste 2 boda!\nUkupno: {UserSession.LoyaltyPoints}",
            "OK");

        await Navigation.PopAsync();
    }
}
