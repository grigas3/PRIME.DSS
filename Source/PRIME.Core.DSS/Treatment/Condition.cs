namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// 
    /// </summary>
    public class Condition
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public double Probability { get; set; }
        public TreatmentOption Treatment { get; set; }
    }
}