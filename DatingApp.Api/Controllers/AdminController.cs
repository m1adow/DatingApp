using AutoMapper;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
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
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            this.userManager = userManager;
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
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
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetPhotoForModerationAsync()
        {
            var membersWithNotApprovedPhotos = await this.uow.UserRepository.GetMembersApprovalUserPhotosAsync();

            if (!membersWithNotApprovedPhotos.Any())
            {
                return NoContent();
            }

            return Ok(membersWithNotApprovedPhotos.Where(m => m.Photos.Count > 0).Select(m => new
            {
                username = m.UserName,
                knownAs = m.KnownAs,
                photos = m.Photos
            }));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("approve-photo")]
        public async Task<ActionResult<PhotoDto>> ApprovePhotoAsync([FromQuery] string username, int photoId, bool isApproved)
        {
            var photo = await this.uow.UserRepository.GetPhotoByIdAsync(username, photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsApproved == isApproved && isApproved)
            {
                return BadRequest();
            }

            photo.IsApproved = isApproved;

            var user = await this.uow.UserRepository.GetUserByUserNameAsync(username);

            if (!user.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            if (!photo.IsApproved)
            {
                await this.photoService.DeletePhotoAsync(photo.PublicId);
                user.Photos.Remove(photo);
            }

            if (await this.uow.CompleteAsync())
            {
                return Ok(new
                {
                    username = username,
                    photoId = photo.Id,
                    isApproved = photo.IsApproved
                });
            }

            return BadRequest("Problem with approving");
        }
    }
}