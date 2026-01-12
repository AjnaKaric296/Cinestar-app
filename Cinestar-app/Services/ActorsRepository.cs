using Cinestar_app.Models;

namespace Cinestar_app.Services;

public static class ActorsRepository
{
    public static Dictionary<string, Actor> Actors { get; } = new();
}
