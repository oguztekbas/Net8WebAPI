using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class BasketDetailRepository : GenericRepository<BasketDetail>, IBasketDetailRepository
    {
        //_dbSet => BasketDetails
        public BasketDetailRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddRangeAsync(IEnumerable<BasketDetail> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
    }
}
