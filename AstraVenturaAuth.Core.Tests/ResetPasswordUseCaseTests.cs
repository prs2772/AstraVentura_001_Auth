using System.Text;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.UseCases;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace AstraVenturaAuth.Core.Tests;

public class ResetPasswordUseCaseTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly ResetPasswordUseCase _useCase;

    public ResetPasswordUseCaseTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _userRepoMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _useCase = new ResetPasswordUseCase(
            _cacheMock.Object,
            _userRepoMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidToken_UpdatesPasswordAndDeletesToken()
    {
        // Arrange
        var token = "valid-token-123";
        var newPassword = "NewSecurePassword123!";
        var dto = new ResetPasswordDto { Token = token, NewPassword = newPassword };
        var ct = CancellationToken.None;

        var userIdGuid = Guid.NewGuid();
        var encodedUserId = Encoding.UTF8.GetBytes(userIdGuid.ToString());

        var user = new User(
            new UserId(userIdGuid),
            new Email("test@domain.com"),
            new PersonName("Test", "User"),
            new PasswordHash("old-hash-98765432101234567890")
        );

        _cacheMock.Setup(c => c.GetAsync($"pwd-reset:{token}", ct)).ReturnsAsync(encodedUserId);

        _userRepoMock
            .Setup(repo => repo.FindByIdAsync(It.Is<UserId>(id => id.Value == userIdGuid), ct))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Hash(newPassword))
            .Returns("new-hash-01234567890123456789");

        // Act
        var result = await _useCase.ExecuteAsync(dto, ct);

        // Assert
        Assert.True(result.IsSuccess);

        // Validation: Verify the new hash was set via User entity method
        Assert.Equal("new-hash-01234567890123456789", user.PasswordHash.Value);

        // Verify it was correctly saved to repository
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), ct), Times.Once);

        // Verify token was deleted from cache preventing reuse
        _cacheMock.Verify(c => c.RemoveAsync($"pwd-reset:{token}", ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExpiredToken_ReturnsFailure()
    {
        // Arrange
        var token = "expired-token-123";
        var dto = new ResetPasswordDto { Token = token, NewPassword = "NewSecurePassword123!" };
        var ct = CancellationToken.None;

        // Simulate missing token from cache
        _cacheMock.Setup(c => c.GetAsync($"pwd-reset:{token}", ct)).ReturnsAsync((byte[])null!);

        // Act
        var result = await _useCase.ExecuteAsync(dto, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidToken", result.Error.Code);

        // Validation: Should not update or save anything
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), ct), Times.Never);
        _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), ct), Times.Never);
    }
}
