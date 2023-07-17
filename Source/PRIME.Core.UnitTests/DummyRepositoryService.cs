using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Common.Entities;
using PRIME.Core.Common.Interfaces;
using Remote.Linq;

namespace PRIME.Core.UnitTests
{
    /// <summary>
    /// Dummy Repository Service
    /// </summary>
    internal class DummyRepositoryService:IRepositoryService
    {
        private readonly Dictionary<Type,List<object>> _sets=new Dictionary<Type, List<object>>();

        public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> query) where T : BaseEntity
        {

            if(!_sets.ContainsKey(typeof(T)))
                return new List<T>();

            var ret=(await _sets[typeof(T)].ToDynamicListAsync<T>()).AsQueryable().Where(query);
            return ret;

        }

        public async Task<IEnumerable<T>> GetAsync<T>() where T : BaseEntity
        {
            if (!_sets.ContainsKey(typeof(T)))
                return new List<T>();
            var ret = (await _sets[typeof(T)].ToDynamicListAsync<T>());
            return ret;
        }

        public async Task<IEnumerable<T>> GetAsync<T>(string includes) where T : BaseEntity
        {
           
            var r = await GetAsync<T>();
            return r;
            
        }

        public async Task<T> FindAsync<T>(int id) where T : BaseEntity
        {
            var r = await GetAsync<T>();
            return r.FirstOrDefault(e => e.Id == id);
        }

        public Task DeleteAsync<T>(int id) where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task InserOrUpdateAsync<T>(T item) where T : BaseEntity
        {

            if (_sets.ContainsKey((typeof(T))))
            {
                _sets[typeof(T)].Add(item);
            }
            else
            {
                _sets.Add(typeof(T),new List<object>(){ item});
            }
            
        }
    }
}
