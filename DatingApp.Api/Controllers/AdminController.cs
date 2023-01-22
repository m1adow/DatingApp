using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
	public class AdminController : BaseApiController
	{
		private readonly UserManager<AppUser> userManager;
		private readonly IUnitOfWork uow;

		public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow)
		{
			this.userManager = userManager;
			this.uow = uow;
		}

		[Authorize(Policy = "RequireAdminRole")]
		[HttpGet("users-with-roles")]
		public async Task<ActionResult> GetUsersWithRolesAsync()
		{
			var users = await this.userManager.Users
											  .OrderBy(u => u.UserName)
											  .Select(u => new
											  {
												  u.Id,
												  UserName = u.UserName,
												  Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
											  })
											  .ToListAsync();

			return Ok(users);
		}

		[Authorize(Policy = "RequireAdminRole")]
		[HttpPost("edit-roles/{username}")]
		public async Task<ActionResult> EditRolesAsync(string username, [FromQuery] string roles)
		{
			if (string.IsNullOrEmpty(roles))
			{
				return BadRequest("You must select at least one role");
			}

			var selectedRoles = roles.Split(",");

			var user = await this.userManager.FindByNameAsync(username);

			if (user == null)
			{
				return NotFound();
			}

			var userRoles = await this.userManager.GetRolesAsync(user);

			var result = await this.userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

			if (!result.Succeeded)
			{
				return BadRequest("Failed to add to roles");
			}

			result = await this.userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove from roles");
            }

			return Ok(await this.userManager.GetRolesAsync(user));
        }

		[Authorize(Policy = "ModeratePhotoRole")]
		[HttpGet("photos-to-moderate")]
		public Task<ActionResult<UserDto>> GetPhotoForModerationAsync()
		{
			//return this.uow.
			return Ok();
		}
	}
}

