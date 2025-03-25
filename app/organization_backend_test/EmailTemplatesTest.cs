using organization_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organization_backend.Test.Helpers
{
    public class EmailTemplatesTest
    {
        [Fact]
        public void GetRegisterEmailTemplate_ShouldReturnFormattedEmail()
        {
            // Arrange
            var name = "John Doe";

            // Act
            var result = EmailTemplates.GetRegisterEmailTemplate(name);

            // Assert
            Assert.Contains(name, result);
            Assert.Contains("Welcome to KATH!", result);
            Assert.Contains("Thank you for registering", result);
        }

        [Fact]
        public void GetOrganizationTeamInvitationTemplate_ShouldReturnFormattedEmail()
        {
            // Arrange
            var organizationName = "Tech Corp";
            var inviterName = "Alice";
            var inviteeName = "Bob";

            // Act
            var result = EmailTemplates.GetOrganizationTeamInvitationTemplate(organizationName, inviterName, inviteeName);

            // Assert
            Assert.Contains(organizationName, result);
            Assert.Contains(inviteeName, result);
            Assert.Contains(inviterName, result);
            Assert.Contains("You have been invited to the organization", result);
        }

    }
}
