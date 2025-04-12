using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using organization_back_end;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Helpers;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Entry;
using organization_back_end.ResponseDto.Entries;
using organization_back_end.Services;
using Xunit;
using File = organization_back_end.Entities.File;

namespace organization_back_end.Tests.Services
{
    public class EntryServiceTests
    {

        private SystemContext CreateMockContext(List<Entry> entries, List<Group> groups, List<File> files)
        {
            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SystemContext(options);
            context.Groups.AddRange(groups);
            context.Entries.AddRange(entries);
            context.Files.AddRange(files);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetEntries_ExistingGroups_ReturnsEntries()
        {
            // Arrange

            var groups = new List<Group>
            {
                new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Group",
                    Description = "Test Description",
                    CreationDate = DateTime.Now,
                    OrganizationId = Guid.NewGuid(),
                }
            };

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = groups[0].Id,
                    Group = groups[0],
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser
                }
            };

            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.GetEntries(groups[0].OrganizationId, groups[0].Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<EntryResponseDto>>(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task CreateEntry_ExistingGroup_Success()
        {
            var groups = new List<Group>
            {
                new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Group",
                    Description = "Test Description",
                    CreationDate = DateTime.Now,
                    OrganizationId = Guid.NewGuid(),
                }
            };

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = groups[0].Id,
                    Group = groups[0],
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser
                }
            };

            var fileMock = new Mock<IFormFile>();

