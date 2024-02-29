using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IBasketRepository : IGenericRepository<Basket>
    {
        Task<IQueryable<Basket>> GetBasketsWithBasketDetails(string userId);
    }
}
