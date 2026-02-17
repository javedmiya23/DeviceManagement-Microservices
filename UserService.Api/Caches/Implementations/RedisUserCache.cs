using StackExchange.Redis;
using UserService.Api.Caches.Interfaces;
using UserService.Api.Models.Entities;

namespace UserService.Api.Caches.Implementations;

public class RedisUserCache : IUserCache
{
    private readonly IDatabase _database;

    public RedisUserCache(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    private string GetKey(string userId) => $"user:{userId}";

    public async Task<User?> GetAsync(string userId)
    {
        var key = GetKey(userId);
        var entries = await _database.HashGetAllAsync(key);

        if (entries.Length == 0)
            return null;

        var dict = entries.ToDictionary(
            x => x.Name.ToString(),
            x => x.Value.ToString());

        return new User
        {
            UserId = userId,
            PasswordHash = dict["PasswordHash"],
            Name = dict["Name"],
            Permissions = dict["Permissions"]
        };
    }

    public async Task AddAsync(User user)
    {
        var key = GetKey(user.UserId);

        var entries = new HashEntry[]
        {
            new("PasswordHash", user.PasswordHash),
            new("Name", user.Name),
            new("Permissions", user.Permissions)
        };

        await _database.HashSetAsync(key, entries);
    }

    public async Task UpdateAsync(User user)
    {
        await AddAsync(user);
    }

    public async Task DeleteAsync(string userId)
    {
        await _database.KeyDeleteAsync(GetKey(userId));
    }
}
