using AuthServer.Core.CommonDTOs;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Service.HelperMethods;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketDetailRepository _basketDetailRepository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IGenericRepository<Basket> _genericRepository;

        public BasketService(IGenericRepository<Basket> genericRepository, IUnitOfWork unitOfWork, IBasketRepository basketRepository, IBasketDetailRepository basketDetailRepository)
        {
            _basketRepository = basketRepository;
            _basketDetailRepository = basketDetailRepository;
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<IEnumerable<BasketDto>>> GetBasketsWithBasketDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Response<IEnumerable<BasketDto>>.Fail("userId is wrong", 400, true);
            }

            var baskets = _basketRepository.GetBasketsWithBasketDetails(userId);

            var dtoBaskets = await baskets.Select(i => new BasketDto
            {
                Id = i.Id,
                IPAdress = i.IPAdress,
                Address = i.Address,
                Date = i.Date,
                UserId = i.UserId,
                TotalPrice = i.BasketDetails.Sum(i => i.Price * i.Quantity),
                BasketDetails = i.BasketDetails.Select(x => new BasketDetailDto
                {
                    BasketId = x.BasketId,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    ProductCode = x.Product.Code,
                    ProductName = x.Product.Name
                }).ToList()
            }).ToListAsync();

            if (dtoBaskets == null)
            {
                return Response<IEnumerable<BasketDto>>.Fail("User does not have baskets", 404, true);
            }

            return Response<IEnumerable<BasketDto>>.Success(dtoBaskets, 200);
        }

        // burayı transaction içine almamızın sebebi:
        // Basketleri db ye ekleyip savechanges yapıcaz gelen id'yi kullanıp basketdetails'ları ekleyecez
        // bu yüzden 2 tane ayrı savechangesvar ilkinde hata var ise veya ikincisinde rollback olunması gerekir
        // eğer bir hata olursa zaten commit metodundan exception dönecek ve global exception yakalayıcı
        // yakalayıp 500 dönecek. Bu tarz durumlar için yani aynı yerde birbirine bağımlı 2 saveChanges'ler 
        // için önemli bir durum.veri tutarlılığı açısından.
        public async Task<Response<NoDataDto>> AddBasketWithBasketDetails(BasketDto basketDto)
        {
            #region IsValidBasketDto

            if (basketDto.IPAdress == null || !SecurityMethods.ValidateIPv4(basketDto.IPAdress))
            {
                return Response<NoDataDto>.Fail("IP Address is wrong", 400, false);
            }
            else if (basketDto.Address == null || basketDto.Address.Length > 60)
            {
                return Response<NoDataDto>.Fail("Address is wrong", 400, false);
            }
            else if(basketDto.UserId == null)
            {
                return Response<NoDataDto>.Fail("UserId is wrong", 400, false);
            }
            else if (basketDto.BasketDetails == null || basketDto.BasketDetails.Count < 1)
            {
                return Response<NoDataDto>.Fail("BasketDetails is required", 400, false);
            }

            #endregion

            using (var transaction = await _unitOfWork.GetDbContext().Database.BeginTransactionAsync()) 
            {
                Basket basket = new Basket()
                {
                    IPAdress = basketDto.IPAdress,
                    Address = basketDto.Address,
                    Date = DateTime.Now,
                    UserId = basketDto.UserId,
                };

                await _genericRepository.AddAsync(basket);
                await _unitOfWork.CommitAsync();

                var basketDetails = basketDto.BasketDetails.Select(i => new BasketDetail
                {
                    BasketId = basket.Id,
                    ProductId = i.ProductId,
                    Price = i.Price,
                    Quantity = i.Quantity,
                }).ToList();

                await _basketDetailRepository.AddRangeAsync(basketDetails);
                await _unitOfWork.CommitAsync();

                await transaction.CommitAsync();

                return Response<NoDataDto>.Success(201);
            }
        }
    }
}
