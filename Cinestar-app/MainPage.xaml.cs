namespace Cinestar_app.Pages;

public partial class MainPage : ContentPage
{
   

    public MainPage()
    {
        InitializeComponent();

        var gradovi = new List<string>
        {
            "Zagreb",
            "Split",
            "Rijeka",
            "Osijek",
            "Pula"
        };

        gradoviPicker.ItemsSource = gradovi;

        gradoviPicker.SelectedItem = "Split";

    }


}
