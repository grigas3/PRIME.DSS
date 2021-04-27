using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PRIME.Core.Common.Interfaces
{


    public interface IConditionRepository
    {

        /// <summary>
        /// Init Repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Init(string id);

        /// <summary>
        /// Has Condition
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        bool? HasCondition(string code,string system);
        /// <summary>
        /// Has Condition with Converter
        /// </summary>
        /// <param name="code"></param>
        /// <param name="system"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        bool? HasCondition(string code, string system, Func<object, bool> convert);

    }
}