            var addEntryRequest = new AddEntryRequest()
            {
                EntryName = "Test Entry2",
                Text = "Test Description2",
                GroupId = groups[0].Id,
                OrganizationId = groups[0].OrganizationId,
                LinkedEntries = new List<Guid> { entries[0].Id },
                File = fileMock.Object,
                Name = "Test File",
                Extension = ".txt"
            };

            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(licencedUser.Id)).ReturnsAsync(licencedUser);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x =>
                    x.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<AddEntryRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync("Test File");

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var clientsMock = new Mock<IHubClients>();
            var aiServiceMock = new Mock<IAIService>();
            var clientProxyMock = new Mock<IClientProxy>();
            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            clientProxyMock.Setup(cp =>
                    cp.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            await entryService.CreateEntry(addEntryRequest, licencedUser.Id);

            // Assert
            var result = await context.Entries.ToListAsync();
            Assert.NotNull(result);
            Assert.IsType<List<Entry>>(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateEntry_NonExistingGroup_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>();

            var addEntryRequest = new AddEntryRequest()
            {
                EntryName = "Test Entry2",
                Text = "Test Description2",
                GroupId = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
            };

            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() => entryService.CreateEntry(addEntryRequest, licencedUser.Id));
        }

        [Fact]
        public async Task UpdateEntry_ExistingGroup_Success()
        {
            var groups = new List<Group>
            {
                new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Group",
                    Description = "Test Description",
                    CreationDate = DateTime.Now,
                    OrganizationId = Guid.NewGuid(),
                }
            };

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = groups[0].Id,
                    Group = groups[0],
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser
                }
            };

            var fileMock = new Mock<IFormFile>();

            var updateEntryRequest = new UpdateEntryRequest()
            {
                EntryId = entries[0].Id,
                EntryName = "Test Entry2",
                Text = "Test Description2",
                GroupId = groups[0].Id,
                OrganizationId = groups[0].OrganizationId,
                File = fileMock.Object,
                Name = "Test File",
                Extension = ".txt"
            };

            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(licencedUser.Id)).ReturnsAsync(licencedUser);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x =>
                    x.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<AddEntryRequest>(), It.IsAny<Guid>()))
                .ReturnsAsync("Test File");

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var clientsMock = new Mock<IHubClients>();
            var aiServiceMock = new Mock<IAIService>();
            var clientProxyMock = new Mock<IClientProxy>();
            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            clientProxyMock.Setup(cp =>
                    cp.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            await entryService.UpdateEntry(updateEntryRequest, licencedUser.Id);

            // Assert
            var result = await context.Entries.ToListAsync();
            Assert.NotNull(result);
            Assert.IsType<List<Entry>>(result);
            Assert.Single(result);
            Assert.Equal("Test Entry2", result[0].Name);
        }

        [Fact]
        public async Task UpdateEntry_NonExistingEntry_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>();

            var updateEntryRequest = new UpdateEntryRequest()
            {
                EntryId = Guid.NewGuid(),
                EntryName = "Test Entry2",
                Text = "Test Description2",
                GroupId = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
            };

            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() => entryService.UpdateEntry(updateEntryRequest, licencedUser.Id));
        }

        [Fact]
        public async Task UpdateEntry_NotCreator_ThrowsException()
        {
            var groups = new List<Group>
            {
                new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Group",
                    Description = "Test Description",
                    CreationDate = DateTime.Now,
                    OrganizationId = Guid.NewGuid(),
                }
            };

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = groups[0].Id,
                    Group = groups[0],
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser
                }
            };

            var updateEntryRequest = new UpdateEntryRequest()
            {
                EntryId = entries[0].Id,
                EntryName = "Test Entry2",
                Text = "Test Description2",
                GroupId = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
            };

            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.UpdateEntry(updateEntryRequest, Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task DeleteEntry_ExistingEntry_Success()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Group",
                    Description = "Test Description",
                    CreationDate = DateTime.Now,
                    OrganizationId = Guid.NewGuid(),
                }
            };

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var files = new List<File>
            {
                new File
                {
                    Id = Guid.NewGuid(),
                    Name = "Test File",
                    Extension = ".txt",
                    Path = "Test Path",
                    Url = "Test Url"
                }
            };

            var link = Guid.NewGuid();

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = groups[0].Id,
                    Group = groups[0],
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                    FileId = files[0].Id,
                    File = files[0],
                    LinkedEntries = new List<Guid> { link }
                },
                new Entry
                {
                    Id = link,
                    Name = "Test Entry2",
                    Text = "Test Description2",
                    GroupId = groups[0].Id,
                    Group = groups[0],
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                },
            };

            var context = CreateMockContext(entries, groups, files);

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(licencedUser.Id)).ReturnsAsync(licencedUser);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x => x.DeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();
            var aiServiceMock = new Mock<IAIService>();

            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            clientProxyMock.Setup(cp =>
                    cp.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            await entryService.DeleteEntry(entries[0].Id, groups[0].Id, licencedUser.Id);

            // Assert
            var result = await context.Entries.ToListAsync();
            var fileResult = await context.Files.ToListAsync();
            Assert.Single(result);
            Assert.Empty(fileResult);
            fileServiceMock.Verify(x => x.DeleteFileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEntry_NonExistingEntry_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>();


            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.DeleteEntry(Guid.NewGuid(), Guid.NewGuid(), licencedUser.Id));
        }

        [Fact]
        public async Task DeleteEntry_NotCreator_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                }
            };


            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.DeleteEntry(entries[0].Id, entries[0].GroupId, Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task DeleteFile_ExistingFile_Success()
        {
            // Arrange
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = Guid.NewGuid(),
            };

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var file = new File
            {
                Id = Guid.NewGuid(),
                Name = "Test File",
                Extension = ".txt",
                Path = "Test Path",
                Url = "Test Url"
            };

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Test Entry",
                Text = "Test Description",
                GroupId = group.Id,
                Group = group,
                CreationDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                LicencedUserId = licencedUser.Id,
                LicencedUser = licencedUser,
                FileId = file.Id,
                File = file
            };

            var context = CreateMockContext(new List<Entry> { entry }, new List<Group> { group },
                new List<File> { file });

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(licencedUser.Id)).ReturnsAsync(licencedUser);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x => x.DeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();
            var aiServiceMock = new Mock<IAIService>();

            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            clientProxyMock.Setup(cp =>
                    cp.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            await entryService.DeleteFile(entry.Id, group.Id, licencedUser.Id);

            // Assert
            var resultEntry = await context.Entries.FindAsync(entry.Id);
            var resultFile = await context.Files.FindAsync(file.Id);

            Assert.NotNull(resultEntry);
            Assert.Null(resultEntry.FileId);
            Assert.Null(resultEntry.File);
            Assert.Null(resultFile);

            fileServiceMock.Verify(x => x.DeleteFileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteFile_NonExistingEntry_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>();


            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.DeleteFile(Guid.NewGuid(), Guid.NewGuid(), licencedUser.Id));
        }

        [Fact]
        public async Task DeleteFile_NotCreator_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                }
            };


            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.DeleteFile(entries[0].Id, entries[0].GroupId, Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task DeleteFile_NonExistingFile_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entry",
                    Text = "Test Description",
                    GroupId = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                    FileId = Guid.NewGuid()
                }
            };

            var files = new List<File>
            {
                new File
                {
                    Id = Guid.NewGuid(),
                    Name = "Test File",
                    Extension = ".txt",
                    Path = "Test Path",
                    Url = "Test Url"
                }
            };


            var context = CreateMockContext(entries, groups, files);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.DeleteFile(entries[0].Id, entries[0].GroupId, licencedUser.Id));
        }

        [Fact]
        public async Task LinkingEntries_WithValidOrganizationId_ReturnsEntries()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = organizationId,
            };

            var licencedUser = new LicencedUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Surname = "User",
                UserName = "testuser@test.com"
            };

            var entries = new List<Entry>
            {
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Entry 1",
                    GroupId = group.Id,
                    Group = group,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                    CreationDate = DateTime.UtcNow,
                    Text = "Test Text"
                },
                new Entry
                {
                    Id = Guid.NewGuid(),
                    Name = "Entry 2",
                    GroupId = group.Id,
                    Group = group,
                    LicencedUserId = licencedUser.Id,
                    LicencedUser = licencedUser,
                    CreationDate = DateTime.UtcNow,
                    Text = "Test Text"
                }
            };

            var groups = new List<Group> { group };

            var context = CreateMockContext(entries, groups, new List<File>());

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.LinkingEntries(organizationId, entries[0].Id);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Contains(resultList, e => e.Name == "Entry 2");
            Assert.All(resultList, e => Assert.Contains("Test User", e.FullName));
        }

        [Fact]
        public async Task GetGraphEntities_WithValidOrganizationId_ReturnsGraphEntities()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = organizationId,
            };

            var link = Guid.NewGuid();

            var entry1 = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Entry 1",
                GroupId = group.Id,
                Group = group,
                LinkedEntries = new List<Guid> { link },
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entry2 = new Entry
            {
                Id = link,
                Name = "Entry 2",
                GroupId = group.Id,
                Group = group,
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entries = new List<Entry> { entry1, entry2 };
            var groups = new List<Group> { group };

            var context = CreateMockContext(entries, groups, new List<File>());

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.GetGraphEntities(organizationId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);

            var graphEntity1 = resultList.FirstOrDefault(e => e.Name == "Entry 1");
            var graphEntity2 = resultList.FirstOrDefault(e => e.Name == "Entry 2");

            Assert.NotNull(graphEntity1);
            Assert.NotNull(graphEntity2);
            Assert.Equal(entry1.LinkedEntries.Count, graphEntity1!.LinkedEntries!.Count);
            Assert.Empty(graphEntity2!.LinkedEntries!);
        }

        [Fact]
        public async Task GetEntry_ValidEntryIdAndOrganizationId_ReturnsEntryViewResponseDto()
        {
            // Arrange
            var organization = new Organization { Id = Guid.NewGuid(), Name = "Test Org", Description = "Test Description", OwnerId = Guid.NewGuid().ToString() };
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = organization.Id,
                Organization = organization
            };
            
            var user = new LicencedUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = "John",
                Surname = "Doe",
                UserName = "johndoe@test.com"
            };

            var linkedEntry1 = new Entry { Id = Guid.NewGuid(), Name = "Linked Entry 1", GroupId = group.Id, Text = "Test Text", LicencedUserId = user.Id };
            var linkedEntry2 = new Entry { Id = Guid.NewGuid(), Name = "Linked Entry 2", GroupId = group.Id, Text = "Test Text", LicencedUserId = user.Id };

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Test Entry",
                Text = "Test Description",
                CreationDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                GroupId = group.Id,
                Group = group,
                LicencedUserId = user.Id,
                LicencedUser = user,
                LinkedEntries = new List<Guid> { linkedEntry1.Id, linkedEntry2.Id }
            };

            var files = new List<File>();
            var entries = new List<Entry> { entry, linkedEntry1, linkedEntry2 };
            var groups = new List<Group> { group };

            var context = CreateMockContext(entries, groups, files);

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.GetEntry(entry.Id, organization.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry.Id, result.Id);
            Assert.Equal(entry.Name, result.Name);
            Assert.Equal(entry.Text, result.Text);
            Assert.Equal(entry.LicencedUserId, result.LicencedUserId);
            Assert.Equal($"{user.Name} {user.Surname}", result.FullName);
            Assert.Equal(2, result.LinkedEntries!.Count);
            Assert.Contains(result.LinkedEntries, e => e.Id == linkedEntry1.Id);
            Assert.Contains(result.LinkedEntries, e => e.Id == linkedEntry2.Id);
        }

        [Fact]
        public async Task GetEntry_NonExistingEntry_ThrowsException()
        {
            var groups = new List<Group>();

            var licencedUser = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                Surname = "Test User",
                UserName = "testuser@test.com",
            };

            var entries = new List<Entry>();


            var context = CreateMockContext(entries, groups, new List<File>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();
            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.GetEntry(Guid.NewGuid(), Guid.NewGuid()));
        }
        
        [Fact]
        public async Task DownloadFile_ValidEntryAndFile_ReturnsFileStreamResult()
        {
            // Arrange
            var file = new File
            {
                Id = Guid.NewGuid(),
                Name = "TestFile",
                Extension = ".txt",
                Path = "TestPath",
                Url = "TestUrl"
            };

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Test Entry",
                GroupId = Guid.NewGuid(),
                FileId = file.Id,
                File = file,
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var files = new List<File> { file };
            var entries = new List<Entry> { entry };
            var groups = new List<Group>();

            var context = CreateMockContext(entries, groups, files);

            var fileServiceMock = new Mock<IFileService>();
            var expectedStreamResult = new FileStreamResult(new MemoryStream(), "application/octet-stream");

            fileServiceMock
                .Setup(fs => fs.DownloadFileAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedStreamResult);

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object, hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.DownloadFile(entry.GroupId, entry.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<FileStreamResult>(result);
            Assert.Equal(expectedStreamResult, result);

            fileServiceMock.Verify(fs => fs.DownloadFileAsync(It.Is<string>(s => s.Contains(file.Id.ToString()))), Times.Once);
        }
        
        [Fact]
        public async Task DownloadFile_NonExistingEntry_ThrowsError()
        {
            // Arrange
            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Test Entry",
                GroupId = Guid.NewGuid(),
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var files = new List<File>();
            var entries = new List<Entry> { entry };
            var groups = new List<Group>();

            var context = CreateMockContext(entries, groups, files);

            var fileServiceMock = new Mock<IFileService>();
            var expectedStreamResult = new FileStreamResult(new MemoryStream(), "application/octet-stream");

            fileServiceMock
                .Setup(fs => fs.DownloadFileAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedStreamResult);

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();
            var aiServiceMock = new Mock<IAIService>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object, hubContextMock.Object, aiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                entryService.DownloadFile(Guid.NewGuid(), Guid.NewGuid()));

        }

        [Fact]
        public async Task AnalyzeWithAi_WithValidData_AIResponse()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = organizationId,
            };

            var link = Guid.NewGuid();

            var entry1 = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Entry 1",
                GroupId = group.Id,
                Group = group,
                LinkedEntries = new List<Guid> { link },
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entry2 = new Entry
            {
                Id = link,
                Name = "EYS gene",
                GroupId = group.Id,
                Group = group,
                Text = "EYS gene is very important",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entries = new List<Entry> { entry1, entry2 };
            var groups = new List<Group> { group };

            var context = CreateMockContext(entries, groups, new List<File>());

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();

            var aiResponse = new AIHelpers.GeminiResponse
            {
                Success = true,
                Data = "EYS gene is very important"
            };

            aiServiceMock
                .Setup(ai => ai.GetResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(aiResponse);

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.AnalyzeWithAi("EYS", group.Id, entry1.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AnalyzeWithAi_NoText_ErrorResponse()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = organizationId,
            };

            var link = Guid.NewGuid();

            var entry1 = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Entry 1",
                GroupId = group.Id,
                Group = group,
                LinkedEntries = new List<Guid> { link },
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entry2 = new Entry
            {
                Id = link,
                Name = "EYS gene",
                GroupId = group.Id,
                Group = group,
                Text = "EYS gene is very important",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entries = new List<Entry> { entry1, entry2 };
            var groups = new List<Group> { group };

            var context = CreateMockContext(entries, groups, new List<File>());

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act
            var result = await entryService.AnalyzeWithAi("", group.Id, entry1.Id);

            var jsonResponse = new[]
            {
                new
                {
                    title = "No links",
                    reason = "No text provided"
                }
            };

            var serializedResponse = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serializedResponse, result);
        }

        [Fact]
        public async Task AnalyzeWithAi_WrongGroupId_ErrorResponse()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "Test Group",
                Description = "Test Description",
                CreationDate = DateTime.Now,
                OrganizationId = organizationId,
            };

            var link = Guid.NewGuid();

            var entry1 = new Entry
            {
                Id = Guid.NewGuid(),
                Name = "Entry 1",
                GroupId = group.Id,
                Group = group,
                LinkedEntries = new List<Guid> { link },
                Text = "Test Text",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entry2 = new Entry
            {
                Id = link,
                Name = "EYS gene",
                GroupId = group.Id,
                Group = group,
                Text = "EYS gene is very important",
                LicencedUserId = Guid.NewGuid().ToString()
            };

            var entries = new List<Entry> { entry1, entry2 };
            var groups = new List<Group> { group };

            var context = CreateMockContext(entries, groups, new List<File>());

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var fileServiceMock = new Mock<IFileService>();
            var aiServiceMock = new Mock<IAIService>();

            var hubContextMock = new Mock<IHubContext<EntriesHub>>();

            var entryService = new EntryService(context, userManagerMock.Object, fileServiceMock.Object,
                hubContextMock.Object, aiServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => entryService.AnalyzeWithAi("", Guid.NewGuid(), entry1.Id));
        }

    }
}
