namespace AstraVenturaAuth.Core.Ports.Drivens;

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);
    bool Verify(string plainTextPassword, string hash);
}
