using System.Collections.ObjectModel;
using Cinestar_app.Models;
using Cinestar_app.Services;
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

        _db = new UserDatabase();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (UserSession.IsLoggedIn)
        {
            NonLoggedInLayout.IsVisible = false;
            LoggedInLayout.IsVisible = true;

            PozdravLabel.Text = $"Dobrodošao/la, {UserSession.CurrentUser.Ime}!";

            await UpdatePointsLabel();

            // Slušaj update bodova iz FilmDetalji
            MessagingCenter.Subscribe<FilmDetalji>(this, "UpdateLoyaltyPoints", async (_) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await UpdatePointsLabel();
                });
            });
        }
        else
        {
            NonLoggedInLayout.IsVisible = true;
            LoggedInLayout.IsVisible = false;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<FilmDetalji>(this, "UpdateLoyaltyPoints");
    }

    // GLOBALNI BODOVI – samo iz UserSession
    private Task UpdatePointsLabel()
    {
        if (!UserSession.IsLoggedIn) return Task.CompletedTask;

        BodoviLabel.Text = UserSession.LoyaltyPoints.ToString();
        return Task.CompletedTask;
    }

    private async void OnViewPointsClicked(object sender, EventArgs e)
    {
        if (!UserSession.IsLoggedIn) return;

        // Trenutni bodovi, ograničeni do 100
        int currentPoints = Math.Min(UserSession.LoyaltyPoints, 100);

        // Bodovi do nove nagrade (100 je limit)
        int pointsToNextReward = 100 - currentPoints;

        // Tekst koji se prikazuje korisniku
        string message = $"Trenutno imaš {currentPoints} zvjezdica.\n" +
                         $"Još {pointsToNextReward} zvjezdica do nove nagrade! 🎁";

        await DisplayAlert("Tvoje zvjezdice", message, "OK");
    }

    public async void OnClickedKupovina(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Filmovi());
    }

}
