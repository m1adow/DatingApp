using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
	{
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
		{
            this.userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync() => Ok(await this.userRepository.GetUsersAsync());

        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetUserAsync(string userName) => await this.userRepository.GetUserByUserNameAsync(userName);
	}
}