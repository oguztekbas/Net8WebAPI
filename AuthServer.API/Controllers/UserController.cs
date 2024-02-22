using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // api/user/register
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            var result = await _userService.CreateUserAsync(createUserDto);

            return ActionResultInstance(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userNameClaim = HttpContext.User.Claims.Where(i => i.Type == "name").FirstOrDefault();
            var result = await _userService.GetUserByUserNameAsync(userNameClaim.Value);

            return ActionResultInstance(result);
        }

        //HttpContext.User.Identity.Name diye direk alabilmemizin sebebi token oluştururken
        //NameIdentifier olarak oluşturmamız böylece otomatik gelen tokendan anlayabiliyor.
        //Ancak string olarak elimizde claim'i username diye oluştursaydık
        //bu endpoint'te gelen token'dan userName'i bulabilmek için
        //HttpContext.User.Claims.Where(i => i.Type == "username") gibi bir kod yazmamız gerekirdi.
    }
}
