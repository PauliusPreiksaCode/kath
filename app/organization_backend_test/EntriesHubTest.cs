using Microsoft.AspNetCore.SignalR;
using Moq;
using organization_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organization_backend.Test.Helpers
{
    public class EntriesHubTest
    {
        [Fact]
        public async Task SubscribeToOrganization_ShouldAddUserToGroup()
        {
            // Arrange
            var hub = new EntriesHub();
            var mockClient = new Mock<IClientProxy>();
            var mockContext = new Mock<HubCallerContext>();
            var mockGroups = new Mock<IGroupManager>();

            mockContext.Setup(c => c.ConnectionId).Returns("test-connection");
            hub.Context = mockContext.Object;
            hub.Groups = mockGroups.Object;

            // Act
            await hub.SubscribeToOrganization("test-org");

            // Assert
            mockGroups.Verify(g => g.AddToGroupAsync("test-connection", "test-org", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeFromOrganization_ShouldRemoveUserFromGroup()
        {
            // Arrange
            var hub = new EntriesHub();
            var mockContext = new Mock<HubCallerContext>();
            var mockGroups = new Mock<IGroupManager>();

            mockContext.Setup(c => c.ConnectionId).Returns("test-connection");
            hub.Context = mockContext.Object;
            hub.Groups = mockGroups.Object;

            // Act
            await hub.UnsubscribeFromOrganization("test-org");

            // Assert
            mockGroups.Verify(g => g.RemoveFromGroupAsync("test-connection", "test-org", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
