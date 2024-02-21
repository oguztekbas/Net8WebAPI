using AuthServer.Core.CommonDTOs;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IProductService
    {
        Task<Response<ProductDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<ProductDto>>> GetAllAsync();
        Task<Response<ProductDto>> AddAsync(ProductDto entity);
        Task<Response<NoDataDto>> RemoveAsync(int id);
        Task<Response<NoDataDto>> UpdateAsync(ProductDto entity, int id);
    }
}
