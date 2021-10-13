using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.File;
using PRIME.Core.Common.Entities;
using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.Web.Context
{

    public class RepositoryService:IRepositoryService
    {
        private readonly DSSContext _context;

        public RepositoryService(DSSContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAsync<T>() where T : BaseEntity
        {
            var res=await _context.Set<T>().ToListAsync();
            return res;
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> query) where T : BaseEntity
        {
            var res = await _context.Set<T>().Where(query).ToListAsync();
            return res;
        }

        public async Task<IEnumerable<T>> GetAsync<T>(string includeExpression) where T : BaseEntity
        {
            var res = await _context.Set<T>().Include(includeExpression).ToListAsync();
            return res;
        }


        public async Task<T> FindAsync<T>(int id) where T : BaseEntity
        {
            var res = await _context.FindAsync<T>(id);
            return res;
        }

        public async Task DeleteAsync<T>(int id) where T : BaseEntity
        {
            var res = await FindAsync<T>(id);
            _context.Set<T>().Remove(res);
            await _context.SaveChangesAsync();
           
        }

        public async Task InserOrUpdateAsync<T>(T item ) where T : BaseEntity
        {
            if (item.Id == 0)
            {
                item.CreatedDate=DateTime.Now;
                item.CreatedBy = "admin";
                _context.Set<T>().Add(item);
            }
            else
            {
                item.ModifiedDate = DateTime.Now;
                item.ModifiedBy = "admin";
                _context.Update(item);
            }
            await _context.SaveChangesAsync();
        }
    }
}
