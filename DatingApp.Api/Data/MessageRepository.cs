using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public void AddGroup(Group group)
        {
            this.context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            this.context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            this.context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnectionAsync(string connectionId)
        {
            return await this.context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnectionAsync(string connectionId)
        {
            return await this.context.Groups.Include(x => x.Connections)
                                            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                                            .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await this.context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroupAsync(string groupName)
        {
            return await this.context.Groups.Include(x => x.Connections)
                                            .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = this.context.Messages
                                    .OrderByDescending(m => m.MessageSent)
                                    .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUserName == messageParams.UserName && !u.RecipientDeleted),
                "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName && !u.SenderDeleted),
                _ => query.Where(u => u.RecipientUserName == messageParams.UserName && !u.RecipientDeleted && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(this.mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUserName, string recipientUserName)
        {
            var messages = await this.context.Messages
                                             .Include(u => u.Sender).ThenInclude(p => p.Photos)
                                             .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                                             .Where(m => m.RecipientUserName == currentUserName
                                                      && !m.RecipientDeleted
                                                      && m.SenderUserName == recipientUserName
                                                      || m.RecipientUserName == recipientUserName
                                                      && !m.SenderDeleted
                                                      && m.SenderUserName == currentUserName)
                                             .OrderBy(m => m.MessageSent)
                                             .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return this.mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            this.context.Connections.Remove(connection);
        }
    }
}

