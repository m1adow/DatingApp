using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Helpers;

namespace DatingApp.Api.Interfaces
{
	public interface ILikesRepository
	{
		Task<UserLike> GetUserLikeAsync(int sourceUserId, int targetUserId);
		Task<AppUser> GetUserWithLikesAsync(int userId);
		Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams);
	}
}

