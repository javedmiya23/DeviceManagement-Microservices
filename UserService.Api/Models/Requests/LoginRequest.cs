using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Models.Requests;

public class LoginRequest
{
    [Required]
    public string UserId { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
