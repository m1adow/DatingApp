using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            var user = this.mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();

            var result = await this.userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResults = await this.userManager.AddToRoleAsync(user, "Member");

            if (!roleResults.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = await this.tokenService.CreateTokenAsync(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await this.userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            var result = await this.userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = await this.tokenService.CreateTokenAsync(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string userName) => await this.userManager.Users.AnyAsync(x => x.UserName == userName.ToLower());
    }
}

