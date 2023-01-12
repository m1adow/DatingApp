using DatingApp.Api.Entities;

namespace DatingApp.Api.Interfaces
{
	public interface ITokenService
	{
		Task<string> CreateTokenAsync(AppUser user);
	}
}

