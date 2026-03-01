namespace AstraVenturaAuth.Core.Domain.ValueObjects;

public sealed record PersonName
{
    public string FirstName { get; }
    public string? MiddleName { get; }
    public string LastName { get; }
    public string? SecondLastName { get; }

    public PersonName(
        string firstName,
        string lastName,
        string? middleName = null,
        string? secondLastName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        SecondLastName = secondLastName;
    }

    public string FullName =>
        string.Join(" ", new[]
        {
            FirstName,
            MiddleName,
            LastName,
            SecondLastName
        }.Where(x => !string.IsNullOrWhiteSpace(x)));
}
