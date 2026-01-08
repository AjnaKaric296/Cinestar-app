using Microsoft.Maui.Controls;

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
            if (Preferences.ContainsKey("SelectedCity"))
            {
                return new Window(new MainTabbedPage());
            }
            else
            {
                return new Window(new SplashPage());
            }
        }
    }
}
