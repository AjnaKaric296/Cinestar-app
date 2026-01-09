using System.Net.Http;
using System.Text.Json;
using Cinestar_app.Models;

namespace Cinestar_app.Services;

public class OmdbService
{
    private readonly HttpClient _client;
    private readonly string _apiKey = "75ace56d"; // tvoj OMDB API key

    public OmdbService()
    {
        _client = new HttpClient();
    }

    // Klasa za search rezultate
    private class OmdbSearchResult
    {
        public OmdbMovieShort[] Search { get; set; } = Array.Empty<OmdbMovieShort>();
        public string Response { get; set; } = "False";
    }

    // Kratka info o filmu
    private class OmdbMovieShort
    {
        public string Title { get; set; } = "";
        public string Year { get; set; } = "";
        public string ImdbID { get; set; } = "";
        public string Poster { get; set; } = "";
    }

    // Detalji filma
    public class OmdbMovieFull
    {
        public string Title { get; set; } = "";
        public string Year { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Plot { get; set; } = "";
        public string Poster { get; set; } = "";
        public string ImdbID { get; set; } = "";
    }

    // Search po naslovu
    public async Task<List<Film>> SearchMoviesAsync(string query)
    {
        var url = $"http://www.omdbapi.com/?apikey={_apiKey}&s={query}&type=movie";
        var response = await _client.GetStringAsync(url);
        var result = JsonSerializer.Deserialize<OmdbSearchResult>(response);

        if (result?.Search == null) return new List<Film>();

        return result.Search.Select(m => new Film
        {
            Title = m.Title,
            Year = m.Year,
            ImdbID = m.ImdbID,
            Poster = m.Poster
        }).ToList();
    }

    // Detalji filma
    public async Task<OmdbMovieFull> GetMovieDetailsAsync(string imdbID)
    {
        var url = $"http://www.omdbapi.com/?apikey={_apiKey}&i={imdbID}&plot=short";
        var response = await _client.GetStringAsync(url);
        var result = JsonSerializer.Deserialize<OmdbMovieFull>(response);
        return result ?? new OmdbMovieFull();
    }
}
