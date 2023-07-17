using PRIME.Core.Common.Models.RX;
using PRIME.Core.MedCheck;
using PRIME.Core.MedCheck.Gene;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PRIME.Core.Common.Interfaces
{
    public class StichMatchItem
    {
        public int queryIndex { get; set; }
        public string queryItem { get; set; }
        public string stringId { get; set; }
        public int ncbiTaxonId { get; set; }
        public string taxonName { get; set; }
        public string preferredName { get; set; }
        public string annotation { get; set; }
    }
    public class StichInteractionItem
    {
        public string nameA { get; set; }
        public string nameB { get; set; }
        public string idA { get; set; }
        public string idB { get; set; }
        public string score { get; set; }
        
    }
    /// <summary>
    /// Medication Check Service
    /// </summary>
    public interface IProteinCheckService
    {
        /// <summary>
        /// Main Medication check method
        /// </summary>
        /// <param name="identifiers">Genes</param>
        /// <returns></returns>
        Task<List<StichInteractionItem>> Check(List<string> identifiers);

        Task<List<StichMatchItem>> MatchItems(string term);
    }

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
