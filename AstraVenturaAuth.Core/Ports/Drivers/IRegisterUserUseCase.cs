using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Dtos;

namespace AstraVenturaAuth.Core.Ports.Drivers;

public interface IRegisterUserUseCase
{
    Task<Result<AuthenticatedUserDto>> ExecuteAsync(RegisterNewUserDto dto, CancellationToken ct = default);
}
