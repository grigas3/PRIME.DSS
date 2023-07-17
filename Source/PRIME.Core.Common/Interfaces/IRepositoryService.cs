using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Common.Entities;

namespace PRIME.Core.Common.Interfaces
{

    /// <summary>
    /// Repository Service
    /// </summary>
    public interface IRepositoryService
    {
        /// <summary>
        /// Get async with Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> query) where T : BaseEntity;
        /// <summary>
        /// Get Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync<T>() where T : BaseEntity;
        /// <summary>
        /// Get Async with includes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync<T>(string includes) where T : BaseEntity;

        /// <summary>
        /// Find Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindAsync<T>(int id) where T : BaseEntity;

        /// <summary>
        /// Delete Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync<T>(int id) where T : BaseEntity;

        /// <summary>
        /// Insert Or Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        Task InserOrUpdateAsync<T>(T item) where T : BaseEntity;

    }
}
