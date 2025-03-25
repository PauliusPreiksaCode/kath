using Microsoft.AspNetCore.Identity;
using Moq;
using organization_back_end.Auth.Model;
using organization_back_end.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organization_back_end.Tests.Auth
{
    public class AuthSeederTest
    {
        [Fact]
        public async Task SeedAsync_ShouldCallAddDefaultRoles()
        {
            // Arrange
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
            var authSeeder = new AuthSeeder(mockRoleManager.Object);

            mockRoleManager.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
                           .ReturnsAsync(IdentityResult.Success);

            // Act
            await authSeeder.SeedAsync();

            // Assert
            foreach (var role in Roles.All)
            {
                mockRoleManager.Verify(rm => rm.RoleExistsAsync(role), Times.Once);
                mockRoleManager.Verify(rm => rm.CreateAsync(It.Is<IdentityRole>(r => r.Name == role)), Times.Once);
            }
        }
    }
}
