using DatingApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Api.SignalR
{
    [Authorize]
	public class PresenceHub : Hub
	{
        private readonly PresenceTracker tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            this.tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            await this.tracker.UserConnectedAsync(Context.User.GetUserName(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

            var currentUsers = await this.tracker.GetOnlineUsersAsync();
            await Clients.Others.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await this.tracker.UserDisconnectedAsync(Context.User.GetUserName(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            var currentUsers = await this.tracker.GetOnlineUsersAsync();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

