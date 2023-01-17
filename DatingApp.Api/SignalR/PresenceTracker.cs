﻿namespace DatingApp.Api.SignalR
{
	public class PresenceTracker
	{
		private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

		public Task UserConnectedAsync(string userName, string connectionId)
		{
			lock (OnlineUsers)
			{
				if (OnlineUsers.ContainsKey(userName))
				{
					OnlineUsers[userName].Add(connectionId);
				}
				else
				{
					OnlineUsers.Add(userName, new List<string> { connectionId });
				}
			}

			return Task.CompletedTask;
		}

		public Task UserDisconnectedAsync(string userName, string connectionId)
		{
			lock (OnlineUsers)
			{
				if (!OnlineUsers.ContainsKey(userName))
				{
					return Task.CompletedTask;
				}

				OnlineUsers[userName].Remove(connectionId);

				if (OnlineUsers[userName].Count == 0)
				{
					OnlineUsers.Remove(userName);
				}
			}

			return Task.CompletedTask;
		}

		public Task<string[]> GetOnlineUsersAsync()
		{
			string[] onlineUsers;

			lock (OnlineUsers)
			{
				onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
			}

			return Task.FromResult(onlineUsers);
		}
	}
}
