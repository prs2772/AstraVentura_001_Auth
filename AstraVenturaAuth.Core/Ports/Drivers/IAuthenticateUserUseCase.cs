using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Dtos;

namespace AstraVenturaAuth.Core.Ports.Drivers;

public interface IAuthenticateUserUseCase
{
    Task<Result<AuthenticatedUserDto>> ExecuteAsync(CredentialsDto credentials, CancellationToken ct = default);
}
