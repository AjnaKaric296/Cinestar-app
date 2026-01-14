using Cinestar_app.Models;

namespace Cinestar_app.Services;

public static class UserSession
{
    public static User? CurrentUser { get; private set; }

    public static void Login(User user)
    {
        CurrentUser = user;
    }

    public static void Logout()
    {
        CurrentUser = null;
    }

    public static bool IsLoggedIn => CurrentUser != null;

    public static int LoyaltyPoints
    {
        get => CurrentUser?.LoyaltyPoints ?? 0;
        set
        {
            if (CurrentUser != null)
                CurrentUser.LoyaltyPoints = value;
        }
    }
}
