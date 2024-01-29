using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //Services klasörü içindeki interface'ler Service katmanında iplemente edilir.
    public interface IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GetAllAsync();
        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);
        Task<Response<TDto>> AddAsync(TDto entity);
        Task<Response<NoDataDto>> Remove(int id);
        Task<Response<NoDataDto>> Update(TDto entity, int id);
    }
    /// Buradaki Service Interface'inde şunu yapmaya çalışıyoruz:
    /// Service katmanında automapper ile mapleme işlemi yapacağımız ve Entity yerine DTo döneceğimiz için
    /// Çevirme IGenericRepository'e göre dönüş tipleri değişti
    /// ve Api tarafına döneceğimiz Response class'ımızı da ekledik.
    /// 
}
