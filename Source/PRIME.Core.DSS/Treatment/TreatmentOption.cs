using System.Collections.Generic;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Treatment Option
    /// </summary>
    public class TreatmentOption
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Replacement Code
        /// </summary>
        public string ReplacementCode { get; set; }
        /// <summary>
        /// Treatment option
        /// </summary>
        public TreatmentAdmission Option { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public double Probability { get; set; }

        /// <summary>
        /// Factores
        /// </summary>
        public List<TreatmentFactor> Factors { get; set; }
        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Missing Variables
        /// </summary>
        public List<string> MissingVariables { get; set; }
        /// <summary>
        /// Preconditions
        /// </summary>
        public List<Precondition> Preconditions { get; set; }
        /// <summary>
        /// Source of DSS
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Outcome
        /// </summary>
        public string Outcome { get; set; }
    }
}