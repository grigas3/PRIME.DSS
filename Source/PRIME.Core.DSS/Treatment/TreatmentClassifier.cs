namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Treatment Classifier
    /// Each treatment option has a classifier 
    /// </summary>
    public class TreatmentClassifier : NaiveBayes
    {
        /// <summary>
        /// Treatment Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Treatment Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Treatment Code Namespace
        /// </summary>
        public string CodeNamespace { get; set; }
        /// <summary>
        /// Replacement Code
        /// </summary>
        public string ReplacementCode { get; set; }
        /// <summary>
        /// Replacement Code namespace
        /// </summary>
        public string ReplacementCodeNamespace { get; set; }
        /// <summary>
        /// Option
        /// </summary>
        public TreatmentAdmission Option { get; set; }
        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }

    }
}