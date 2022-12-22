using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
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
		public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() => await this.context.Users.ToListAsync();

		[HttpGet("{id}")]
		public async Task<ActionResult<AppUser>> GetUser(int id) => await context.Users.FindAsync(id);
	}
}