using PRIME.Core.Common.Models.CDS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.Common.Interfaces
{
    public interface ICDSService
    {

        List<Card> GetCards(IConditionRepository repo);
        Task<IEnumerable<Card>> GetCardsAsync(IConditionRepository repository, DSSModel model);
        Task<IEnumerable<Card>> GetCardsAsync(IConditionRepository repository,int id);
        Task<IEnumerable<ICondition>> GetConditionsAsync(IConditionRepository repository, int id,bool userOnly=false);
    }
}
