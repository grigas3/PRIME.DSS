using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Interfaces
{
    /// <summary>
    /// Job Factory
    /// </summary>
    public interface IJobFactory
    {
        IEnumerable<IRecurringJob> GetJobs();
    }
}
