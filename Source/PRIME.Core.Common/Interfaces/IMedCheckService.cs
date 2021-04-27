using PRIME.Core.Common.Models.RX;
using PRIME.Core.MedCheck;
using PRIME.Core.MedCheck.Gene;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PRIME.Core.Common.Interfaces
{


    /// <summary>
    /// Medication Check Service
    /// </summary>
    public interface IMedCheckService
    {
        /// <summary>
        /// Main Medication check method
        /// </summary>
        /// <param name="genes">Genes</param>
        /// <param name="drugs"></param>
        /// <returns></returns>
        Task<CheckResponse> Check(string genes, string drugs);
    }
}
