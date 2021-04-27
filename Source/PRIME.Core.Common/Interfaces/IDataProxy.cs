using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRIME.Core.Common.Interfaces
{
    /// <summary>
    /// Data Proxy Interface for Get/Insert data to PRIME Cloud Repository
    /// </summary>
    public interface IDataProxy
    {
        
        /// <summary>
        /// Get a list of items from the main repository
        /// </summary>
        /// <typeparam name="T">The type of item</typeparam>
        /// <param name="id"></param>
        /// <param name="take">Take</param>
        /// <param name="skip">Skip</param>
        /// <param name="filter">Filter (Defined per type)</param>
        /// <param name="sort">Sort property</param>
        /// <param name="sortdir">Sort direction</param>
        /// <param name="lastmodified">Last modified (for syncing)</param>
        /// <returns>List of T items</returns>
        Task<IEnumerable<T>> Get<T>(int take, int skip, string filter, string sort, string sortdir = "false", long lastmodified = -1) where T : class;

        /// <summary>
        /// Get a single item
        /// </summary>
        /// <typeparam name="T">Item</typeparam>

        /// <param name="id">Item id</param>        
        /// <returns>A single T item</returns>
        Task<T> Get<T>(string id) where T : class;
    }
}