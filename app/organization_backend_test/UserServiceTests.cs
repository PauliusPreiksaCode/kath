using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using organization_back_end.Auth.Model;
using organization_back_end.ResponseDto.Users;
using organization_back_end.Services;
using Xunit;

namespace organization_back_end.Tests.Services
{
    public class UserServiceTests
    {
        private SystemContext CreateMockContext(List<User> users)
        {
            var options = new DbContextOptionsBuilder<SystemContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SystemContext(options);
            context.Users.AddRange(users);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetUserById_ExistingUser_ReturnsUser()
        {
            // Arrange
            var userId = "user1";
            var users = new List<User>
            {
                new User
                {
                    Id = userId,
                    UserName = "test@example.com",
                    Name = "Test",
                    Surname = "User"
                },
                new User
                {
                    Id = "user2",
                    UserName = "another@example.com",
                    Name = "Another",
                    Surname = "User"
                }
            };

            var context = CreateMockContext(users);
            var userService = new UserService(context);

            // Act
            var result = await userService.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("test@example.com", result.UserName);
        }

        [Fact]
        public async Task GetUserById_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = "user1",
                    UserName = "test@example.com",
                    Name = "Test",
                    Surname = "User"
                },
            };

            var context = CreateMockContext(users);
            var userService = new UserService(context);

            // Act
            var result = await userService.GetUserById("non-existing-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOtherAllUsers_MultipleUsers_ReturnsOtherUsers()
        {
            // Arrange
            var userId = "user1";
            var users = new List<User>
            {
                new User
                {
                    Id = "user3",
                    UserName = "test@example.com",
                    Name = "Test",
                    Surname = "User"
                },
                new User
                {
                    Id = "user2",
                    UserName = "another@example.com",
                    Name = "Another",
                    Surname = "User"
                }
            };

            var context = CreateMockContext(users);
            var userService = new UserService(context);

            // Act
            var result = await userService.GetOtherAllUsers(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, u => u.Id == userId);
            Assert.Contains(result, u => u.Id == "user2");
            Assert.Contains(result, u => u.Id == "user3");
        }

        [Fact]
        public async Task GetOtherAllUsers_SingleUser_ReturnsEmptyList()
        {
            // Arrange
            var currentUserId = "user3";
            var users = new List<User>
            {
                new User
                {
                    Id = "user3",
                    UserName = "test@example.com",
                    Name = "Test",
                    Surname = "User"
                },
            };

            var context = CreateMockContext(users);
            var userService = new UserService(context);

            // Act
            var result = await userService.GetOtherAllUsers(currentUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}