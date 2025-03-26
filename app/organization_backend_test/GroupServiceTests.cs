using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Group;
using organization_back_end.Services;
using Group = organization_back_end.Entities.Group;

namespace organization_back_end.Tests.Services
{
    public class GroupServiceTests
    {
        private SystemContext CreateMockContext(List<Organization> organizations, List<Group> groups, List<Entry> entries)
        {
            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SystemContext(options);
            context.Organizations.AddRange(organizations);
            context.Groups.AddRange(groups);
            context.Entries.AddRange(entries);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task CreateGroup_ExistingOrganization_Success()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Org",
                Groups = new List<Group>(),
                Description = "Test Description",
                OwnerId = Guid.NewGuid().ToString()

            };

            var organizations = new List<Organization> { organization };
            var groups = new List<Group>();
            var entries = new List<Entry>();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            var request = new AddGroupRequest
            {
                OrganizationId = organizationId,
                Name = "New Group",
                Description = "Test Group"
            };

            // Act
            await service.CreateGroup(request);

            // Assert
            var createdGroup = context.Groups.FirstOrDefault();
            Assert.NotNull(createdGroup);
            Assert.Equal("New Group", createdGroup.Name);
            Assert.Equal("Test Group", createdGroup.Description);
            Assert.Equal(organizationId, createdGroup.OrganizationId);
            Assert.Contains(createdGroup, organization.Groups);
        }

