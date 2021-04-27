using System;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Naive Variable Instance
    /// </summary>
    public class NaiveVarInstance
    {
        /// <summary>
        /// Variable Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Variable Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Variable Preset
        /// </summary>
        public bool Present { get; set; }
    }
}