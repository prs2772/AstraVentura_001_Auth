using AstraVenturaAuth.Core.Common;

namespace AstraVenturaAuth.Core.Ports.Drivers;

public interface IRequestPasswordRecoveryUseCase
{
    Task<Result<bool>> ExecuteAsync(string email, string ipAddress, CancellationToken ct);
}
