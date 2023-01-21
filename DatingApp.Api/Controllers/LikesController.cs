using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Extensions;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork uow;

        public LikesController(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLikeAsync(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await this.uow.UserRepository.GetUserByUserNameAsync(username);
            var sourceUser = await this.uow.LikesRepository.GetUserWithLikesAsync(sourceUserId);

            if (likedUser == null)
            {
                return NotFound();
            }

            if (sourceUser.UserName == username)
            {
                return BadRequest("You cannot like yourself");
            }

            var userLike = await this.uow.LikesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                return BadRequest("You already like this user");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await this.uow.CompleteAsync() )
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikesAsync([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await this.uow.LikesRepository.GetUserLikesAsync(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }
	}
}

