using Microsoft.Maui.Controls;

namespace Cinestar_app
{
    public partial class LoadingPage : ContentPage
    {
 
        public LoadingPage()
        {
            InitializeComponent();
            
        }

      
        private async Task StartLoadingAsync()
        {
            
            await Task.Delay(1000); 
        }
    }
}
