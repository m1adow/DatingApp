using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;

namespace DatingApp.Api.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            this.context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            this.context.Messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await this.context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = this.context.Messages
                                    .OrderByDescending(m => m.MessageSent)
                                    .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUserName == messageParams.UserName),
                "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName),
                _ => query.Where(u => u.RecipientUserName == messageParams.UserName && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(this.mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public Task<IEnumerable<MessageDto>> GetMessageThreadAsync(int currentUserId, int recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}

