using System.Text;
using AstraVenturaAuth.Core.Common.Errors;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Dtos;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.UseCases;
using Moq;
using Xunit;

namespace AstraVenturaAuth.Core.Tests;

public class ChangePasswordUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly ChangePasswordUseCase _useCase;

    public ChangePasswordUseCaseTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _tokenGeneratorMock = new Mock<ITokenGenerator>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _useCase = new ChangePasswordUseCase(
            _userRepoMock.Object,
            _tokenGeneratorMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCurrentPassword_UpdatesPasswordAndInvalidatesTokens()
    {
        // Arrange
        var userIdStr = Guid.NewGuid().ToString();
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewSecurePassword123!",
        };
        var ct = CancellationToken.None;

        var user = new User(
            new UserId(Guid.Parse(userIdStr)),
            new Email("test@domain.com"),
            new PersonName("Test", "User"),
            new PasswordHash("hashed-old-password-123456")
        );

        _userRepoMock
            .Setup(repo =>
                repo.FindByIdAsync(It.Is<UserId>(id => id.Value.ToString() == userIdStr), ct)
            )
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify(dto.CurrentPassword, user.PasswordHash.Value))
            .Returns(true);

        _passwordHasherMock
            .Setup(h => h.Hash(dto.NewPassword))
            .Returns("hashed-new-password-123456");

        // Act
        var result = await _useCase.ExecuteAsync(userIdStr, dto, ct);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify the new hash was set
        Assert.Equal("hashed-new-password-123456", user.PasswordHash.Value);

        // Verify it was correctly saved to repository
        _userRepoMock.Verify(r => r.SaveAsync(user, ct), Times.Once);

        // Verify tokens were invalidated
        _tokenGeneratorMock.Verify(
            t => t.InvalidateAllRefreshTokensForUserAsync(userIdStr, ct),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithIncorrectCurrentPassword_ReturnsFailure()
    {
        // Arrange
        var userIdStr = Guid.NewGuid().ToString();
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewSecurePassword123!",
        };
        var ct = CancellationToken.None;

        var user = new User(
            new UserId(Guid.Parse(userIdStr)),
            new Email("test@domain.com"),
            new PersonName("Test", "User"),
            new PasswordHash("hashed-old-password-123456")
        );

        _userRepoMock
            .Setup(repo =>
                repo.FindByIdAsync(It.Is<UserId>(id => id.Value.ToString() == userIdStr), ct)
            )
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify(dto.CurrentPassword, user.PasswordHash.Value))
            .Returns(false); // verification fails

        // Act
        var result = await _useCase.ExecuteAsync(userIdStr, dto, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(AuthErrors.InvalidCredentials.Code, result.Error.Code);

        // Verify no updates or token invalidations occurred
        _userRepoMock.Verify(r => r.SaveAsync(It.IsAny<User>(), ct), Times.Never);
        _tokenGeneratorMock.Verify(
            t => t.InvalidateAllRefreshTokensForUserAsync(It.IsAny<string>(), ct),
            Times.Never
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithIdenticalNewPassword_ReturnsFailure()
    {
        // Arrange
        var userIdStr = Guid.NewGuid().ToString();
        var samePassword = "SamePassword123!";
        var dto = new ChangePasswordDto
        {
            CurrentPassword = samePassword,
            NewPassword = samePassword,
        };
        var ct = CancellationToken.None;

        // Act
        var result = await _useCase.ExecuteAsync(userIdStr, dto, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("IdenticalPassword", result.Error.Code);

        // Validation: Verify it returns early without checking DB or hashing
        _userRepoMock.Verify(r => r.FindByIdAsync(It.IsAny<UserId>(), ct), Times.Never);
    }
}
