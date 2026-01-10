using System.Net.Http;
using System.Text.Json;

namespace Cinestar_app.Services;

public class OmdbService
{
    private readonly string apiKey = "fa0b2c6c";
    private readonly HttpClient client = new();

    private Dictionary<string, OmdbMovieDetails> cache = new();

    public class OmdbMovieShort
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string imdbID { get; set; }
        public string Poster { get; set; }
    }

    public class OmdbMovieDetails
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }
        public string imdbID { get; set; }
    }

    public async Task<List<OmdbMovieShort>> SearchMoviesAsync(string query)
    {
        var url = $"https://www.omdbapi.com/?apikey={apiKey}&s={query}";
        var response = await client.GetStringAsync(url);
        var json = JsonDocument.Parse(response);

        var list = new List<OmdbMovieShort>();
        if (json.RootElement.TryGetProperty("Search", out var searchResults))
        {
            foreach (var item in searchResults.EnumerateArray())
            {
                list.Add(new OmdbMovieShort
                {
                    Title = item.GetProperty("Title").GetString(),
                    Year = item.GetProperty("Year").GetString(),
                    imdbID = item.GetProperty("imdbID").GetString(),
                    Poster = item.TryGetProperty("Poster", out var p) ? p.GetString() : "placeholder.png"
                });
            }
        }
        return list;
    }

    public async Task<OmdbMovieDetails> GetMovieDetailsAsync(string imdbID)
    {
        if (cache.ContainsKey(imdbID))
            return cache[imdbID];

        var url = $"https://www.omdbapi.com/?apikey={apiKey}&i={imdbID}";
        var response = await client.GetStringAsync(url);
        var json = JsonDocument.Parse(response).RootElement;

        var details = new OmdbMovieDetails
        {
            Title = json.GetProperty("Title").GetString(),
            Year = json.GetProperty("Year").GetString(),
            Genre = json.GetProperty("Genre").GetString(),
            Plot = json.GetProperty("Plot").GetString(),
            Poster = json.GetProperty("Poster").GetString(),
            imdbID = json.GetProperty("imdbID").GetString()
        };

        cache[imdbID] = details;
        return details;
    }
}
