using Cinestar_app.Models;
using Cinestar_app.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cinestar_app;

public partial class MojeRezervacije : ContentPage
{
    private readonly UserDatabase _db;
    public ObservableCollection<Reservation> Reservations { get; set; } = new();

    public MojeRezervacije()
    {
        InitializeComponent();

        _db = new UserDatabase();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReservationsAsync();
    }

    private async Task LoadReservationsAsync()
    {
        Reservations.Clear();

       
        var email = Preferences.Get("LoggedInEmail", string.Empty);
        if (!UserSession.IsLoggedIn || UserSession.CurrentUser == null)
        {
            await DisplayAlert("Greška", "Morate biti prijavljeni da vidite rezervacije.", "OK");
            return;
        }

        var reservations = await _db.GetReservationsForUserAsync(email);

        foreach (var r in reservations)
            Reservations.Add(r);
    }
}
