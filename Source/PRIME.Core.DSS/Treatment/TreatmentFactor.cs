namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Treatment Factor
    /// </summary>
    public class TreatmentFactor
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Treatment Effect
        /// </summary>
        public TreatmentFactorEffect Effect { get; set; }
    }


    public class Precondition
    {

        public string Code { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Presence
        /// </summary>
        public bool? Presence { get; set; }

    }
}