using AuthServer.Cache;
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
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthServer.Service.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketDetailRepository _basketDetailRepository;
        protected readonly IUnitOfWork _unitOfWork;

        private const string _basketKeyForRedis = "basketsCache";
        private readonly RedisService _redisService;

        public BasketService(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IBasketDetailRepository basketDetailRepository, RedisService redisService)
        {
            _basketRepository = basketRepository;
            _basketDetailRepository = basketDetailRepository;
            _unitOfWork = unitOfWork;
            _redisService = redisService;
        }

        public async Task<Response<IEnumerable<BasketDto>>> GetBasketsWithBasketDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Response<IEnumerable<BasketDto>>.Fail("userId is wrong", 400, true);
            }

            // Rediste varsa Cache'den al
            if (await _redisService.GetDb().KeyExistsAsync(_basketKeyForRedis))
            {
                return Response<IEnumerable<BasketDto>>.Success(await GetBasketsFromCacheAsync(), 200);
            } // Rediste yoksa SQL'den al.
            else
            {
                var baskets = await _basketRepository.GetBasketsWithBasketDetails(userId);

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

                await LoadBasketsToCacheAsync(dtoBaskets);

                return Response<IEnumerable<BasketDto>>.Success(dtoBaskets, 200);
            }
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

                await _basketRepository.AddAsync(basket);
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


                await LoadBasketToCacheAsync(basket);

                return Response<NoDataDto>.Success(201);
            }
        }

        //Sipariş listesini Redise yükle
        private async Task LoadBasketsToCacheAsync(IEnumerable<BasketDto> baskets)
        {
            foreach (var basket in baskets)
            {
                await _redisService.GetDb().HashSetAsync(_basketKeyForRedis, basket.Id, JsonSerializer.Serialize(basket));
            }
        }

        //Tek bir siparişi Redise yükle
        private async Task LoadBasketToCacheAsync(Basket basket)
        {
            BasketDto basketDto = new BasketDto();

            basketDto.Id = basket.Id;
            basketDto.IPAdress = basket.IPAdress;
            basketDto.Address = basket.Address;
            basketDto.Date = basket.Date;
            basketDto.UserId = basket.UserId;
            basketDto.TotalPrice = basket.BasketDetails.Sum(i => i.Price * i.Quantity);
            basketDto.BasketDetails = basket.BasketDetails.Select(x => new BasketDetailDto
            {
                BasketId = x.BasketId,
                Price = x.Price,
                Quantity = x.Quantity,
                ProductId = x.ProductId,
                ProductCode = x.Product.Code,
                ProductName = x.Product.Name
            }).ToList();

            await _redisService.GetDb().HashSetAsync(_basketKeyForRedis, basket.Id, JsonSerializer.Serialize(basket));
        }

        //Sipariş Listesini Redisten al.
        private async Task<IEnumerable<BasketDto>> GetBasketsFromCacheAsync()
        {
            var baskets = new List<BasketDto>();
            var cacheBaskets = await _redisService.GetDb().HashGetAllAsync(_basketKeyForRedis);

            foreach (var cacheBasket in cacheBaskets)
            {
                var basket = JsonSerializer.Deserialize<BasketDto>(cacheBasket.Value);
                baskets.Add(basket);
            }

            return baskets;
        }
    }
}
