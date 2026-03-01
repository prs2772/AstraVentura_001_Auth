using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Dtos;

namespace AstraVenturaAuth.Core.Ports.Drivers;

public interface IRefreshTokenUseCase
{
    Task<Result<AuthenticatedUserDto>> ExecuteAsync(string refreshToken, CancellationToken ct = default);
}
