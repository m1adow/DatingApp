using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
	{
        private readonly DataContext context;

        public UsersController(DataContext context)
		{
            this.context = context;
        }

        [AllowAnonymous]
        [HttpGet]
		public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync() => await this.context.Users.ToListAsync();

		[HttpGet("{id}")]
		public async Task<ActionResult<AppUser>> GetUserAsync(int id) => await this.context.Users.FindAsync(id);
	}
}