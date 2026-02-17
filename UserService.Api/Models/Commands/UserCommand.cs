namespace UserService.Api.Models.Commands;

public class UserCommand
{
    public string CommandType { get; set; } = default!;
    public string CorrelationId { get; set; } = default!;
    public string Payload { get; set; } = default!;
}
