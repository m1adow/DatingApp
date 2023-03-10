using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Helpers;

namespace DatingApp.Api.Interfaces
{
	public interface IUserRepository
	{
		void Update(AppUser user);
		Task<IEnumerable<AppUser>> GetUsersAsync();
		Task<AppUser> GetUserByIdAsync(int id);
		Task<AppUser> GetUserByUserNameAsync(string userName);
		Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
		Task<MemberDto> GetMemberAsync(string userName);
		Task<string> GetUserGenderAsync(string userName);
		Task<Photo> GetPhotoByIdAsync(string userName, int id);
	}
}

