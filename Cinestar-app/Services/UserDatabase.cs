using SQLite;
using Cinestar_app.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;

namespace Cinestar_app.Services;

public class UserDatabase
{
    private SQLiteAsyncConnection _database;

    public UserDatabase()
    {
        InitializeAsync();
    }

    private void InitializeAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "cinestar.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<User>().Wait();
        _database.CreateTableAsync<Loyalty>().Wait();
    }


     public Task<User?> GetUserByEmailAsync(string email)
     {
        // Ensure database connection is initialized
        // Normalize email and query the Users table
        email = email.Trim().ToLower();
        return _database.Table<User>()
            .Where(u => u.Email.ToLower() == email)
            .FirstOrDefaultAsync();
     }

    public Task AddUserAsync(User user)
    {
        // Ensure normalized email
        user.Email = user.Email?.Trim().ToLower() ?? "";
        return _database.InsertAsync(user);
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return _database.Table<User>().ToListAsync();
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


}
