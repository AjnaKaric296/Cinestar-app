using Cinestar_app.Models;
using System.Text.Json;

public class OmdbService
{
    private readonly string apiKey = "75ace56d";
    private readonly HttpClient client = new();

    public OmdbService()
    {
        client.BaseAddress = new Uri("http://www.omdbapi.com/");
    }

    public async Task<List<Film>> SearchMoviesAsync(string query, int page = 1)
    {
        var url = $"?apikey={apiKey}&s={Uri.EscapeDataString(query)}&type=movie&page={page}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        var searchResult = JsonSerializer.Deserialize<SearchResponse>(content);
        return searchResult?.Search ?? new List<Film>();
    }

    public async Task<Film> GetMovieDetailsAsync(string imdbID)
    {
        var url = $"?apikey={apiKey}&i={imdbID}&plot=short";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Film>(content);
    }
}

public class SearchResponse
{
    public List<Film> Search { get; set; }
    public string totalResults { get; set; }
    public string Response { get; set; }
}
