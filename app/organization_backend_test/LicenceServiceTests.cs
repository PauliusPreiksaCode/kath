
using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Enums;
using organization_back_end.RequestDtos.Licences;
using organization_back_end.Services;
using File = System.IO.File;
using Session = Stripe.Checkout.Session;

namespace organization_back_end.Tests.Services
{
    public class LicenceServiceTests
    {
        private SystemContext CreateMockContext(List<Licence> licences, List<LicenceLedgerEntry>? ledgerEntries = null, List<User>? users = null)
        {
            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            ledgerEntries ??= new List<LicenceLedgerEntry>();
            users ??= new List<User>();

            var context = new SystemContext(options);
            context.Licences.AddRange(licences);
            context.LicenceLedgerEntries.AddRange(ledgerEntries);
            context.Users.AddRange(users);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GenerateLicense_ValidRequest_Success()
        {
            // Arrange
            var request = new CreateLicenceRequest
            {
                Name = "Test Licence",
                Price = 10,
                Type = LicenceType.User,
                Duration = 30
            };

            var context = CreateMockContext(new List<Licence>());
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.GenerateLicense(request);

            // Assert
            var licence = await context.Licences.FirstOrDefaultAsync();
            Assert.NotNull(licence);
            Assert.Equal(request.Name, licence.Name);
            Assert.Equal(request.Price, licence.Price);
            Assert.Equal(request.Type, licence.Type);
            Assert.Equal(request.Duration, licence.Duration);
        }

        [Fact]
        public async Task GetAllLicences_ValidRequest_Success()
        {
            var licences = new List<Licence>
            {
                new Licence
                {
                    Name = "Test Licence 1",
                    Price = 10,
                    Type = LicenceType.User,
                    Duration = 30
                },
                new Licence
                {
                    Name = "Test Licence 2",
                    Price = 20,
                    Type = LicenceType.User,
                    Duration = 60
                }
            };

            var context = CreateMockContext(licences);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            var result = await licenceService.GetAllLicences();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetLicenceById_ValidRequest_Success()
        {
            var licences = new List<Licence>
            {
                new Licence
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Licence 1",
                    Price = 10,
                    Type = LicenceType.User,
                    Duration = 30
                },
            };

            var context = CreateMockContext(licences);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            var result = await licenceService.GetLicenceById(licences.First().Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(licences.First().Id, result.Id);
        }

        [Fact]
        public async Task CreateInitialLicenceLedgerEntry_ValidRequest_Success()
        {
            var licence = new Licence
            {
                Id = Guid.NewGuid(),
                Name = "Test Licence 1",
                Price = 10,
                Type = LicenceType.User,
                Duration = 30
            };

            var user = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Name = "Test User",
                Surname = "Test Surname",
            };

            var context = CreateMockContext(new List<Licence> { licence });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            var result = await licenceService.CreateInitialLicenceLedgerEntry(licence, user.Id);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            Assert.NotNull(await context.LicenceLedgerEntries.FirstOrDefaultAsync(l => l.UserId.Equals(user.Id)));
            Assert.Equal(1, await context.LicenceLedgerEntries.CountAsync(l => l.UserId.Equals(user.Id)));
        }

        [Fact]
        public async Task CreateInitialLicenceLedgerEntry_NotValidRequest_RandomGuid()
        {
            var licence = new Licence
            {
                Id = Guid.NewGuid(),
                Name = "Test Licence 1",
                Price = 10,
                Type = LicenceType.User,
                Duration = 30
            };

            var user = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Name = "Test User",
                Surname = "Test Surname",
            };

            var context = CreateMockContext(new List<Licence> { licence });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            var result = await licenceService.CreateInitialLicenceLedgerEntry(licence, Guid.NewGuid().ToString());

            // Assert
            Assert.NotEqual(1, await context.LicenceLedgerEntries.CountAsync(l => l.UserId.Equals(user.Id)));
        }

