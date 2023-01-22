using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Helpers;
using DatingApp.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string userName)
            => await this.context.Users.Where(u => u.UserName == userName)
                                       .ProjectTo<MemberDto>(this.mapper.ConfigurationProvider)
                                       .SingleOrDefaultAsync();

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = this.context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDto>(this.mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id) => await this.context.Users.FindAsync(id);

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
            => await this.context.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == userName);

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
            => await this.context.Users.Include(p => p.Photos).ToListAsync();

        public void Update(AppUser user) => this.context.Entry(user).State = EntityState.Modified;

        public async Task<string> GetUserGenderAsync(string userName) => await this.context.Users
                                                                                            .Where(x => x.UserName == userName)
                                                                                            .Select(x => x.Gender)
                                                                                            .FirstOrDefaultAsync();

        public async Task<IEnumerable<MemberDto>> GetMembersApprovalUserPhotosAsync()
        {
            return await this.context.Users.Include(p => p.Photos.Where(p => !p.IsApproved))
                                            .ProjectTo<MemberDto>(this.mapper.ConfigurationProvider)
                                            .ToListAsync();
        }

        public async Task<Photo> GetPhotoByIdAsync(string userName, int id)
        {
            return await this.context.Users.Where(u => u.UserName == userName)
                                            .Include(u => u.Photos)
                                            .Select(u => u.Photos.FirstOrDefault(p => p.Id == id))
                                            .FirstOrDefaultAsync();
        }
    }
}

