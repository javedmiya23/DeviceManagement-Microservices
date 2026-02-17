using System.Text.Json;
using UserService.Api.Models.Entities;
using UserService.Api.Repositories;
using UserService.Api.Utilities;
using UserService.Api.Models.Requests;
using UserService.Api.Caches.Interfaces;
using UserService.Api.Audit;
namespace UserService.Api.Services;

public class UserProcessor
{
    private readonly UserRepository _repository;

   private readonly IUserCache _cache;

    private readonly IAuditQueue _auditQueue;

    public UserProcessor(
        UserRepository repository,
        IUserCache cache,
        IAuditQueue auditQueue)
    {
        _repository = repository;
        _cache = cache;
        _auditQueue = auditQueue;
    }



   public async Task HandleCreateAsync(string payload)
    {
        var request = JsonSerializer.Deserialize<CreateUserRequest>(payload)!;

        var user = new User
        {
            UserId = request.UserId,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Name = request.Name,
            Permissions = request.Permissions
        };

        await _repository.AddUserAsync(user);

        await _cache.AddAsync(user);

        _auditQueue.Enqueue(new AuditLog
        {
            UserId = request.UserId,
            Message = $"User {request.UserId} created.",
            CreatedAt = DateTime.UtcNow
        });

    }


    public async Task HandleUpdateAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(payload)!;

        await _repository.UpdateUserAsync(
            data["UserId"],
            data["Name"],
            data["Permissions"]);

        var updatedUser = new User
        {
            UserId = data["UserId"],
            Name = data["Name"],
            Permissions = data["Permissions"],
            PasswordHash = "" 
        };

        await _cache.UpdateAsync(updatedUser);

        _auditQueue.Enqueue(new AuditLog
        {
            UserId = data["UserId"],
            Message = $"User {data["UserId"]} updated.",
            CreatedAt = DateTime.UtcNow
        });

    }

    public async Task HandleDeleteAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(payload)!;

        await _repository.DeleteUserAsync(data["UserId"]);

        await _cache.DeleteAsync(data["UserId"]);
        
        _auditQueue.Enqueue(new AuditLog
        {
            UserId = data["UserId"],
            Message = $"User {data["UserId"]} deleted.",
            CreatedAt = DateTime.UtcNow
        });

    }


}
