using Cinestar_app.Models;
using Cinestar_app.Data;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace Cinestar_app;

public partial class GlumciPage : ContentPage
{
    public GlumciPage()
    {
        InitializeComponent();

        // Prikaz svih glumaca iz baze
        ActorsCollectionView.ItemsSource = ActorsDatabase.AllActors;
    }
}
