﻿using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;

namespace DatingApp.Api.Interfaces
{
	public interface IUserRepository
	{
		void Update(AppUser user);
		Task<bool> SaveAllAsync();
		Task<IEnumerable<AppUser>> GetUsersAsync();
		Task<AppUser> GetUserByIdAsync(int id);
		Task<AppUser> GetUserByUserNameAsync(string userName);
		Task<IEnumerable<MemberDto>> GetMembersAsync();
		Task<MemberDto> GetMemberAsync(string userName);
	}
}

