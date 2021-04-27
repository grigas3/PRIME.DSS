using Newtonsoft.Json;
using PRIME.Core.Common.Models;
using PRIME.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Extensions
{
    /// <summary>
    /// Common Model Extensions
    /// </summary>
    public static class CommonModelExtensions
    {

        #region Helpers
        /// <summary>
        /// Get Clinical Information List
        /// The basic info are the Code and the Value
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ClinicalInfo> GetClinicalInfoList(this PDPatient patient)
        {
            if (string.IsNullOrEmpty(patient.ClinicalInfo))
            {
                //If CLinical info string is null or emtpy return empty lsit

                return new List<ClinicalInfo>();
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<ClinicalInfo>>(patient.ClinicalInfo);
                }
                catch
                {
                    return new List<ClinicalInfo>();
                }
            }
        }
        #endregion
    }
}
