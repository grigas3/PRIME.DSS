using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Interfaces
{
    /// <summary>
    /// Basic interface with the minimum number of properties an Observation should have
    /// </summary>
    public interface IObservation:ICondition
    {
        
        /// <summary>
        /// Timestamp
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Category { get; set; }
    }
}
