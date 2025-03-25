using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using organization_back_end.Entities;
using organization_back_end.Auth;
using organization_back_end.Helpers;
using System.Linq;

namespace organization_back_end.Tests.Auth
{
    public class SessionServiceTests
    {
        private readonly Mock<SystemContext> _mockContext;
        private readonly List<Session> _sessionData;
        private readonly SessionService _sessionService;

        public SessionServiceTests()
        {
            _sessionData = new List<Session>();
            _mockContext = new Mock<SystemContext>();
            var mockSessionSet = CreateMockDbSet(_sessionData);

            _mockContext.Setup(x => x.Session).Returns(mockSessionSet.Object);
            _mockContext
                .Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);

            _sessionService = new SessionService(_mockContext.Object);
        }

        // Helper method to create a mockable DbSet
        private static Mock<DbSet<Session>> CreateMockDbSet(List<Session> data)
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<Session>>();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids =>
                {
                    var session = data.FirstOrDefault(d => d.Id == (Guid)ids[0]);
                    return ValueTask.FromResult(session);
                });

            mockSet.Setup(m => m.Add(It.IsAny<Session>()))
                .Callback<Session>(data.Add)
                .Returns<Session>(s => null);

            return mockSet;
        }

        [Fact]
        public async Task ExtendSessionAsync_ShouldUpdateSessionDetails()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var refreshToken = "newRefreshToken";
            var newExpiresAt = DateTime.UtcNow.AddDays(14);

            // Prepare existing session in the mock dataset
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "user123",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                LastRefreshToken = "oldRefreshToken".ToSHA256(),
                IsRevoked = false,
                InitiatedAt = DateTimeOffset.UtcNow
            };

            // Add the session to the data list
            _sessionData.Add(existingSession);

            // Act
            await _sessionService.ExtendSessionAsync(sessionId, refreshToken, newExpiresAt);

            // Assert
            var updatedSession = _sessionData.FirstOrDefault(s => s.Id == sessionId);
            Assert.NotNull(updatedSession);
            Assert.Equal(newExpiresAt, updatedSession.ExpiresAt);
            Assert.Equal(refreshToken.ToSHA256(), updatedSession.LastRefreshToken);
        }

        [Fact]
        public async Task ExtendSessionAsync_NonExistentSession_ShouldThrowException()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var refreshToken = "newRefreshToken";
            var newExpiresAt = DateTime.UtcNow.AddDays(14);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _sessionService.ExtendSessionAsync(sessionId, refreshToken, newExpiresAt));
        }

        [Fact]
        public async Task InvalidateSessionAsync_ShouldMarkSessionAsRevoked()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "user123",
                IsRevoked = false,
                InitiatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                LastRefreshToken = "someToken"
            };

            // Add the session to the data list
            _sessionData.Add(existingSession);

            // Act
            await _sessionService.InvalidateSessionAsync(sessionId);

            // Assert
            var updatedSession = _sessionData.FirstOrDefault(s => s.Id == sessionId);
            Assert.NotNull(updatedSession);
            Assert.True(updatedSession.IsRevoked);
        }

        [Fact]
        public async Task IsSessionValidAsync_ValidSession_ShouldReturnTrue()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var refreshToken = "validRefreshToken";
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "user123",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                InitiatedAt = DateTimeOffset.UtcNow,
                IsRevoked = false,
                LastRefreshToken = refreshToken.ToSHA256()
            };

            // Add the session to the data list
            _sessionData.Add(existingSession);

            // Act
            var isValid = await _sessionService.IsSessionValidAsync(sessionId, refreshToken);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task IsSessionValidAsync_ExpiredSession_ShouldReturnFalse()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var refreshToken = "validRefreshToken";
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "user123",
                ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired
                InitiatedAt = DateTimeOffset.UtcNow.AddDays(-2),
                IsRevoked = false,
                LastRefreshToken = refreshToken.ToSHA256()
            };

            // Act
            var isValid = await _sessionService.IsSessionValidAsync(sessionId, refreshToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task IsSessionValidAsync_RevokedSession_ShouldReturnFalse()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var refreshToken = "validRefreshToken";
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "user123",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                InitiatedAt = DateTimeOffset.UtcNow,
                IsRevoked = true,
                LastRefreshToken = refreshToken.ToSHA256()
            };

            // Act
            var isValid = await _sessionService.IsSessionValidAsync(sessionId, refreshToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task IsSessionValidAsync_InvalidRefreshToken_ShouldReturnFalse()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var refreshToken = "validRefreshToken";
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "user123",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                InitiatedAt = DateTimeOffset.UtcNow,
                IsRevoked = false,
                LastRefreshToken = "differentRefreshToken".ToSHA256()
            };

            // Act
            var isValid = await _sessionService.IsSessionValidAsync(sessionId, refreshToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task CreateSessionAsync_ShouldAddNewSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = "testUser";
            var refreshToken = "testRefreshToken";
            var expiresAt = DateTime.UtcNow.AddDays(7);

            // Act
            await _sessionService.createSessionAsync(sessionId, userId, refreshToken, expiresAt);

            // Assert
            var createdSession = _sessionData.FirstOrDefault(s => s.Id == sessionId);
            Assert.NotNull(createdSession);
            Assert.Equal(userId, createdSession.UserId);
            Assert.Equal(refreshToken.ToSHA256(), createdSession.LastRefreshToken);
            Assert.Equal(expiresAt, createdSession.ExpiresAt);
            Assert.False(createdSession.IsRevoked);
        }

        [Fact]
        public async Task InvalidateSessionAsync_ShouldNotMarkSessionAsRevoked()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var existingSession = new Session
            {
                Id = sessionId,
                UserId = "testUser",
                IsRevoked = false,
                InitiatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                LastRefreshToken = "someToken"
            };

            _sessionData.Add(existingSession);

            // Act
            await _sessionService.InvalidateSessionAsync(Guid.NewGuid());

            // Assert
            var updatedSession = _sessionData.FirstOrDefault(s => s.Id == sessionId);
            Assert.NotNull(updatedSession);
            Assert.False(updatedSession.IsRevoked);
        }
    }
}