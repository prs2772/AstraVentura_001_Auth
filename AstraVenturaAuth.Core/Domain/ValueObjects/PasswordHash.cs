namespace AstraVenturaAuth.Core.Domain.ValueObjects;

public sealed record PasswordHash
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password hash cannot be empty");

        if (value.Length < 20)
            throw new ArgumentException("Invalid password hash format");

        Value = value;
    }

    public override string ToString() => Value;
}
