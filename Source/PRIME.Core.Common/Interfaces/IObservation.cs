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
        /// Code Name space
        /// </summary>
        string CodeNameSpace { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        long Timestamp { get; set; }

        string Description { get; set; }
    }
}
