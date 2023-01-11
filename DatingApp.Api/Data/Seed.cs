using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
	public class Seed
	{
		public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
		{
			if (await userManager.Users.AnyAsync())
			{
				return;
			}

			var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

			var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

			foreach (var user in users)
			{
				user.UserName = user.UserName.ToLower();

				await userManager.CreateAsync(user, "Pa$$w0rd");
			}	
		} 
	}
}

