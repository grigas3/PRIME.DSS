using System.Collections.Generic;
using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.DSS.Fuzzy
{

    public class FuzzyVariable : IVariable
    {
        public string CodeNameSpace { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class FuzzyCollection
    {

    
        public FuzzyCollection()
        {

        }

        public FuzzyCollection(IEnumerable<string> variables)
        {
            this.Variables=new List<FuzzyVariable>();
            foreach (var v in variables)
            {
                this.Variables.Add(new FuzzyVariable(){ Name=v,Code=v});
            }

        }

        /// <summary>
        /// Variables of Collection
        /// </summary>
        public List<FuzzyVariable> Variables { get; set; }

        /// <summary>
        /// Rules
        /// </summary>
        public List<FuzzyRule> Rules { get; set; }
    }
}