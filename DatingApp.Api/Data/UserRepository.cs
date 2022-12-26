using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;

        public UserRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(int id) => await this.context.Users.FindAsync(id);

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
            => await this.context.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == userName);

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
            => await this.context.Users.Include(p => p.Photos).ToListAsync();

        public async Task<bool> SaveAllAsync() => await this.context.SaveChangesAsync() > 0;

        public void Update(AppUser user) => this.context.Entry(user).State = EntityState.Modified;
    }
}

