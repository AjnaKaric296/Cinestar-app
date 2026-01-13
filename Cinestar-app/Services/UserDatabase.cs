using SQLite;
using Cinestar_app.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cinestar_app.Services;

public class UserDatabase
{
    private SQLiteAsyncConnection _database;

    public UserDatabase()
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CinestarUsers.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<User>().Wait();
        _database.CreateTableAsync<Loyalty>();
    }

    public Task<int> AddUserAsync(User user)
    {
        return _database.InsertAsync(user);
    }

     public Task<User> GetUserByEmailAsync(string email)
     {
        email = email.Trim().ToLower();
        return _database.Table<User>()
                        .Where(u => u.Email.ToLower() == email)
                        .FirstOrDefaultAsync();
     }

    public Task<List<User>> GetAllUsersAsync()
    {
        return _database.Table<User>().ToListAsync();
    }

    public async Task UpdateUserLoyaltyAsync(string email, int newPoints)
    {
        var user = await _database.Table<User>()
                                  .Where(u => u.Email == email)
                                  .FirstOrDefaultAsync();

        if (user != null)
        {
            user.LoyaltyPoints = newPoints;  // ili Bodovi, kako se zove u tvojoj klasi
            await _database.UpdateAsync(user);
        }
    }

    // UserDatabase.cs
    public async Task<Loyalty?> GetLoyaltyAsync(string email)
    {
        return await _database.Table<Loyalty>()
            .FirstOrDefaultAsync(l => l.UserEmail == email);
    }

    public async Task AddPointsAsync(string email, int points)
    {
        var loyalty = await GetLoyaltyAsync(email);

        if (loyalty == null)
        {
            loyalty = new Loyalty
            {
                UserEmail = email,
                Bodovi = points
            };
            await _database.InsertAsync(loyalty);
        }
        else
        {
            loyalty.Bodovi += points;
            await _database.UpdateAsync(loyalty);
        }
    }
    public Task<int> AddReservationAsync(Reservation reservation)
    {
        // automatski stvara tablicu ako ne postoji
        _database.CreateTableAsync<Reservation>().Wait();
        return _database.InsertAsync(reservation);
    }

    public Task<List<Reservation>> GetReservationsForUserAsync(string email)
    {
        _database.CreateTableAsync<Reservation>().Wait();
        return _database.Table<Reservation>()
                        .Where(r => r.UserEmail == email)
                        .ToListAsync();
    }



}
