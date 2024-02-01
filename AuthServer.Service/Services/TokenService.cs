using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    // TokenService'i API değil AuthenticationService kullanacak. Bu sayede api kullanacak.
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value; //Token değerlerini app.settings'den bu şekilde aldık.
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _tokenOption.Audience),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            TokenDto tokenDto = new TokenDto
            {
               AccessToken = token,
               RefreshToken = CreateRefreshToken(),
               AccessTokenExpiration = accessTokenExpiration,
               RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;
        }

        // Üyelik sistemi barındırmayan client'le ilgili üretilen token. Bu yüzden refreshToken'ı yok.
        // ve claims'i farklı ClientId var.
        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            ClientTokenDto clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration
            };

            return clientTokenDto;
        }




        //RefreshToken sabit bir string değer yani bir Token olmadığı için rastgele bir number
        //ürettik ve private buraya ait bi metod.
        private string CreateRefreshToken()
        {
            //return Guid.NewGuid().ToString(); Alternatif.

            var numberByte = new Byte[32];

            using (var rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(numberByte);

                return Convert.ToBase64String(numberByte);
            }
        }

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences)
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()) //Her token için numara
            };

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud,x)));

            return userList;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();

            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()); //Her token için numara
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString()); // Sub yani Subject
            // bu Token'ı client için oluşturyoruz o yüzden o client'in Id'si

            return claims;
        }
    }
}
