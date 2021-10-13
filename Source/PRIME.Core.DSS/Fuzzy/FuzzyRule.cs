using System.Collections.Generic;

namespace PRIME.Core.DSS.Fuzzy
{
    /// <summary>
    ///   Fuzzy Rule
    /// </summary>
    public class FuzzyRule
    {

        /// <summary>
        /// Name of Rule
        /// In case of one variable typically the name is the same with the OrRule
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Fuzzyor Crisp
        /// </summary>
        public bool Fuzzy { get; set; }
        /// <summary>
        /// OR Rules
        /// </summary>
        public List<OrRule> OrRules { get; set; }
    }
}