        [Fact]
        public async Task GetLicenceLedgerEntries_ValidRequest_Success()
        {
            var licence = new Licence
            {
                Id = Guid.NewGuid(),
                Name = "Test Licence 1",
                Price = 10,
                Type = LicenceType.User,
                Duration = 30
            };

            var user = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Name = "Test User",
                Surname = "Test Surname",
            };

            var context = CreateMockContext(new List<Licence> { licence });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act & Assert
            Assert.Empty(await licenceService.GetLicenceLedgerEntries(user.Id));

            // Act
            await licenceService.CreateInitialLicenceLedgerEntry(licence, user.Id);
            var result = await licenceService.GetLicenceLedgerEntries(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task HasRole_ValidUserAndRole_ReturnsTrue()
        {
            // Arrange
            var user = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Name = "Test User",
                Surname = "Test Surname",
            };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, "User")).ReturnsAsync(true); // Mock role check

            var licenceService = new LicenceService(Mock.Of<SystemContext>(), userManagerMock.Object);

            // Act
            var result = await licenceService.HasRole(user.Id, "User");

            // Assert
            Assert.True(result);

            userManagerMock.Verify(u => u.FindByIdAsync(user.Id), Times.Once); // Ensure FindByIdAsync was called
            userManagerMock.Verify(u => u.IsInRoleAsync(user, "User"), Times.Once); // Ensure role check was performed
        }

        [Fact]
        public async Task HasRole_InvalidUserAndRole_ReturnsFalse()
        {
            // Arrange
            var user = new LicencedUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Name = "Test User",
                Surname = "Test Surname",
            };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, "User")).ReturnsAsync(false); // Mock role check

            var licenceService = new LicenceService(Mock.Of<SystemContext>(), userManagerMock.Object);

            // Act
            var result = await licenceService.HasRole(user.Id, "User");

            // Assert
            Assert.False(result);

            userManagerMock.Verify(u => u.FindByIdAsync(user.Id), Times.Once); // Ensure FindByIdAsync was called
            userManagerMock.Verify(u => u.IsInRoleAsync(user, "User"), Times.Once); // Ensure role check was performed
        }

        [Fact]
        public async Task RemoveLicence_ValidRequest_DeactivatesLicence()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var licence = new Licence
            {
                Id = Guid.NewGuid(),
                Name = "Test Licence 1",
                Price = 10,
                Type = LicenceType.User,
                Duration = 30
            };

            var ledgerEntries = new List<LicenceLedgerEntry>
            {
                new LicenceLedgerEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    IsActive = true,
                    LicenceId = licence.Id
                }
            };

            var context = CreateMockContext(new List<Licence> { licence }, ledgerEntries);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.RemoveLicence(ledgerEntries[0].Id, userId);

            // Assert
            var updatedEntry = context.LicenceLedgerEntries.FirstOrDefault(l => l.Id.Equals(ledgerEntries[0].Id));
            Assert.NotNull(updatedEntry);
            Assert.False(updatedEntry.IsActive);
        }

        [Fact]
        public async Task TransferLicence_ValidRequest_TransfersAndDeactivatesOldEntry()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var newUserId = Guid.NewGuid().ToString();
            var ledgerEntryId = Guid.NewGuid();

            var licence = new Licence
            {
                Id = Guid.NewGuid(),
                Name = "Test Licence 1",
                Price = 10,
                Type = LicenceType.User,
                Duration = 30
            };

            var ledgerEntries = new List<LicenceLedgerEntry>
            {
                new LicenceLedgerEntry
                {
                    Id = ledgerEntryId,
                    UserId = userId,
                    IsActive = true,
                    PurchaseDate = DateTime.UtcNow,
                    PaymentStatus = LicencePaymentStatus.Received,
                    LicenceId = licence.Id
                }
            };

            var context = CreateMockContext(new List<Licence> { licence }, ledgerEntries);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.TransferLicence(userId, newUserId, ledgerEntryId);

            // Assert
            var oldEntry = context.LicenceLedgerEntries.FirstOrDefault(l => l.Id == ledgerEntryId);
            var newEntry = context.LicenceLedgerEntries.FirstOrDefault(l => l.UserId == newUserId);

            Assert.NotNull(oldEntry);
            Assert.False(oldEntry.IsActive);
            Assert.Equal(LicencePaymentStatus.Transferred, oldEntry.PaymentStatus);

            Assert.NotNull(newEntry);
            Assert.True(newEntry.IsActive);
            Assert.Equal(LicencePaymentStatus.Received, newEntry.PaymentStatus);
            Assert.Equal(licence.Id, newEntry.LicenceId);
        }

        [Fact]
        public async Task TransferLicence_NonExistsLedgerEntry_Returns()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var newUserId = Guid.NewGuid().ToString();
            var ledgerEntryId = Guid.NewGuid();

            var ledgerEntries = new List<LicenceLedgerEntry>();

            var context = CreateMockContext(new List<Licence>(), ledgerEntries);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.TransferLicence(userId, newUserId, ledgerEntryId);

            // Assert
            var oldEntry = context.LicenceLedgerEntries.FirstOrDefault(l => l.Id == ledgerEntryId);
            Assert.Null(oldEntry);
        }

        // [Fact]
        // public async Task UpdateLicenceLedgerEntry_PaidUserLicence_ActivatesAndAddsPayment()
        // {
        //     // Arrange
        //     var userId = Guid.NewGuid().ToString();
        //     var licenceId = Guid.NewGuid();
        //
        //     var user = new LicencedUser
        //     {
        //         Id = userId,
        //         UserName = "testuser",
        //         Name = "Test User"
        //     };
        //
        //     var licence = new Licence
        //     {
        //         Id = licenceId,
        //         Name = "Test User Licence",
        //         Type = LicenceType.User,
        //         Price = 10,
        //         Duration = 30
        //     };
        //
        //     var ledgerEntry = new LicenceLedgerEntry
        //     {
        //         Id = Guid.NewGuid(),
        //         UserId = userId,
        //         LicenceId = licenceId,
        //         IsActive = false,
        //         PaymentStatus = LicencePaymentStatus.Unpaid
        //     };
        //
        //     var stripeSession = new Session
        //     {
        //         Created = DateTime.UtcNow, // Note: Stripe uses Unix timestamp
        //         AmountTotal = 1000,
        //         PaymentIntentId = "pi_test123"
        //     };
        //
        //     var options = new DbContextOptionsBuilder<SystemContext>()
        //         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //         .Options;
        //
        //     var context = new SystemContext(options);
        //     context.Licences.Add(licence);
        //     context.LicenceLedgerEntries.Add(ledgerEntry);
        //     context.Users.Add(user);
        //     await context.SaveChangesAsync();
        //
        //     var userManagerMock = new Mock<UserManager<User>>(
        //         Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        //
        //     userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
        //     userManagerMock.Setup(u => u.IsInRoleAsync(user, Roles.LicencedUser)).ReturnsAsync(false);
        //     userManagerMock.Setup(u => u.AddToRoleAsync(user, Roles.LicencedUser)).Returns(Task.FromResult(IdentityResult.Success));
        //
        //     var licenceService = new LicenceService(context, userManagerMock.Object);
        //
        //     // Act
        //     await licenceService.UpdateLicenceLedgerEntry(ledgerEntry.Id, LicencePaymentStatus.Paid, stripeSession);
        //
        //     // Assert
        //     var updatedLedgerEntry = await context.LicenceLedgerEntries
        //         .Include(x => x.Payment)
        //         .FirstOrDefaultAsync(l => l.Id == ledgerEntry.Id);
        //
        //     Assert.NotNull(updatedLedgerEntry);
        //     Assert.True(updatedLedgerEntry.IsActive);
        //     Assert.Equal(LicencePaymentStatus.Paid, updatedLedgerEntry.PaymentStatus);
        //
        //     Assert.NotNull(updatedLedgerEntry.Payment);
        //     Assert.Equal(10m, updatedLedgerEntry.Payment.Amount);
        //     Assert.Equal("pi_test123", updatedLedgerEntry.Payment.PaymentNumberStripe);
        //
        //     userManagerMock.Verify(u => u.AddToRoleAsync(user, Roles.LicencedUser), Times.Once);
        // }

        // [Fact]
        // public async Task UpdateLicenceLedgerEntry_PaidOrganizationLicence_AddsOrganizationOwnerRole()
        // {
        //     // Arrange
        //     var userId = Guid.NewGuid().ToString();
        //     var licenceId = Guid.NewGuid();
        //
        //     var user = new LicencedUser
        //     {
        //         Id = userId,
        //         UserName = "testuser",
        //         Name = "Test User",
        //         Surname = "Test Surname"
        //     };
        //
        //     var licence = new Licence
        //     {
        //         Id = licenceId,
        //         Name = "Test Organization Licence",
        //         Type = LicenceType.Organization,
        //         Price = 100,
        //         Duration = 365
        //     };
        //
        //     var ledgerEntry = new LicenceLedgerEntry
        //     {
        //         Id = Guid.NewGuid(),
        //         UserId = userId,
        //         LicenceId = licenceId,
        //         IsActive = false,
        //         PaymentStatus = LicencePaymentStatus.Unpaid
        //     };
        //
        //     var stripeSession = new Session
        //     {
        //         Created = DateTime.UtcNow, // Note: Stripe uses Unix timestamp
        //         AmountTotal = 1000,
        //         PaymentIntentId = "pi_test123"
        //     };
        //
        //     var context = CreateMockContext(new List<Licence> { licence }, new List<LicenceLedgerEntry> { ledgerEntry });
        //
        //     var userManagerMock = new Mock<UserManager<User>>(
        //         Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        //
        //     userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
        //     userManagerMock.Setup(u => u.IsInRoleAsync(user, Roles.OrganizationOwner)).ReturnsAsync(false);
        //     userManagerMock.Setup(u => u.AddToRoleAsync(user, Roles.OrganizationOwner))
        //         .Returns(Task.FromResult(IdentityResult.Success));
        //
        //     // Mock the service to use a custom method for updating user type
        //     var licenceService = new Mock<LicenceService>(context, userManagerMock.Object) {
        //         CallBase = true
        //     };
        //
        //     // Override the SQL execution method to simply update the user type in memory
        //     licenceService.Setup(s => s.UpdateUserType(It.IsAny<User>(), It.IsAny<string>()))
        //         .Callback<User, string>((u, type) => {
        //             u.UserType = type;
        //         });
        //
        //     // Act
        //     await licenceService.Object.UpdateLicenceLedgerEntry(ledgerEntry.Id, LicencePaymentStatus.Paid, stripeSession);
        //
        //     // Assert
        //     var updatedLedgerEntry = await context.LicenceLedgerEntries
        //         .Include(x => x.Payment)
        //         .FirstOrDefaultAsync(l => l.Id == ledgerEntry.Id);
        //
        //     var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //
        //     Assert.NotNull(updatedLedgerEntry);
        //     Assert.True(updatedLedgerEntry.IsActive);
        //     Assert.Equal(LicencePaymentStatus.Paid, updatedLedgerEntry.PaymentStatus);
        //
        //     Assert.NotNull(updatedLedgerEntry.Payment);
        //     Assert.Equal(100m, updatedLedgerEntry.Payment.Amount);
        //     Assert.Equal("pi_test123", updatedLedgerEntry.Payment.PaymentNumberStripe);
        //
        //     Assert.Equal("OrganizationOwner", updatedUser.UserType);
        //
        //     userManagerMock.Verify(u => u.AddToRoleAsync(user, Roles.OrganizationOwner), Times.Once);
        // }
        //
        // public virtual void UpdateUserType(User user, string userType)
        // {
        //     user.UserType = userType;
        // }

        // [Fact]
        // public async Task UpdateLicenceLedgerEntry_Unpaid_DeactivatesLedgerEntry()
        // {
        //     // Arrange
        //     var userId = Guid.NewGuid().ToString();
        //     var licenceId = Guid.NewGuid();
        //
        //     var user = new LicencedUser
        //     {
        //         Id = userId,
        //         UserName = "testuser",
        //         Name = "Test User"
        //     };
        //
        //     var licence = new Licence
        //     {
        //         Id = licenceId,
        //         Name = "Test User Licence",
        //         Type = LicenceType.User,
        //         Price = 10,
        //         Duration = 30
        //     };
        //
        //     var ledgerEntry = new LicenceLedgerEntry
        //     {
        //         Id = Guid.NewGuid(),
        //         UserId = userId,
        //         LicenceId = licenceId,
        //         IsActive = true,
        //         PaymentStatus = LicencePaymentStatus.Paid
        //     };
        //
        //     var stripeSession = new Session
        //     {
        //         Created = DateTime.UtcNow, // Note: Stripe uses Unix timestamp
        //         AmountTotal = 1000,
        //         PaymentIntentId = "pi_test123"
        //     };
        //
        //     var options = new DbContextOptionsBuilder<SystemContext>()
        //         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //         .Options;
        //
        //     var context = new SystemContext(options);
        //     context.Licences.Add(licence);
        //     context.LicenceLedgerEntries.Add(ledgerEntry);
        //     context.Users.Add(user);
        //     await context.SaveChangesAsync();
        //
        //     var userManagerMock = new Mock<UserManager<User>>(
        //         Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        //
        //     var licenceService = new LicenceService(context, userManagerMock.Object);
        //
        //     // Act
        //     await licenceService.UpdateLicenceLedgerEntry(ledgerEntry.Id, LicencePaymentStatus.Unpaid, stripeSession);
        //
        //     // Assert
        //     var updatedLedgerEntry = await context.LicenceLedgerEntries
        //         .FirstOrDefaultAsync(l => l.Id == ledgerEntry.Id);
        //
        //     Assert.NotNull(updatedLedgerEntry);
        //     Assert.False(updatedLedgerEntry.IsActive);
        //     Assert.Equal(LicencePaymentStatus.Unpaid, updatedLedgerEntry.PaymentStatus);
        // }

        [Fact]
        public async Task UpdateLicenceLedgerEntry_NonExistentEntry_DoesNothing()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var stripeSession = new Mock<Session>().Object;

            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SystemContext(options);
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.UpdateLicenceLedgerEntry(nonExistentId, LicencePaymentStatus.Paid, stripeSession);

            // Assert
            // No exception should be thrown, and no changes should occur
            Assert.Empty(context.LicenceLedgerEntries);
        }

        [Fact]
        public async Task UpdateLicenceLedgerEntry_PaidOrganizationLicence_Success()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var licenceId = Guid.NewGuid();
            var ledgerEntryId = Guid.NewGuid();

            var user = new LicencedUser
            {
                Id = userId,
                UserName = "testuser",
                Name = "Test User",
                Surname = "Test Surname"
            };

            var licence = new Licence
            {
                Id = licenceId,
                Name = "Test Organization Licence",
                Type = LicenceType.Organization,
                Price = 100,
                Duration = 365
            };

            var ledgerEntry = new LicenceLedgerEntry
            {
                Id = ledgerEntryId,
                UserId = userId,
                LicenceId = licenceId,
                IsActive = false,
                PaymentStatus = LicencePaymentStatus.Unpaid
            };

            var stripeSession = new Session
            {
                Created = DateTime.UtcNow,
                AmountTotal = 10000, // $100.00
                PaymentIntentId = "pi_test123"
            };

            
            var context = CreateMockContext(new List<Licence> { licence }, new List<LicenceLedgerEntry> { ledgerEntry }, new List<User> { user });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, Roles.OrganizationOwner)).ReturnsAsync(true);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.UpdateLicenceLedgerEntry(ledgerEntryId, LicencePaymentStatus.Paid, stripeSession);

            // Assert
            await context.Entry(ledgerEntry).ReloadAsync();
            await context.Entry(user).ReloadAsync();

            Assert.True(ledgerEntry.IsActive);
            Assert.Equal(LicencePaymentStatus.Paid, ledgerEntry.PaymentStatus);

            var payment = await context.Payments.FirstOrDefaultAsync(p => p.LicenceLegerEntryId == ledgerEntryId);
            Assert.NotNull(payment);
            Assert.Equal(100m, payment.Amount);

            userManagerMock.Verify(u => u.AddToRoleAsync(user, Roles.OrganizationOwner), Times.Never);
        }
        
        [Fact]
        public async Task UpdateLicenceLedgerEntry_UnpaidOrganizationLicence_PaymentNull()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var licenceId = Guid.NewGuid();
            var ledgerEntryId = Guid.NewGuid();

            var user = new LicencedUser
            {
                Id = userId,
                UserName = "testuser",
                Name = "Test User",
                Surname = "Test Surname"
            };

            var licence = new Licence
            {
                Id = licenceId,
                Name = "Test Organization Licence",
                Type = LicenceType.Organization,
                Price = 100,
                Duration = 365
            };

            var ledgerEntry = new LicenceLedgerEntry
            {
                Id = ledgerEntryId,
                UserId = userId,
                LicenceId = licenceId,
                IsActive = false,
                PaymentStatus = LicencePaymentStatus.Unpaid
            };

            var stripeSession = new Session
            {
                Created = DateTime.UtcNow,
                AmountTotal = 10000, // $100.00
                PaymentIntentId = "pi_test123"
            };

            
            var context = CreateMockContext(new List<Licence> { licence }, new List<LicenceLedgerEntry> { ledgerEntry }, new List<User> { user });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.IsInRoleAsync(user, Roles.OrganizationOwner)).ReturnsAsync(true);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.UpdateLicenceLedgerEntry(ledgerEntryId, LicencePaymentStatus.Unpaid, stripeSession);

            // Assert
            await context.Entry(ledgerEntry).ReloadAsync();
            await context.Entry(user).ReloadAsync();

            Assert.False(ledgerEntry.IsActive);
            Assert.Equal(LicencePaymentStatus.Unpaid, ledgerEntry.PaymentStatus);

            var payment = await context.Payments.FirstOrDefaultAsync(p => p.LicenceLegerEntryId == ledgerEntryId);
            Assert.Null(payment);

            userManagerMock.Verify(u => u.AddToRoleAsync(user, Roles.OrganizationOwner), Times.Never);
        }
        
        [Fact]
        public async Task HasRole_NonExistUser_False()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var user = new LicencedUser
            {
                Id = userId,
                UserName = "testuser",
                Name = "Test User",
                Surname = "Test Surname"
            };
            

            
            var context = CreateMockContext(new List<Licence>(), new List<LicenceLedgerEntry>(), new List<User> { user });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            var result = await licenceService.HasRole(Guid.NewGuid().ToString(), Roles.LicencedUser);

            // Assert
            Assert.False(result);
        }
        
        [Fact] 
        public async Task ValidateLicenceExpiration_ValidLicence_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var licenceId = Guid.NewGuid();
            var ledgerEntryId = Guid.NewGuid();

            var user = new LicencedUser
            {
                Id = userId,
                UserName = "testuser",
                Name = "Test User",
                Surname = "Test Surname"
            };

            var licence = new Licence
            {
                Id = licenceId,
                Name = "Test Organization Licence",
                Type = LicenceType.Organization,
                Price = 100,
                Duration = 365
            };

            var ledgerEntry = new LicenceLedgerEntry
            {
                Id = ledgerEntryId,
                UserId = userId,
                LicenceId = licenceId,
                IsActive = true,
                PaymentStatus = LicencePaymentStatus.Paid,
                PurchaseDate = DateTime.UtcNow.AddDays(-366)
            };

            var context = CreateMockContext(new List<Licence> { licence }, new List<LicenceLedgerEntry> { ledgerEntry }, new List<User> { user });
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var licenceService = new LicenceService(context, userManagerMock.Object);

            // Act
            await licenceService.ValidateLicenceExpiration(userId);

            // Assert
            await context.Entry(ledgerEntry).ReloadAsync();
            Assert.False(ledgerEntry.IsActive);
        }

    }
}