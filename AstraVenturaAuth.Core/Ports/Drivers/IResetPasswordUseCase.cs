using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Dtos;

namespace AstraVenturaAuth.Core.Ports.Drivers;

public interface IResetPasswordUseCase
{
    Task<Result<bool>> ExecuteAsync(ResetPasswordDto dto, CancellationToken ct);
}
