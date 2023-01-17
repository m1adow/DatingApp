using DatingApp.Api.Extensions;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Api.SignalR
{
	public class MessageHub : Hub
	{
        private readonly IMessageRepository messageRepository;

        public MessageHub(IMessageRepository messageRepository)
		{
            this.messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await this.messageRepository.GetMessageThreadAsync(Context.User.GetUserName(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}

