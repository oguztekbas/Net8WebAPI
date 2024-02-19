using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.UnitOfWork
{
    //UnitOfWork'un amacı veri tutarlılığı. Yani beginTransaction rollback kodlarını yazmadan
    // SaveChanges işlemini bir hamlede yapabilmek için Repository paterniyle kullanılan eş bir patern.
    public interface IUnitOfWork
    {
        Task CommitAsync(); 
        void Commit();
        Task RollbackAsync();
        void Rollback();

        DbContext GetDbContext();
    }
}
