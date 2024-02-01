using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using AuthServer.Service.AutoMapper;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager; 
        }

        // Oauth gibi protokollere uygun hazır Identity Framework'ünü kullanıyoruz projede.
        // Burda Password geçme işleminde =>  userManager.CreateAsync(user,createUserDto.Password)
        // Identity framework'üyle otomatik hashlendiğini unutma.
        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName
            };

            var result = await _userManager.CreateAsync(user,createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }

            var userAppDto = ObjectMapper.Mapper.Map<UserAppDto>(user);

            return Response<UserAppDto>.Success(userAppDto, 200);
        }

        public async Task<Response<UserAppDto>> GetUserByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if(user == null)
            {
                return Response<UserAppDto>.Fail("UserName not found", 404, true);
            }

            var userAppDto = ObjectMapper.Mapper.Map<UserAppDto>(user);

            return Response<UserAppDto>.Success(userAppDto, 200);
        }
    }
}
