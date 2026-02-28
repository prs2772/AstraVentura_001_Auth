namespace AstraVenturaAuth.Core.Common.Errors;

/// <summary>
/// Define un error con su código y el mensaje del mismo
/// </summary>
/// <param name="Code">Código del error, AVAuth.[Nombre/tipo error]</param>
/// <param name="Message">Mensaje personalizado para mostrar el error en cuestión</param>
public record ErrorResult(string Code, string Message)
{
    /// <summary>
    /// No hay ningún error
    /// </summary>
    public static readonly ErrorResult None = new(string.Empty, string.Empty);

    /// <summary>
    /// Error por valor nulo no permisible
    /// </summary>
    public static readonly ErrorResult NullValue = new("Error.NullValue", "El valor no puede ser nulo.");
}
