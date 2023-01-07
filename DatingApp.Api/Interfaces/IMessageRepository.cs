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
		Task<IEnumerable<MessageDto>> GetMessageThreadAsync(int currentUserId, int recipientId);
		Task<bool> SaveAllAsync();
	}
}

