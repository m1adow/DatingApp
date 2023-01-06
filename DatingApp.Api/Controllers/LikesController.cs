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
        private readonly IUserRepository userRepository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLikeAsync(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await this.userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await this.likesRepository.GetUserWithLikesAsync(sourceUserId);

            if (likedUser == null)
            {
                return NotFound();
            }

            if (sourceUser.UserName == username)
            {
                return BadRequest("You cannot like yourself");
            }

            var userLike = await this.likesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

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

            if (await this.userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikesAsync([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await this.likesRepository.GetUserLikesAsync(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }
	}
}

