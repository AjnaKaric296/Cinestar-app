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
    }

    public Task<int> AddUserAsync(User user)
    {
        return _database.InsertAsync(user);
    }

    public Task<User> GetUserByEmailAsync(string email)
    {
        return _database.Table<User>()
                        .Where(u => u.Email == email)
                        .FirstOrDefaultAsync();
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return _database.Table<User>().ToListAsync();
    }
}
