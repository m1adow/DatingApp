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
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
		{
            this.uow = uow;
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

            var sender = await this.uow.UserRepository.GetUserByUserNameAsync(userName);
            var recipient = await this.uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

            if (recipient == null)
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

            this.uow.MessageRepository.AddMessage(message);

            if (await this.uow.CompleteAsync())
            {
                return Ok(this.mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            var messages = await this.uow.MessageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessageAsync(int id)
        {
            var username = User.GetUserName();

            var message = await this.uow.MessageRepository.GetMessageAsync(id);

            if (message.SenderUserName != username && message.RecipientUserName != username)
            {
                return Unauthorized();
            }

            if (message.SenderUserName == username)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientUserName == username)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                this.uow.MessageRepository.DeleteMessage(message);
            }

            if (await this.uow.CompleteAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting the message");
        }
	}
}

