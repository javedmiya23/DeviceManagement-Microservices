using System.Collections.Concurrent;
using UserService.Api.Caches.Interfaces;
using UserService.Api.Models.Entities;

namespace UserService.Api.Caches.Implementations;

public class InMemoryUserCache : IUserCache
{
    private readonly ConcurrentDictionary<string, User> _cache =
        new ConcurrentDictionary<string, User>();

    public Task<User?> GetAsync(string userId)
    {
        _cache.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }

    public Task AddAsync(User user)
    {
        _cache[user.UserId] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        _cache[user.UserId] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string userId)
    {
        _cache.TryRemove(userId, out _);
        return Task.CompletedTask;
    }
}
