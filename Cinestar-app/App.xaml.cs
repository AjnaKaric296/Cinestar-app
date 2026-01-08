using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Cinestar_app
{
    public partial class App : Application
    {
        public static bool IsUserRegistered { get; set; } = false;  // ✅ DODAJ OVO!

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // *** TEMPORARNO OBRIŠI OVO ZA TEST ***
            Preferences.Clear(); // ODKOMENTIRAJ OVO 1 PUT

            if (Preferences.ContainsKey("SelectedCity"))
            {
                return new Window(new AppShell());
            }
            else
            {
                return new Window(new SplashPage());
            }
        }

        public static bool IsUserRegistered { get; set; } = false;

       

    }
}
