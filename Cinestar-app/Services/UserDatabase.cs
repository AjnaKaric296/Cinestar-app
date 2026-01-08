using SQLite;
using Cinestar_app.Models;
using System.IO;
using System.Threading.Tasks;

namespace Cinestar_app.Services;

public class UserDatabase
{
    readonly SQLiteAsyncConnection database;

    public UserDatabase()
    {
        // ✅ Android siguran path!
#if ANDROID
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "CinestarUsers.db3");
#else
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "CinestarUsers.db3");
#endif

        database = new SQLiteAsyncConnection(dbPath);
        database.CreateTableAsync<User>().Wait();
    }

    public Task<int> AddUserAsync(User user) => database.InsertAsync(user);

    public Task<User> GetUserByEmailAsync(string email)
    {
        email = email.Trim().ToLower();
        return database.Table<User>()
                       .Where(u => u.Email.ToLower() == email)
                       .FirstOrDefaultAsync();
    }
}
