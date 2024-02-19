using AuthServer.Core.CommonDTOs;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Service.AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IGenericRepository<TEntity> _genericRepository;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await _genericRepository.GetAllAsync());

            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var productTEntity = await _genericRepository.GetByIdAsync(id);

            if (productTEntity == null)
            {
                return Response<TDto>.Fail("Id not found", 404, true);
            }

            var productTDto = ObjectMapper.Mapper.Map<TDto>(productTEntity);

            return Response<TDto>.Success(productTDto, 200);
        }

        public async Task<Response<NoDataDto>> RemoveAsync(int id)
        {
            var productTEntity = await _genericRepository.GetByIdAsync(id);

            if(productTEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            _genericRepository.Remove(productTEntity);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> UpdateAsync(TDto entity, int id)
        {
            var productTEntity = await _genericRepository.GetByIdAsync(id);

            if (productTEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            var updateTEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            _genericRepository.Update(updateTEntity);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var productsTEntity = await _genericRepository.Where(predicate).ToListAsync();

            var productsTDto = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(productsTEntity);

            return Response<IEnumerable<TDto>>.Success(productsTDto, 200);
        }
    }
}
