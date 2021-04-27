using PRIME.Core.Common.Exceptions;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Web.Entities;
using System;
using System.Linq;

namespace PRIME.Core.Web.Context
{

    /// <summary>
    /// Aggregation Definition Provider
    /// </summary>
    public class AggrDefinitionProvider: IAggrDefinitionProvider
    {

        #region Private Declarations
        private readonly Context.DSSContext _context;
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public AggrDefinitionProvider(Context.DSSContext context)
        {
            _context = context;
            
        }

        /// <summary>
        /// Get Config in JSON format from meta-observation code
        /// </summary>
        /// <param name="code">Meta-observation code</param>
        /// <returns></returns>
        public string GetJsonConfigFromCode(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }


            var model=_context.Set<AggrModel>().FirstOrDefault(e => e.Code == code);

            if (model == null)
                throw new AggrDefinitionNotFoundException(code);


            return model.Config;

        }
    }
}
