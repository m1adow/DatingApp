﻿using System.Security.Claims;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
	{
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
		{
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersAsync() => Ok(await this.userRepository.GetMembersAsync());

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserAsync(string userName) => await this.userRepository.GetMemberAsync(userName);

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync(MemberUpdateDto memberUpdateDto)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this.userRepository.GetUserByUserNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            this.mapper.Map(memberUpdateDto, user);

            if (await this.userRepository.SaveAllAsync())
            {
                return NoContent(); 
            }

            return BadRequest("Failed to update user");
        }
	}
}