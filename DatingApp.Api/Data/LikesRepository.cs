using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Extensions;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int targetUserId)
        {
            return await this.context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams)
        {
            var users = this.context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = this.context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikesAsync(int userId)
        {
            return await this.context.Users
                                     .Include(u => u.LikedUsers)
                                     .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}

