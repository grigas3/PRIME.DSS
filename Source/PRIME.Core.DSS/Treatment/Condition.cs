using PRIME.Core.Common.Interfaces;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// 
    /// </summary>
    public class Condition:ICondition
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CodeNameSpace { get; set; }
        public double Value { get; set; }
        
    }
}