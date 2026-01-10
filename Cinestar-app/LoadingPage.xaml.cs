using Microsoft.Maui.Controls;

namespace Cinestar_app
{
    public partial class LoadingPage : ContentPage
    {
        // Konstruktor
        public LoadingPage()
        {
            InitializeComponent();
            // Ako želiš odmah nešto async, pozovi metodu iz konstruktora
            // Npr. StartLoadingAsync();  
        }

        // Async metoda mora biti unutar klase
        private async Task StartLoadingAsync()
        {
            // Ovo je primjer async koda
            await Task.Delay(1000); // simulacija učitavanja
        }
    }
}
