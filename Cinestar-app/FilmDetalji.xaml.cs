using Cinestar_app.Models;
using Microsoft.Maui.Controls;
using System;

namespace Cinestar_app;

public partial class FilmDetalji : ContentPage
{
    private Film film;

    public FilmDetalji(Film selectedFilm)
    {
        InitializeComponent();

        film = selectedFilm;

        PosterImage.Source = film.Poster;
        TitleLabel.Text = film.Title;
        GenreLabel.Text = $"Žanr: {film.Genre}";
        YearLabel.Text = $"Godina: {film.Year}";
        PlotLabel.Text = film.Plot;

        foreach (var time in film.Showtimes)
        {
            var btn = new Button { Text = time };
            btn.Clicked += OnShowtimeClicked;
            ShowtimesLayout.Children.Add(btn);
        }
    }

    private async void OnShowtimeClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            await DisplayAlert("Odabrali ste termin", $"{film.Title} - {btn.Text}", "OK");
        }
    }
}
