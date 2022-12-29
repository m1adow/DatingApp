using System.Security.Claims;

namespace DatingApp.Api.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static string GetUserName(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

