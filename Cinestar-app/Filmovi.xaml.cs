namespace Cinestar_app.Pages;

public partial class Filmovi : ContentPage
{
	public Filmovi()
	{
		InitializeComponent();


	}

    private async void IdiNaFIlmDetalji(object sender, EventArgs e)
    { 
        await Navigation.PushAsync(new FilmDetalji());
    }

}