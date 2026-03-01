using System.ComponentModel.DataAnnotations;

namespace AstraVenturaAuth.Core.Dtos;

public class CredentialsDto
{
    [Required]
    public string? Email { get; set; }

    [Required]
    [MinLength(8)]
    public string? Password { get; set; }
}
