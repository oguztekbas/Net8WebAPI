using AuthServer.Core.DTOs;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateAccessTokenAsync(LoginDto loginDto);

        //Refresh token kullanarak AccessToken oluşturma
        Task<Response<TokenDto>> CreateAccessTokenByRefreshToken(string refreshToken);

        //Client tarafında kullanıcı logout olduğunda client token ve refreshToken bilgilerini
        //localStorage'den siler ama backEnd tarafta db'den de silinmesi gerekir. Bu işlemi de yapması gerekir.
        //Ayrıca refreshToken kötü niyetli kullanıcıya geçtiği zamanda bu metod kullanılabilir.
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);
        
        //MiniApp3 dediğimiz örneğin hava durumu API'si. Yani üyelik sistemi yok ama
        //appSettings'e koyacağımız clientId ve şifresine göre token vereceğiz ona göre çalışacak.
        Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto);
    }
}
