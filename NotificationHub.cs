using Microsoft.AspNetCore.SignalR;

namespace TaskManagerApi
{
    public class NotificationHub : Hub
    {
        public async Task Send(string Message)
        {
            await Clients.All.SendAsync(Message);
        }
    }
}
