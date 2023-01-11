using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
	public class AccountController : BaseApiController
	{
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
		{
            this.context = context;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
                return BadRequest("Username is taken");

            var user = this.mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();

            await this.context.AddAsync(user);
            await this.context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await this.context.Users.Include(u => u.Photos).FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null)
                return Unauthorized("Invalid username");

            return new UserDto
            {
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string userName) => await this.context.Users.AnyAsync(x => x.UserName == userName.ToLower());
	}
}

