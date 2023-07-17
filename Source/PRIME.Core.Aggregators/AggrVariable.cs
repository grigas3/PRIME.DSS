using System.ComponentModel;
using Newtonsoft.Json;

namespace PRIME.Core.Aggregators
{
    /// <summary>
    /// Aggregation Variable Definition
    /// </summary>
    public class AggrVariable
    {

        //TODO: Probably Remove Source
        /// <summary>
        /// Source of Variable. The source can be 1) observation and 2) clinical
        /// </summary>
        [Description("Source of the variable the source can be 1) observation and 2) clinical")]
        public string Source { get; set; }
        /// <summary>
        /// Code 
        /// </summary>
        [Description("Variable code")]
        [JsonRequired]
        public string Code { get; set; }
        /// <summary>
        /// Weight
        /// </summary>
        [Description("Variable weight. This is the A in the Ax+B regression function.")]
        [JsonRequired]
        public double Weight { get; set; }

        /// <summary>
        /// Variable Min Value
        /// </summary>
        public double? Min { get; set; }
        /// <summary>
        /// Variable MAx Value
        /// </summary>
        public double? Max { get; set; }

    }
}