using System.Net.Http;
using System.Text.Json;
using Cinestar_app.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cinestar_app.Services
{
    public class FilmService
    {
        private readonly string apiKey = "418f702e"; // OMDb ključ

        private static readonly Random rnd = new();

        public async Task<Film> GetFilmFromApi(string imdbId)
        {
            using var http = new HttpClient();
            string url = $"https://www.omdbapi.com/?i={imdbId}&apikey={apiKey}";

            var response = await http.GetStringAsync(url);
            var apiFilm = JsonSerializer.Deserialize<FilmApiResponse>(response);

            if (apiFilm == null) return null;

            var actorNames = (apiFilm.Actors ?? "")
                .Split(", ")
                .Where(a => !string.IsNullOrWhiteSpace(a) && a != "N/A")
                .ToList();

            var actorsList = actorNames
                .Select(a => new Actor
                {
                    Name = a,
                    Photo = $"https://thispersondoesnotexist.com"
                })
                .ToList();

            var film = new Film
            {
                Title = apiFilm.Title,
                Year = apiFilm.Year,
                Genre = apiFilm.Genre,
                Poster = apiFilm.Poster == "N/A" ? "placeholder.png" : apiFilm.Poster,
                Plot = apiFilm.Plot,
                ImdbID = apiFilm.ImdbID,
                Actors = actorsList
            };

            return film;
        }

        public async Task<List<FilmApiResponse>> SearchMoviesAsync(string query)
        {
            using var http = new HttpClient();
            string url = $"https://www.omdbapi.com/?s={query}&apikey={apiKey}";

            var response = await http.GetStringAsync(url);
            var searchResult = JsonSerializer.Deserialize<OmdbSearchResponse>(response);

            return searchResult?.Search ?? new List<FilmApiResponse>();
        }
    }

    public class OmdbSearchResponse
    {
        public List<FilmApiResponse> Search { get; set; } = new();
    }
}
