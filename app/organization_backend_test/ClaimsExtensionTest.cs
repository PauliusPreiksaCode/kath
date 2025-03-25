using organization_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace organization_backend.Test.Helpers
{
    public class ClaimsExtensionTest
    {
        [Fact]
        public void GetUserId_ValidClaimsPrincipal_ShouldReturnUserId()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.System, "12345") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var userId = principal.GetUserId();

            // Assert
            Assert.Equal("12345", userId);
        }

        [Fact]
        public void GetUserId_NullClaimsPrincipal_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal principal = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => principal.GetUserId());
        }

        [Fact]
        public void GetUserEmail_ValidClaimsPrincipal_ShouldReturnEmail()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "user@example.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var email = principal.GetUserEmail();

            // Assert
            Assert.Equal("user@example.com", email);
        }

        [Fact]
        public void GetUserEmail_NullClaimsPrincipal_ShouldThrowArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal principal = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => principal.GetUserEmail());
        }
    }
}
