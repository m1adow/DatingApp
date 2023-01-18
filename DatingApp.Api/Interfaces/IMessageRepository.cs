using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Helpers;

namespace DatingApp.Api.Interfaces
{
	public interface IMessageRepository
	{
		void AddMessage(Message message);
		void DeleteMessage(Message message);
		Task<Message> GetMessageAsync(int id);
		Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
		Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUserName, string recipientUserName);
		Task<bool> SaveAllAsync();
		void AddGroup(Group group);
		void RemoveConnection(Connection connection);
		Task<Connection> GetConnectionAsync(string connectionId);
		Task<Group> GetMessageGroupAsync(string groupName);
	}
}

