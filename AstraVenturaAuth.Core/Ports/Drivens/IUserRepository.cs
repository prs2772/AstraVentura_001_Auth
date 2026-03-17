using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Domain.ValueObjects;

namespace AstraVenturaAuth.Core.Ports.Drivens;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(Email email, CancellationToken ct = default);
    Task<User?> FindByIdAsync(UserId id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Email email, CancellationToken ct = default);
    Task<Result<User>> SaveAsync(User user, CancellationToken ct = default);
    Task<Result<User>> UpdateAsync(User user, CancellationToken ct = default);
}
