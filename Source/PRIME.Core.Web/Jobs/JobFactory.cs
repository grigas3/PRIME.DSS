using Microsoft.Extensions.DependencyInjection;
using PRIME.Core.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIME.Core.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    public  class JobFactory:IJobFactory
    {
        #region Private Declarations
        private readonly IServiceProvider _serviceProvider;
        #endregion
        /// <summary>
        /// Job Factory
        /// </summary>
        /// <param name="serviceProvider"></param>
        public JobFactory(IServiceProvider serviceProvider)
        {

            _serviceProvider = serviceProvider;


        }

        /// <summary>
        /// Get Jobs
        /// </summary>
        /// <returns></returns>
        public  IEnumerable<IRecurringJob> GetJobs()
        {            
            var list= _serviceProvider.GetServices<IRecurringJob>();

            return list;

        }

    }
}
