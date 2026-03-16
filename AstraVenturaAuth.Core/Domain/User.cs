using AstraVenturaAuth.Core.Domain.ValueObjects;

namespace AstraVenturaAuth.Core.Domain;

public sealed class User
{
    public UserId Id { get; }
    public Email EmailAddress { get; }
    public PersonName Name { get; }
    public PasswordHash PasswordHash { get; private set; }

    public void UpdatePassword(PasswordHash newPassword)
    {
        PasswordHash = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
    }

    public User(UserId id, Email emailAddress, PersonName name, PasswordHash passwordHash)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }
}
