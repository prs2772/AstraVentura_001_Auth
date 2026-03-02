using AstraVenturaAuth.Core.Ports.Drivens;
using BCrypt.Net;

namespace AstraVenturaAuth.Adapters.Drivens.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainTextPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 12);

    public bool Verify(string plainTextPassword, string hash) =>
        BCrypt.Net.BCrypt.Verify(plainTextPassword, hash);
}
