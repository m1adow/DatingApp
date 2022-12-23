using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
        private readonly DataContext context;

        public UsersController(DataContext context)
		{
            this.context = context;
        }

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync() => await this.context.Users.ToListAsync();
         
		[HttpGet("{id}")]
		public async Task<ActionResult<AppUser>> GetUserAsync(int id) => await this.context.Users.FindAsync(id);
	}
}