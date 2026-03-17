using AstraVenturaAuth.Adapters.Drivens.Database.Models;
using AstraVenturaAuth.Core.Common;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.Common.Errors;
using Microsoft.EntityFrameworkCore;

namespace AstraVenturaAuth.Adapters.Drivens.Database;

/// <summary>
/// Implementación de IUserRepository para manejo de usuarios en la base de datos
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AuthDbContext _db;

    public UserRepository(AuthDbContext db) => _db = db;

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken ct = default)
    {
        var entity = await _db.Users
            .AsNoTracking() // Para que no esté vigilando el contexto del objeto, no lo vamos a modificar ni guardar.
            .FirstOrDefaultAsync(u => u.Email == email.Value, ct);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<User?> FindByIdAsync(UserId id, CancellationToken ct = default)
    {
        var entity = await _db.Users
            .AsNoTracking() // Para que no esté vigilando el contexto del objeto, no lo vamos a modificar ni guardar.
            .FirstOrDefaultAsync(u => u.Id == id.Value, ct);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<bool> ExistsAsync(Email email, CancellationToken ct = default) =>
        await _db.Users.AnyAsync(u => u.Email == email.Value, ct);

    public async Task<Result<User>> SaveAsync(User user, CancellationToken ct = default)
    {
        var entity = new UserEntity
        {
            Id = user.Id.Value,
            Email = user.EmailAddress.Value,
            FirstName = user.Name.FirstName,
            MiddleName = user.Name.MiddleName,
            LastName = user.Name.LastName,
            SecondLastName = user.Name.SecondLastName,
            PasswordHash = user.PasswordHash.Value,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Users.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> UpdateAsync(User user, CancellationToken ct = default)
    {
        var entity = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id.Value, ct);

        if (entity is null)
        {
            return Result<User>.Failure(new ErrorResult("UserNotFound", "User not found for update."));
        }

        entity.Email = user.EmailAddress.Value;
        entity.FirstName = user.Name.FirstName;
        entity.MiddleName = user.Name.MiddleName;
        entity.LastName = user.Name.LastName;
        entity.SecondLastName = user.Name.SecondLastName;
        entity.PasswordHash = user.PasswordHash.Value;
        // entity.UpdatedAt = DateTime.UtcNow; // Avoid migration for now

        await _db.SaveChangesAsync(ct);

        return Result<User>.Success(user);
    }

    /// <summary>
    /// Mapea una entidad de usuario (BD) a un objeto de dominio (Core)
    /// </summary>
    private static User MapToDomain(UserEntity e) =>
        new(new UserId(e.Id),
            new Email(e.Email),
            new PersonName(e.FirstName, e.LastName, e.MiddleName, e.SecondLastName),
            new PasswordHash(e.PasswordHash));
}
