using System.Text.Json.Serialization;

namespace Cinestar_app.Models;

public class FilmApiResponse
{
    [JsonPropertyName("Title")]
    public string Title { get; set; }

    [JsonPropertyName("Year")]
    public string Year { get; set; }

    [JsonPropertyName("Genre")]
    public string Genre { get; set; }

    [JsonPropertyName("Poster")]
    public string Poster { get; set; }

    [JsonPropertyName("Plot")]
    public string Plot { get; set; }

    [JsonPropertyName("imdbID")]
    public string ImdbID { get; set; }

    [JsonPropertyName("Actors")]
    public string Actors { get; set; }
}
