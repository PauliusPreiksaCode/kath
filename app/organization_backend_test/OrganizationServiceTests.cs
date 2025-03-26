using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Organization;
using organization_back_end.Services;

namespace organization_back_end.Tests.Services
{
    public class OrganizationServiceTests
    {
        private SystemContext CreateMockContext(List<Organization> organizations, List<User> users, List<OrganizationUser> organizationUsers)
        {
            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SystemContext(options);
            context.Organizations.AddRange(organizations);
            context.Users.AddRange(users);
            context.OrganizationUsers.AddRange(organizationUsers);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetOrganizations_ExistingUser_ReturnsOrganizations()
        {
            // Arrange
            var userId = "user1";
            var organizations = new List<Organization>
            {
                new Organization { Id = Guid.NewGuid(), Name = "Org 1", OwnerId = userId, Description = "Org 1 des" },
                new Organization { Id = Guid.NewGuid(), Name = "Org 2", OwnerId = "user2", Description = "Org 2 des" }
            };

            var organizationUsers = new List<OrganizationUser>
            {
                new OrganizationUser { Id = Guid.NewGuid(), UserId = userId, OrganizationId = organizations[0].Id }
            };

            var context = CreateMockContext(organizations, new List<User>(), organizationUsers);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();
            var organizationService = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            var result = await organizationService.GetOrganizations(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Org 1", result.First().Name);
        }

        [Fact]
        public async Task IsUserOrganizationOwner_UserIsOwner_ReturnsTrue()
        {
            // Arrange
            var userId = "owner1";
            var organization = new Organization { Id = Guid.NewGuid(), Name = "Test Org", OwnerId = userId, Description = "Org des" };
            var context = CreateMockContext(new List<Organization> { organization }, new List<User>(), new List<OrganizationUser>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();
            var organizationService = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            var result = await organizationService.IsUserOrganizationOwner(userId, organization.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsUserOrganizationOwner_UserIsNotOwner_ReturnsFalse()
        {
            // Arrange
            var userId = "user1";
            var organization = new Organization { Id = Guid.NewGuid(), Name = "Test Org", OwnerId = "owner1", Description = "Org des" };
            var context = CreateMockContext(new List<Organization> { organization }, new List<User>(), new List<OrganizationUser>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();
            var organizationService = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            var result = await organizationService.IsUserOrganizationOwner(userId, organization.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateOrganization_Success()
        {
            // Arrange
            var userId = "user1";
            var request = new CreateOrganizationRequest { Name = "New Org", Description = "New Organization Description" };
            var user = new OrganizationOwner { Id = userId, UserName = "test@example.com", Name = "Test", Surname = "User" };

            var users = new List<User> { user };
            var organizations = new List<Organization>();
            var organizationUsers = new List<OrganizationUser>();

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.CreateOrganization(userId, request, user);

            // Assert
            var organization = context.Organizations.FirstOrDefault();
            Assert.NotNull(organization);
            Assert.Equal("New Org", organization.Name);
            Assert.Equal(userId, organization.OwnerId);
        }

        [Fact]
        public async Task AddUserToOrganization_Success()
        {
            // Arrange
            var userId = "user1";
            var organizationId = Guid.NewGuid();
            var user = new LicencedUser { Id = userId, UserName = "user@example.com", Name = "Test User", Surname = "Test User" };
            var organization = new Organization { Id = organizationId, Name = "Test Organization", Description = "Test", OwnerId = userId };
            var organizations = new List<Organization> { organization };
            var users = new List<User> { user };
            var organizationUsers = new List<OrganizationUser>();

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.AddUserToOrganization(userId, organizationId);

            // Assert
            var organizationUser = context.OrganizationUsers.FirstOrDefault();
            Assert.NotNull(organizationUser);
            Assert.Equal(userId, organizationUser.UserId);
            emailServiceMock.Verify(es => es.SendInvitationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddUserToOrganization_ContainsUser()
        {
            // Arrange
            var userId = "user1";
            var organizationId = Guid.NewGuid();
            var user = new LicencedUser { Id = userId, UserName = "user@example.com", Name = "Test User", Surname = "Test User" };
            var organization = new Organization { Id = organizationId, Name = "Test Organization", Description = "Test", OwnerId = userId };
            var organizations = new List<Organization> { organization };
            var users = new List<User> { user };
            var organizationUsers = new List<OrganizationUser> { new OrganizationUser { Id = Guid.NewGuid(), OrganizationId = organizationId, UserId = userId, User = user } };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.AddUserToOrganization(userId, organizationId);

            // Assert
            var organizationUser = context.OrganizationUsers.FirstOrDefault();
            Assert.NotNull(organizationUser);
            Assert.Equal(userId, organizationUser.UserId);
        }

        [Fact]
        public async Task RemoveUserFromOrganization_Success()
        {
            // Arrange
            var userId = "user1";
            var organizationId = Guid.NewGuid();
            var user = new LicencedUser { Id = userId, UserName = "user@example.com", Name = "Test User", Surname = "Test user" };
            var organizationUser = new OrganizationUser { UserId = userId, OrganizationId = organizationId, User = user };
            var organization = new Organization { Id = organizationId, Name = "Test Organization", Description = "Test", OwnerId = userId };
            var organizations = new List<Organization> { organization };
            var users = new List<User> { user };
            var organizationUsers = new List<OrganizationUser> { organizationUser };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.RemoveUserFromOrganization(userId, organizationId);

            // Assert
            var removedOrganizationUser = context.OrganizationUsers.FirstOrDefault();
            Assert.Null(removedOrganizationUser); // The user should be removed from the OrganizationUser table
        }

        [Fact]
        public async Task RemoveUserFromOrganization_NotExist()
        {
            // Arrange
            var userId = "user1";
            var organizationId = Guid.NewGuid();
            var user = new LicencedUser { Id = userId, UserName = "user@example.com", Name = "Test User", Surname = "Test user" };
            var organizationUser = new OrganizationUser { UserId = userId, OrganizationId = organizationId, User = user };
            var organization = new Organization { Id = organizationId, Name = "Test Organization", Description = "Test", OwnerId = userId };
            var organizations = new List<Organization> { organization };
            var users = new List<User> { user };
            var organizationUsers = new List<OrganizationUser>();

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.RemoveUserFromOrganization("user2", organizationId);

            // Assert
            var removedOrganizationUser = context.OrganizationUsers.FirstOrDefault();
            Assert.Null(removedOrganizationUser); // The user should be removed from the OrganizationUser table
        }

        [Fact]
        public async Task UpdateOrganization_Success()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var organizationId = Guid.NewGuid();
            var organization = new Organization { Id = organizationId, Name = "Old Org", Description = "Old Description", OwnerId = ownerId };
            var organizations = new List<Organization> { organization };
            var users = new List<User>();
            var organizationUsers = new List<OrganizationUser>();

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.UpdateOrganization(organizationId, "Updated Org", "Updated Description");

            // Assert
            var updatedOrganization = context.Organizations.FirstOrDefault(x => x.Id == organizationId);
            Assert.NotNull(updatedOrganization);
            Assert.Equal("Updated Org", updatedOrganization.Name);
            Assert.Equal("Updated Description", updatedOrganization.Description);
        }

        [Fact]
        public async Task DeleteOrganization_Success()
        {
            // Arrange
            var ownerId = Guid.NewGuid().ToString();
            var organizationId = Guid.NewGuid();
            var organization = new Organization { Id = organizationId, Name = "Org to Delete", Description = "Delete Me", OwnerId = ownerId };
            var organizations = new List<Organization> { organization };
            var users = new List<User>();
            var organizationUsers = new List<OrganizationUser>();

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            await service.DeleteOrganization(organizationId);

            // Assert
            var deletedOrganization = context.Organizations.FirstOrDefault(x => x.Id == organizationId);
            Assert.Null(deletedOrganization); // The organization should be deleted
        }

        [Fact]
        public async Task GetOrganizationUsers_Success()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var userId = "user1";

            var users = new List<User>
            {
                new LicencedUser { Id = userId, UserName = "user@example.com", Name = "User1", Surname = "User1" },
                new LicencedUser { Id = "user2", UserName = "user2@example.com", Name = "User2", Surname = "User2" }
            };

            var organizationUsers = new List<OrganizationUser>
            {
                new OrganizationUser { UserId = userId, OrganizationId = organizationId, User = users[0] as LicencedUser },
                new OrganizationUser { UserId = "user2", OrganizationId = organizationId, User = users[1] as LicencedUser }
            };

            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Org",
                Description = "Test Org Description",
                OwnerId = userId,
                Users = new List<OrganizationUser>
                {
                    organizationUsers[0],
                    organizationUsers[1]
                }
                
            };

            var organizations = new List<Organization> { organization };
            

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            var result = await service.GetOrganizationUsers(organizationId, userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("user2", result.First().Id);
        }

        [Fact]
        public async Task GetOrganizationUsers_EmptyOrganization()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var userId = "user1";

            var users = new List<User>();

            var organizationUsers = new List<OrganizationUser>();

            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Org",
                Description = "Test Org Description",
                OwnerId = userId,
                Users = new List<OrganizationUser>()
            };

            var organizations = new List<Organization> { organization };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var groupServiceMock = new Mock<IGroupService>();
            var emailServiceMock = new Mock<IEmailService>();

            var context = CreateMockContext(organizations, users, organizationUsers);
            var service = new OrganizationService(context, userManagerMock.Object, groupServiceMock.Object, emailServiceMock.Object);

            // Act
            var result = await service.GetOrganizationUsers(Guid.NewGuid(), userId);

            // Assert
            Assert.Equal(0, result.Count);
        }
    }
}
