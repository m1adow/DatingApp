using System.Security.Claims;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Extensions;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
	{
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
		{
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersAsync([FromQuery]UserParams userParams)
        {
            var gender = await this.uow.UserRepository.GetUserGenderAsync(User.GetUserName());

            if (!(userParams.Gender is string { Length: > 0 }))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await this.uow.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserAsync(string username) => await this.uow.UserRepository.GetMemberAsync(username);

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync(MemberUpdateDto memberUpdateDto)
        {
            var user = await this.uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            if (user == null)
            {
                return NotFound();
            }

            this.mapper.Map(memberUpdateDto, user);

            if (await this.uow.CompleteAsync())
            {
                return NoContent(); 
            }

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhotoAsync(IFormFile file)
        {
            var user = await this.uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            if (user == null)
            {
                return NotFound();
            }

            var result = await this.photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = false,
                IsApproved = false
            };

            user.Photos.Add(photo);

            if (await this.uow.CompleteAsync())
            {
                return CreatedAtAction(nameof(GetUserAsync),
                    new { username = user.UserName },
                    this.mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhotoAsync(int photoId)
        {
            var user = await this.uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            if (user == null)
            {
                return NotFound();
            }

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }

            if (photo.IsApproved) 
            {
                return BadRequest("You cannot set not approved photo as main");
            }

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }

            photo.IsMain = true;

            if (await this.uow.CompleteAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhotoAsync(int photoId)
        {
            var user = await this.uow.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }

            user.Photos.Remove(photo);

            if (await this.uow.CompleteAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting photo");
        }
    }
}