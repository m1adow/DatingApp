using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.DTOs
{
	public class RegisterDto
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		[StringLength(8, MinimumLength = 4)]
		public string Password { get; set; }
	}
}

