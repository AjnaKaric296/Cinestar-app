using System.Net.Http;
using System.Text.Json;

namespace Cinestar_app.Services
{
    public class OmdbService
    {
        private const string ApiKey = "88ad3a5";
        private readonly HttpClient client;
        private readonly Dictionary<string, OmdbMovieDetails> cache = new();

        public OmdbService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://www.omdbapi.com/")
            };
        }

        // DTO za listu (Search)
        public class OmdbMovieShort
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string imdbID { get; set; }
            public string Poster { get; set; }
        }

        // DTO za detalje filma
        public class OmdbMovieDetails
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Genre { get; set; }
            public string Runtime { get; set; }
            public string Director { get; set; }
            public string Plot { get; set; }
            public string Poster { get; set; }
            public string imdbID { get; set; }
            public string Actors { get; set; }
        }

        public async Task<List<OmdbMovieShort>> SearchMoviesAsync(string query)
        {
            var response = await client.GetStringAsync($"?apikey={ApiKey}&s={query}");
            using var json = JsonDocument.Parse(response);

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
                        Poster = item.TryGetProperty("Poster", out var p) && p.GetString() != "N/A"
                            ? p.GetString()
                            : "placeholder.png"
                    });
                }
            }

            return list;
        }

        public async Task<OmdbMovieDetails> GetMovieDetailsAsync(string imdbID)
        {
            if (cache.TryGetValue(imdbID, out var cachedMovie))
                return cachedMovie;

            var response = await client.GetStringAsync($"?apikey={ApiKey}&i={imdbID}&plot=full");
            using var jsonDoc = JsonDocument.Parse(response);
            var json = jsonDoc.RootElement;

            var details = new OmdbMovieDetails
            {
                Title = json.GetProperty("Title").GetString(),
                Year = json.GetProperty("Year").GetString(),
                Genre = json.GetProperty("Genre").GetString(),
                Director = json.GetProperty("Director").GetString(),
                Plot = json.GetProperty("Plot").GetString(),
                Poster = json.GetProperty("Poster").GetString(),
                imdbID = json.GetProperty("imdbID").GetString(),
                Actors = json.GetProperty("Actors").GetString(),
                Runtime = json.GetProperty("Runtime").GetString()
            };

            cache[imdbID] = details;
            return details;
        }
    }
}
