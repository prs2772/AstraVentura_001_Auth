namespace AstraVenturaAuth.Core.Dtos;

public class ChangePasswordDto
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}
