using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Common.Entities;

namespace PRIME.Core.Common.Interfaces
{
    public interface IRepositoryService
    {
        /// <summary>
        /// Get async with Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> query) where T : BaseEntity;
         Task<IEnumerable<T>> GetAsync<T>() where T : BaseEntity;
        Task<IEnumerable<T>> GetAsync<T>(string includes) where T : BaseEntity;
        Task<T> FindAsync<T>(int id) where T : BaseEntity;
        Task DeleteAsync<T>(int id) where T : BaseEntity;
        Task InserOrUpdateAsync<T>(T item) where T : BaseEntity;

    }
}
