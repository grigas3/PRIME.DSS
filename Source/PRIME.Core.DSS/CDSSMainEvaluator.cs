using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS.Dexi;

namespace PRIME.Core.DSS
{

  
    public class CDSSMainEvaluator
    {
        /// <summary>
        /// Evaluate Async
        /// </summary>
        /// <param name="cDsService"></param>
        /// <param name="aggregator"></param>
        /// <param name="repositoryService"></param>
        /// <param name="repository"></param>
        /// <param name="patientId"></param>
        /// <param name="clientId"></param>
        /// <param name="clientCode"></param>
        /// <param name="evaluateDss"></param>
        /// <returns></returns>
        public static async Task<Tuple<object,EvaluationStats>> EvaluateAsync(
            ICDSService cDsService,
            IAggregator aggregator,
            IRepositoryService repositoryService,
            IConditionRepository repository,
            string patientId,
            int? clientId,
            string clientCode,
            bool evaluateDss
          
            )
        {
            CDSClient client = null;
            EvaluationStats stats=new EvaluationStats();
            if (clientId.HasValue)
            {
                client = (await repositoryService.GetAsync<CDSClient>(e => e.Id == clientId.Value))
                    .FirstOrDefault();
            }
            else
                client = (await repositoryService.GetAsync<CDSClient>(e => e.Code == clientCode)).FirstOrDefault();
            if (client == null)
            {
                throw new Exception($"Client with code {clientCode} does not exist");
                
            }

            var id = client.Id;
            var aggregators = (await repositoryService.GetAsync<AggrModel>(e => e.CDSClientId == id)).ToList();
            //Init Condition Repository
            await repository.Init(patientId);
            //Perform Aggregations
            await repository.Aggregate(patientId, aggregator, aggregators);

            if (evaluateDss)
            {
                //Get DSS Cards
                var ret = await cDsService.GetCardsWithStatsAsync(repository, id);
                var cards = ret.Item1;
                return new Tuple<object, EvaluationStats>(ret.Item1, ret.Item2);
            }
            else
            {
                var inputs = await cDsService.GetConditionsAsync(repository, id, true);
                return new Tuple<object, EvaluationStats>(inputs, stats);
            }
        }

        /// <summary>
        /// Evaluate Async
        /// </summary>
        /// <param name="cDsService"></param>
        /// <param name="aggregator"></param>
        /// <param name="repositoryService"></param>
        /// <param name="repository"></param>
        /// <param name="patientId"></param>
        /// <param name="clientId"></param>
        /// <param name="clientCode"></param>
        /// <param name="evaluateDss"></param>
        /// <returns></returns>
        public static async Task<Tuple<object, EvaluationStats,IEnumerable<DSSValue>>> EvaluateDSSAsync(
            ICDSService cDsService,
            IAggregator aggregator,
            IRepositoryService repositoryService,
            IConditionRepository repository,
            string patientId,
            List<AggrModel> aggregators,
            DSSModel model
            

            )
        {
            //Init Condition Repository
            await repository.Init(patientId);
            //Perform Aggregations
            
            await repository.Aggregate(patientId, aggregator, aggregators);

        
                //Get DSS Cards
                var ret = await cDsService.GetCardsWithStatsAsync(repository, model);
                var cards = ret.Item1;
                return new Tuple<object, EvaluationStats, IEnumerable<DSSValue>>(ret.Item1, ret.Item2,ret.Item3);
          
        }

    }
}
