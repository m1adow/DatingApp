using AutoMapper;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;

namespace DatingApp.Api.Helpers
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<AppUser, MemberDto>();
			CreateMap<Photo, PhotoDto>();
		}
	}
}

