using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PRIME.Core.Context.Entities;

namespace PRIME.Core.Common.Interfaces
{
    public class ConditionResult:ICondition
    {

        public string Code { get; set; }
        public string CodeNameSpace { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public string Name { get; set; }
    }


    public interface ICondition{

        double Value { get; set; }
        string Name { get; set; }
        string Code { get; set; }
        string CodeNameSpace { get; set; }
    }

    public interface IConditionRepository
    {
        
        /// <summary>
        /// Init Repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Init(string id);



        /// <summary>
        /// Aggregate
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="aggregator"></param>
        /// <param name="aggregators"></param>
        /// <returns></returns>
        Task Aggregate(string patientId, IAggregator aggregator, List<AggrModel> aggregators);

        /// <summary>
        /// Has Condition
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        bool? HasCondition(string code,string system);
        /// <summary>
        /// Has Condition with Converter
        /// </summary>        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        bool? HasCondition(string code, string system, Func<object, bool> convert);

        /// <summary>
        /// Get Condition as double value
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        double? GetCondition(string code, string system);

        /// <summary>
        /// Add a condition
        /// </summary>
        /// <param name="oCode"></param>
        /// <param name=""></param>
        bool AddCondition(string oCode, string codeNamespace,double value=1.0);

        /// <summary>
        /// Remove Condition
        /// </summary>
        /// <param name="oCode"></param>
        /// <param name=""></param>
        void RemoveCondition(string oCode, string codeNamespace);


        ConditionResult GetConditionRes(string code, string system);
        Dictionary<string, string> ToDict(IValueMapping mapping);
        
    }


    public interface IValueMapping
    {
        int GetValue(string variable,double v);

    }
}
