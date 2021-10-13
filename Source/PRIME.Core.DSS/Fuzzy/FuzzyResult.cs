using System.Collections.Generic;

namespace PRIME.Core.DSS.Fuzzy
{
    public class FuzzyResult
    {
        public List<string> MissingVariables { get; } = new List<string>();

        public string Rule { get; set; }
        public string Variable { get; set; }

        public double Result { get; set; }
    }
}