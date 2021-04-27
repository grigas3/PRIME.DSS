using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PRIME.Core.Common.Models
{


    public class DrugItem
    {

        public long RXCUI { get; set; }
        public string Name { get; set; }
        public string CMLIds { get; set; }

    }

    public interface IDrugRepository
    {

        /// <summary>
        /// Get RXCUI based on drug name
        /// </summary>
        /// <param name="drug"></param>
        /// <returns></returns>
        Task<DrugItem> GetRXCUI(string drug);
        

    }
}
