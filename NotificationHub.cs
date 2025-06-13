using Microsoft.AspNetCore.SignalR;

namespace TaskManagerApi
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Global", "Connected to socket!");
        }
    }
}
