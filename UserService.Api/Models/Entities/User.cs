namespace UserService.Api.Models.Entities;

public class User
{
    public string UserId { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Permissions { get; set; } = default!;
}
