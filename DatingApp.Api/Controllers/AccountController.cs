using System.Security.Cryptography;
using System.Text;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
	public class AccountController : BaseApiController
	{
        private readonly DataContext context;

        public AccountController(DataContext context)
		{
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> RegisterAsync(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
                return BadRequest("Username is taken");

            using (var hmac = new HMACSHA512())
            {
                var user = new AppUser
                {
                    UserName = registerDto.UserName.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key
                };

                await this.context.AddAsync(user);
                await this.context.SaveChangesAsync();

                return user;
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> LoginAsync(LoginDto loginDto)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null)
                return Unauthorized("Invalid username");

            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i])
                        return Unauthorized("Invalid password");
                }
            }

            return user;
        }

        private async Task<bool> UserExists(string userName) => await this.context.Users.AnyAsync(x => x.UserName == userName.ToLower());
	}
}

