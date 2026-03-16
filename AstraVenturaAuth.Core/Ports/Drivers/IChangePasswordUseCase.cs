using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Dtos;

namespace AstraVenturaAuth.Core.Ports.Drivers;

public interface IChangePasswordUseCase
{
    Task<Result<bool>> ExecuteAsync(string userIdStr, ChangePasswordDto dto, CancellationToken ct = default);
}
