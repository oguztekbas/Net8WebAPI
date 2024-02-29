using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class BasketRepository : GenericRepository<Basket>, IBasketRepository
    {
        //_dbSet => Baskets
        public BasketRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IQueryable<Basket>> GetBasketsWithBasketDetails(string userId)
        {
            var basketsWithBasketDetails =  _dbSet.Where(i => i.UserId == userId)
                .Include(i => i.BasketDetails)
                .OrderByDescending(i => i.Date)
                .AsNoTracking();

            return basketsWithBasketDetails;
        }
    }
}
