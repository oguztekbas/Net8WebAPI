using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IBasketDetailRepository : IGenericRepository<BasketDetail>
    {
        Task AddRangeAsync(IEnumerable<BasketDetail> entities);
    }
}
