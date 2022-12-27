﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
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

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
            => await this.context.Users.ProjectTo<MemberDto>(this.mapper.ConfigurationProvider)
                                       .ToListAsync();

        public async Task<AppUser> GetUserByIdAsync(int id) => await this.context.Users.FindAsync(id);

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
            => await this.context.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == userName);

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
            => await this.context.Users.Include(p => p.Photos).ToListAsync();

        public async Task<bool> SaveAllAsync() => await this.context.SaveChangesAsync() > 0;

        public void Update(AppUser user) => this.context.Entry(user).State = EntityState.Modified;
    }
}

