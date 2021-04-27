using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Models
{


    public class ConditionsCodes
    {

        public const string FLUCTUATIONS = "FLUCTUATIONS";
        public const string DYSKINESIAS = "DYSKINESIAS";

    }
  

    
    public class Patient
    {

        public List<string> Genes { get; set; }

        public List<string> Conditions { get; set; }

        public List<string> Medications { get; set; }

    }
}
