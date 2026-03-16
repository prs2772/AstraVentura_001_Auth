using System.Text;
using System.Text.Json;
using AstraVenturaAuth.Core.Domain;
using AstraVenturaAuth.Core.Domain.ValueObjects;
using AstraVenturaAuth.Core.Ports.Drivens;
using AstraVenturaAuth.Core.UseCases;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AstraVenturaAuth.Core.Tests;

public class RequestPasswordRecoveryUseCaseTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly RequestPasswordRecoveryUseCase _useCase;

    public RequestPasswordRecoveryUseCaseTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _userRepoMock = new Mock<IUserRepository>();
        _emailSenderMock = new Mock<IEmailSender>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["FrontendBaseUrl"]).Returns("https://test.com");

        _useCase = new RequestPasswordRecoveryUseCase(
            _cacheMock.Object,
            _userRepoMock.Object,
            _emailSenderMock.Object,
            _configMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidUser_GeneratesTokenAndSendsEmail()
    {
        // Arrange
        var email = "test@domain.com";
        var ip = "127.0.0.1";
        var ct = CancellationToken.None;

        var user = new User(
            new UserId(Guid.NewGuid()),
            new Email(email),
            new PersonName("Test", "User"),
            new PasswordHash("somehash1234567890123")
        );

        _userRepoMock
            .Setup(repo => repo.FindByEmailAsync(It.Is<Email>(e => e.Value == email), ct))
            .ReturnsAsync(user);

        // Act
        var result = await _useCase.ExecuteAsync(email, ip, ct);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify token was cached (we capture the arguments to verify 15 min expiration)
        _cacheMock.Verify(
            c =>
                c.SetAsync(
                    It.Is<string>(k => k.StartsWith("pwd-reset:")),
                    It.IsAny<byte[]>(),
                    It.Is<DistributedCacheEntryOptions>(opts =>
                        opts.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(15)
                    ),
                    ct
                ),
            Times.Once
        );

        // Verify email was sent
        _emailSenderMock.Verify(
            e =>
                e.SendPasswordResetEmailAsync(
                    email,
                    It.Is<string>(link =>
                        link.StartsWith("https://test.com/reset-password?token=")
                    ),
                    ct
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenRateLimitExceeded_ReturnsFailure()
    {
        // Arrange
        var email = "test@domain.com";
        var ip = "127.0.0.1";
        var ct = CancellationToken.None;

        // Simulate IP having 3 attempts already
        var encodedAttempts = Encoding.UTF8.GetBytes("3");
        _cacheMock.Setup(c => c.GetAsync($"ratelimit:ip:{ip}", ct)).ReturnsAsync(encodedAttempts);

        // Act
        var result = await _useCase.ExecuteAsync(email, ip, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("RateLimitExceeded", result.Error.Code);
    }
}
