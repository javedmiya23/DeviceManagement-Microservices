namespace UserService.Api.Models.Requests;

public class UpdateUserRequest
{
    public string Name { get; set; } = default!;
    public string Permissions { get; set; } = default!;
}
