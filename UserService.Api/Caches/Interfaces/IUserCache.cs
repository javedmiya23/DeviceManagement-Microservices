using UserService.Api.Models.Entities;

namespace UserService.Api.Caches.Interfaces;

public interface IUserCache
{
    Task<User?> GetAsync(string userId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string userId);
}
