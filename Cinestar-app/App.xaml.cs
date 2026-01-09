using Microsoft.Maui.Controls;

namespace Cinestar_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();  // ✅ OVDJE mora raditi!

            MainPage = new NavigationPage(new HomePage())
            {
                BarBackgroundColor = Color.FromArgb("#1E3A8A"),
                BarTextColor = Colors.White
            };
        }
    }
}
