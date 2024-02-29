using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IBasketRepositoryWithCache : IGenericRepository<Basket>
    {
        IQueryable<Basket> GetBasketsWithBasketDetails(string userId);
    }
}
