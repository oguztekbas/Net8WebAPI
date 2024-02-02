using AuthServer.Core.CommonDTOs;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<ClientTokenOption> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<ClientTokenOption>> optionsClient, ITokenService tokenService, UserManager<UserApp>
            userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CreateAccessTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true); //400 Client Hatası
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true); //400 Client Hatası
            }

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenService.Where(i => i.UserId == user.Id).FirstOrDefaultAsync();

            // Token almak için request geldi. Token'ı yarattık token içinde refreshToken'ı da dönüyorsun
            // Ancak db'deki durumunu kontrol etmelisin. Db'de kaydı yoksa ekleme işlemi yap.
            // Db'de kaydı varsa değeri yeni yarattığın tokendaki güncel refreshToken bilgisiyle güncelle
            
            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, 200);
        }

        // İlk önce böyle bir refreshToken DB'de var mı kontrol ediyoruz.
        // Yoksa hata yolluyoruz. Varsa böyle bir kullanıcı var mı çünkü refreshToken içinde userId bilgisi de var.
        // Bu kontrolden sonra yeni bir token yaratıp yolluyoruz.
        public async Task<Response<TokenDto>> CreateAccessTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(i => i.Code == refreshToken).FirstOrDefaultAsync();

            if(existRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh token not found", 404, true);
            }

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if (user == null)
            {
                return Response<TokenDto>.Fail("User Id not found", 404, true);
            }

            var tokenDto = _tokenService.CreateToken(user);

            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, 200);
        }

        // Üyelik sistemi olmayan örneğin hava durumu app'e hizmet eden API için token yaratma.
        // 
        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.FirstOrDefault(i => i.Id == clientLoginDto.ClientId && i.Secret == clientLoginDto.ClientSecret);

            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404, true);
            }

            var token = _tokenService.CreateTokenByClient(client);

            return Response<ClientTokenDto>.Success(token, 200);
        }

        // Kullanıcı client tarafından logout request'i ile geldiğinde refreshToken revoke edilmeli
        // yani DB'den silinmeli ki kötü niyetli kullanıcı tarafından ele geçirilip suistimal edilmesin.
        // veya her hangi bir durumda da token bilgisinin ele geçildiği bilinirse kullanılabilir.
        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(i => i.Code == refreshToken).FirstOrDefaultAsync();

            if(existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", 404, true);
            }

            _userRefreshTokenService.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(200);
        }
    }
}
