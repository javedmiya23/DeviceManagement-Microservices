using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Models.Requests;

public class CreateUserRequest
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9_]{4,20}$",
        ErrorMessage = "UserId must be 4-20 characters, alphanumeric or underscore.")]
    public string UserId { get; set; } = default!;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; } = default!;

    [Required]
    [RegularExpression("^[a-zA-Z ]{3,50}$",
        ErrorMessage = "Name must contain only letters and spaces.")]
    public string Name { get; set; } = default!;

    [Required]
    [RegularExpression(@"^(\d+,)*\d+$",
        ErrorMessage = "Permissions must be comma-separated numbers.")]
    public string Permissions { get; set; } = default!;
}
