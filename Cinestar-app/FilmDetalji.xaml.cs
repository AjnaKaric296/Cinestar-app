using Cinestar_app.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace Cinestar_app
{
    public partial class FilmDetalji : ContentPage
    {
        private Film film;

        public FilmDetalji(Film selectedFilm)
        {
            InitializeComponent();
            film = selectedFilm;

            PosterImage.Source = film.Poster;
            TitleLabel.Text = film.Title;
            YearLabel.Text = film.Year;
            GenreLabel.Text = film.Genre;
            PlotLabel.Text = film.Plot;

            LoadShowtimes();
        }

        private void LoadShowtimes()
        {
            ShowtimesLayout.Children.Clear();

            foreach (var time in film.Showtimes)
            {
                var btn = new Button
                {
                    Text = time,
                    BackgroundColor = Color.FromArgb("#051851"),
                    TextColor = Colors.White,
                    CornerRadius = 8,
                    Margin = new Thickness(5),
                    Padding = new Thickness(15, 8)
                };

                btn.Clicked += OnShowtimeClicked;
                ShowtimesLayout.Children.Add(btn);
            }
        }

        private async void OnShowtimeClicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                await DisplayAlert(
                    "Rezervacija",
                    $"{film.Title}\nTermin: {btn.Text}",
                    "OK"
                );
            }
        }
    }
}
