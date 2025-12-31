using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Cinestar_app
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await AnimateSplash();
        }

        private async Task AnimateSplash()
        {
            await LogoImage.FadeTo(1, 1000);
            await LogoImage.ScaleTo(1.0, 1000, Easing.CubicOut);

            await Task.Delay(200);
            await WelcomeLabel.FadeTo(1, 1000);

            await Task.Delay(1500);

            Application.Current.MainPage = new CityPickerPage();
        }
    }
}
