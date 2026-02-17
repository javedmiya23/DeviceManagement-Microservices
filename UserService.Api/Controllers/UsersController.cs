using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserService.Api.Caches.Interfaces;
using UserService.Api.Infrastructure.Kafka;
using UserService.Api.Models.Commands;
using UserService.Api.Models.Requests;
using UserService.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using UserService.Api.Attributes;
namespace UserService.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly KafkaProducer _producer;
    private readonly IUserCache _cache;
    private readonly UserRepository _repository;

    public UsersController(
        KafkaProducer producer,
        IUserCache cache,
        UserRepository repository)
    {
        _producer = producer;
        _cache = cache;
        _repository = repository;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }

    [Authorize]

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var cached = await _cache.GetAsync(userId);

        if (cached != null)
            return Ok(cached);

        var user = await _repository.GetByUserIdAsync(userId);

        if (user == null)
            return NotFound();

        await _cache.AddAsync(user);

        return Ok(user);
    }

    [Authorize]
    [RequirePermission(7)]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new UserCommand
        {
            CommandType = "CreateUser",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Serialize(request)
        };

        await _producer.ProduceAsync("user-commands", command);

        return Accepted(new
        {
            message = "User creation command accepted",
            correlationId = command.CorrelationId
        });
    }

 
    [Authorize]
    [RequirePermission(8)]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(
        string userId,
        [FromBody] UpdateUserRequest request)
    {
        var command = new UserCommand
        {
            CommandType = "UpdateUser",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Serialize(new
            {
                UserId = userId,
                request.Name,
                request.Permissions
            })
        };

        await _producer.ProduceAsync("user-commands", command);

        return Accepted(new
        {
            message = "User update command accepted",
            correlationId = command.CorrelationId
        });
    }

    [Authorize]
    [RequirePermission(9)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var command = new UserCommand
        {
            CommandType = "DeleteUser",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Serialize(new
            {
                UserId = userId
            })
        };

        await _producer.ProduceAsync("user-commands", command);

        return Accepted(new
        {
            message = "User deletion command accepted",
            correlationId = command.CorrelationId
        });
    }
}
