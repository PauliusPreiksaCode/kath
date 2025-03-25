using organization_back_end.RequestDtos.Email;

namespace organization_back_end.Interfaces;

public interface IEmailService
{
    void SendRegisterEmail(string email, string name);
    void SendInvitationEmail(string organizationName, string inviterName, string inviteeName, string email);
}