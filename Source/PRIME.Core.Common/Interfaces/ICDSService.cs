using PRIME.Core.Common.Models.CDS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Common.Models;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.Common.Interfaces
{
    public interface ICDSService
    {

        List<Card> GetCards(IConditionRepository repo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IEnumerable<Card>> GetCardsAsync(IConditionRepository repository, DSSModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Card>> GetCardsAsync(IConditionRepository repository,int id);

        /// <summary>
        /// Get Cards with Stats
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<Card>,EvaluationStats>> GetCardsWithStatsAsync(IConditionRepository repository, int id);


        /// <summary>
        /// Get Cards with Stats
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<Card>, EvaluationStats, List<DSSValue>>> GetCardsWithStatsAsync(IConditionRepository repository, DSSModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <param name="userOnly"></param>
        /// <returns></returns>
        Task<IEnumerable<ICondition>> GetConditionsAsync(IConditionRepository repository, int id,bool userOnly=false);
    }
}
