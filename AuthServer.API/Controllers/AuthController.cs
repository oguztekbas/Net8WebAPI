using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly Serilog.ILogger _logger;

        public AuthController(IAuthenticationService authenticationService, Serilog.ILogger logger)
        {
            _authenticationService = authenticationService;  
            _logger = logger;
        }

        // api/auth/login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            //var a = 12;
            //var b = 0;
            //var c = a / b;

            var result = await _authenticationService.CreateAccessTokenAsync(loginDto);

            return ActionResultInstance(result); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.CreateAccessTokenByRefreshToken(refreshTokenDto.Token);

            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.RevokeRefreshToken(refreshTokenDto.Token);

            return ActionResultInstance(result);
        }

        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var result = _authenticationService.CreateTokenByClient(clientLoginDto);

            return ActionResultInstance(result);
        }
    }
}
