using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : CustomBaseController
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [Route("user/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBasketsWithBasketDetails(string userId)
        {
            var result = await _basketService.GetBasketsWithBasketDetails(userId);

            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBasketWithBasketDetails(BasketDto basketDto)
        {
            var result = await _basketService.AddBasketWithBasketDetails(basketDto);

            return ActionResultInstance(result);
        }
    }
}
