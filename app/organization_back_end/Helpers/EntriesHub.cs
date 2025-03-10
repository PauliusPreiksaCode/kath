using Microsoft.AspNetCore.SignalR;

namespace organization_back_end.Helpers;

public class EntriesHub : Hub
{
    public async Task SubscribeToOrganization(string organizationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, organizationId);
    }
    
    public async Task UnsubscribeFromOrganization(string organizationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, organizationId);
    }
}