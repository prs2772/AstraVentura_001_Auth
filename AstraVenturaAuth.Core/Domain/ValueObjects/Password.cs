namespace AstraVenturaAuth.Core.Domain.ValueObjects;

public sealed record Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password cannot be empty");

        if (value.Length < 8)
            throw new ArgumentException("Password must be minimum 8 characters.");

        if (!value.Any(char.IsUpper))
            throw new ArgumentException("Password must contain uppercase letter.");

        if (!value.Any(char.IsLower))
            throw new ArgumentException("Password must contain lowercase letter.");

        if (!value.Any(char.IsDigit))
            throw new ArgumentException("Password must contain a number.");

        if (!value.Any(c => "!#$&/()=.".Contains(c)))
            throw new ArgumentException("Password must contain a valid symbol: !#$&/()=.");

        Value = value;
    }

    public override string ToString() => Value;
}
