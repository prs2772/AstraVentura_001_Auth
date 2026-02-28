using AstraVenturaAuth.Core.Common.Errors;

namespace AstraVenturaAuth.Core.Common;

/// <summary>
/// Result Pattern para gestión de un resultado esperado que podría fallar
/// </summary>
/// <typeparam name="T">Clase envuelta esperada</typeparam>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public ErrorResult Error { get; }

    private Result(bool isSuccess, T? value, ErrorResult error)
    {
        if (isSuccess && error != ErrorResult.None)
            throw new InvalidOperationException("Un resultado exitoso no puede tener error.");

        if (!isSuccess && error == ErrorResult.None)
            throw new InvalidOperationException("Un resultado fallido debe tener un error.");

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, ErrorResult.None);
    public static Result<T> Failure(ErrorResult error) => new(false, default, error);
}