        [Fact]
        public async Task CreateGroup_NonExistingOrganization_ThrowsException()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var organizations = new List<Organization>();
            var groups = new List<Group>();
            var entries = new List<Entry>();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            var request = new AddGroupRequest
            {
                OrganizationId = organizationId,
                Name = "New Group",
                Description = "Test Group"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateGroup(request));
        }

        [Fact]
        public async Task GetGroups_ExistingOrganization_ReturnsGroups()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var organization = new Organization { 
                Id = organizationId, 
                Name = "Test Org",
                Description = "Test Description",
                OwnerId = Guid.NewGuid().ToString()
            };
            var groupId = Guid.NewGuid();

            var groups = new List<Group>
            {
                new Group
                {
                    Id = groupId,
                    Name = "Group 1",
                    OrganizationId = organizationId,
                    Description = "Test Description",
                    Entries = new List<Entry> {
                    new Entry { Id = Guid.NewGuid(), GroupId = groupId, Name = "Entry1", Text = "Text", LicencedUserId = Guid.NewGuid().ToString() },
                    new Entry { Id = Guid.NewGuid(), GroupId = groupId, Name = "Entry2", Text = "Text", LicencedUserId = Guid.NewGuid().ToString() }
                    }
                },
                new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Group 2",
                    OrganizationId = organizationId,
                    Description = "Test Description",
                    Entries = new List<Entry> { }
                }
            };

            var organizations = new List<Organization> { organization };
            var entries = groups.SelectMany(g => g.Entries).ToList();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            // Act
            var result = await service.GetGroups(organizationId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, g => g.Name == "Group 1" && g.EntriesCount == 2);
            Assert.Contains(result, g => g.Name == "Group 2" && g.EntriesCount == 0);
        }

        [Fact]
        public async Task UpdateGroup_ExistingGroup_Success()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group
            {
                Id = groupId,
                Name = "Old Group",
                Description = "Old Description",
                OrganizationId = organizationId
            };

            var organizations = new List<Organization>
            {
                new Organization { Id = organizationId, Name = "Test Org", Description = "Test", OwnerId = Guid.NewGuid().ToString() }
            };
            var groups = new List<Group> { group };
            var entries = new List<Entry>();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            var request = new UpdateGroupRequest
            {
                GroupId = groupId,
                OrganizationId = organizationId,
                Name = "Updated Group",
                Description = "Updated Description"
            };

            // Act
            await service.UpdateGroup(request);

            // Assert
            var updatedGroup = context.Groups.FirstOrDefault(g => g.Id == groupId);
            Assert.NotNull(updatedGroup);
            Assert.Equal("Updated Group", updatedGroup.Name);
            Assert.Equal("Updated Description", updatedGroup.Description);
        }

        [Fact]
        public async Task UpdateGroup_NonExistingGroup_ThrowsException()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var organizations = new List<Organization>
            {
                new Organization { Id = organizationId, Name = "Test Org", Description = "Test", OwnerId = Guid.NewGuid().ToString() }
            };
            var groups = new List<Group>();
            var entries = new List<Entry>();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            var request = new UpdateGroupRequest
            {
                GroupId = groupId,
                OrganizationId = organizationId,
                Name = "Updated Group",
                Description = "Updated Description"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UpdateGroup(request));
        }

        [Fact]
        public async Task DeleteGroup_Success()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Org",
                Groups = new List<Group>(),
                Description = "Test Description",
                OwnerId = Guid.NewGuid().ToString()
            };

            var entry1Id = Guid.NewGuid();
            var entry2Id = Guid.NewGuid();
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();

            var group = new Group
            {
                Id = groupId,
                Name = "Group to Delete",
                OrganizationId = organizationId,
                Organization = organization,
                Description = "Test Description",
                Entries = new List<Entry>()
            };

            organization.Groups.Add(group);
            var organizations = new List<Organization> { organization };
            var groups = new List<Group> { group };
            var entries = group.Entries.ToList();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            entryServiceMock
                .Setup(x => x.DeleteEntry(entry1Id, groupId, userId1))
                .Returns(Task.CompletedTask);
            entryServiceMock
                .Setup(x => x.DeleteEntry(entry2Id, groupId, userId2))
                .Returns(Task.CompletedTask);

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            var request = new DeleteGroupRequest
            {
                GroupId = groupId,
                OrganizationId = organizationId
            };

            // Act
            await service.DeleteGroup(request);

            // Assert
            var deletedGroup = context.Groups.FirstOrDefault(g => g.Id == groupId);
            Assert.Null(deletedGroup);

            Assert.DoesNotContain(group, organization.Groups);
        }

        [Fact]
        public async Task DeleteGroup_Success_WithoutRequest()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var organization = new Organization
            {
                Id = organizationId,
                Name = "Test Org",
                Groups = new List<Group>(),
                Description = "Test Description",
                OwnerId = Guid.NewGuid().ToString()
            };

            var entry1Id = Guid.NewGuid();
            var entry2Id = Guid.NewGuid();
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();

            var group = new Group
            {
                Id = groupId,
                Name = "Group to Delete",
                OrganizationId = organizationId,
                Organization = organization,
                Description = "Test Description",
                Entries = new List<Entry>()
            };

            organization.Groups.Add(group);
            var organizations = new List<Organization> { organization };
            var groups = new List<Group> { group };
            var entries = group.Entries.ToList();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            entryServiceMock
                .Setup(x => x.DeleteEntry(entry1Id, groupId, userId1))
                .Returns(Task.CompletedTask);
            entryServiceMock
                .Setup(x => x.DeleteEntry(entry2Id, groupId, userId2))
                .Returns(Task.CompletedTask);

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            // Act
            await service.DeleteGroup(groupId, organizationId);

            // Assert
            var deletedGroup = context.Groups.FirstOrDefault(g => g.Id == groupId);
            Assert.Null(deletedGroup);

            Assert.DoesNotContain(group, organization.Groups);
        }

        [Fact]
        public async Task DeleteGroup_NonExistingGroup_ThrowsException()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var organizations = new List<Organization>
            {
                new Organization { Id = organizationId, Name = "Test Org", Description = "Test", OwnerId = Guid.NewGuid().ToString() }
            };
            var groups = new List<Group>();
            var entries = new List<Entry>();

            var context = CreateMockContext(organizations, groups, entries);

            var entryServiceMock = new Mock<IEntryService>();
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new GroupService(context, entryServiceMock.Object, userManagerMock.Object);

            var request = new DeleteGroupRequest
            {
                GroupId = groupId,
                OrganizationId = organizationId
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeleteGroup(request));
        }
    }
}