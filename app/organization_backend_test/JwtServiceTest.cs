using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using organization_back_end.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace organization_back_end.Tests.Auth
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            // Setup mock configuration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration
                .Setup(x => x["Jwt:Secret"])
                .Returns("ThisIsAVeryLongSecretKeyForTestingPurposes123!");
            _mockConfiguration
                .Setup(x => x["Jwt:ValidAudience"])
                .Returns("TestAudience");
            _mockConfiguration
                .Setup(x => x["Jwt:ValidIssuer"])
                .Returns("TestIssuer");

            // Create JwtService with mock configuration
            _jwtService = new JwtService(_mockConfiguration.Object);
        }

        [Fact]
        public void CreateAccessToken_ShouldGenerateValidToken()
        {
            // Arrange
            string username = "testuser";
            string userId = "12345";
            var roles = new[] { "Admin", "User" };

            // Act
            string token = _jwtService.CreateAccessToken(username, userId, roles);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            // Validate token details
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidIssuer = "TestIssuer",
                ValidAudience = "TestAudience",
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_mockConfiguration.Object["Jwt:Secret"])),
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, validationParams, out _);

            // Check claims
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Name && c.Value == username);
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.System && c.Value == userId);
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
        }

        [Fact]
        public void CreateRefreshToken_ShouldGenerateValidToken()
        {
            // Arrange
            Guid sessionId = Guid.NewGuid();
            string userId = "12345";
            DateTime expires = DateTime.UtcNow.AddDays(7);

            // Act
            string refreshToken = _jwtService.CreateRefreshToken(sessionId, userId, expires);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(refreshToken));

            // Validate token details
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidIssuer = "TestIssuer",
                ValidAudience = "TestAudience",
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_mockConfiguration.Object["Jwt:Secret"])),
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(refreshToken, validationParams, out var token);

            // Check claims
            Assert.Contains(principal.Claims, c => c.Type == "SessionId" && c.Value == sessionId.ToString());
        }

        [Fact]
        public void TryParseRefreshToken_ValidToken_ShouldReturnTrue()
        {
            // Arrange
            Guid sessionId = Guid.NewGuid();
            string userId = "12345";
            DateTime expires = DateTime.UtcNow.AddDays(7);

            // Create a valid refresh token
            string validRefreshToken = _jwtService.CreateRefreshToken(sessionId, userId, expires);

            // Act
            bool result = _jwtService.TryParseRefreshToken(validRefreshToken, out ClaimsPrincipal? claims);

            // Assert
            Assert.True(result);
            Assert.NotNull(claims);
            Assert.Contains(claims.Claims, c => c.Type == "SessionId" && c.Value == sessionId.ToString());
        }

        [Fact]
        public void TryParseRefreshToken_ExpiredToken_ShouldReturnFalse()
        {
            // Arrange
            Guid sessionId = Guid.NewGuid();
            string userId = "12345";
            DateTime expires = DateTime.UtcNow.AddDays(-1); // Expired token

            // Create an expired refresh token
            string expiredRefreshToken = _jwtService.CreateRefreshToken(sessionId, userId, expires);

            // Act
            bool result = _jwtService.TryParseRefreshToken(expiredRefreshToken, out ClaimsPrincipal? claims);

            // Assert
            Assert.False(result);
            Assert.Null(claims);
        }

        [Fact]
        public void TryParseRefreshToken_InvalidToken_ShouldReturnFalse()
        {
            // Arrange
            string invalidToken = "invalid.token.here";

            // Act
            bool result = _jwtService.TryParseRefreshToken(invalidToken, out ClaimsPrincipal? claims);

            // Assert
            Assert.False(result);
            Assert.Null(claims);
        }
    }
}