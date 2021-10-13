using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Context.Entities;
using PRIME.Core.Models;

namespace PRIME.Core.Common.Interfaces
{

    /// <summary>
    /// Aggregator interface
    /// </summary>
    public interface IAggregator
    {
        /// <summary>
        /// Run Aggregation for specific patient and code
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="code">Meta Observation Code</param>
        /// <param name="lastExecutionTime">Last Execution time</param>
        /// <param name="aggregationType"></param>
        /// <param name="filterType"></param>
        /// <returns>List of observations</returns>
        Task<IEnumerable<IObservation>> Run(string patientId, string code, string codeNamepace, DateTime? lastExecutionTime,string aggregationType= null, string filterType = null);

        Task<IEnumerable<IObservation>> Run(string patientId, string code,string codeNamespace, IEnumerable<PDObservation> rawObs, string config);

        Task<IObservation> RunSingle(string patientId, string code, string codeNamespace, IEnumerable<PDObservation> rawObs, string config);



    }
}
