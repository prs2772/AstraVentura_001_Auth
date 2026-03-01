namespace AstraVenturaAuth.Core.Dtos;

public class RegisterNewUserDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPrincipal { get; set; }
    public string? ApellidoSecundario { get; set; }
}
