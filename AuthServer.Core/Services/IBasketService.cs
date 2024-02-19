using AuthServer.Core.CommonDTOs;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IBasketService : IGenericService<Basket, BasketDto>
    {
        Task<Response<IEnumerable<BasketDto>>> GetBasketsWithBasketDetails(string userId);
        Task<Response<NoDataDto>> AddBasketWithBasketDetails(BasketDto basketDto);
    }
}
