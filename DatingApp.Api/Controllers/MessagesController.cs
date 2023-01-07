using AutoMapper;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Extensions;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
	public class MessagesController : BaseApiController
	{
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
		{
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessageAsync(CreateMessageDto createMessageDto)
        {
            var userName = User.GetUserName();

            if (userName == createMessageDto.RecipientUserName.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var sender = await this.userRepository.GetUserByUserNameAsync(userName);
            var recipient = await this.userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

            if (recipient != null)
            {
                return NotFound();
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            this.messageRepository.AddMessage(message);

            if (await this.messageRepository.SaveAllAsync())
            {
                return Ok(this.mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            var messages = await this.messageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }
	}
}

