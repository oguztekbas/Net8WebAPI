using AuthServer.Core.CommonDTOs;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Service.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ProductService : IProductService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IGenericRepository<Product> _genericRepository;
        public ProductService(IUnitOfWork unitOfWork, IGenericRepository<Product> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<IEnumerable<ProductDto>>(await _genericRepository.GetAllAsync());

            return Response<IEnumerable<ProductDto>>.Success(products, 200);
        }

        public async Task<Response<ProductDto>> AddAsync(ProductDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<Product>(entity);

            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<ProductDto>(newEntity);

            return Response<ProductDto>.Success(newDto, 200);
        }

        public async Task<Response<ProductDto>> GetByIdAsync(int id)
        {
            var productTEntity = await _genericRepository.GetByIdAsync(id);

            if (productTEntity == null)
            {
                return Response<ProductDto>.Fail("Id not found", 404, true);
            }

            var productTDto = ObjectMapper.Mapper.Map<ProductDto>(productTEntity);

            return Response<ProductDto>.Success(productTDto, 200);
        }

        public async Task<Response<NoDataDto>> RemoveAsync(int id)
        {
            var productTEntity = await _genericRepository.GetByIdAsync(id);

            if (productTEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            _genericRepository.Remove(productTEntity);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> UpdateAsync(ProductDto entity, int id)
        {
            var productTEntity = await _genericRepository.GetByIdAsync(id);

            if (productTEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            var updateTEntity = ObjectMapper.Mapper.Map<Product>(entity);

            _genericRepository.Update(updateTEntity);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }
    }
}
