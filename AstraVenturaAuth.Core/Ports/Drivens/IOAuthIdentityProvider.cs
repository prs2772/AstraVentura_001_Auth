namespace AstraVenturaAuth.Core.Ports.Drivens;

public record OAuthUserInfo(string ProviderId, string Email, string Name);

public interface IOAuthIdentityProvider
{
    Task<OAuthUserInfo?> GetUserInfoAsync(string accessToken, CancellationToken ct = default);
}
