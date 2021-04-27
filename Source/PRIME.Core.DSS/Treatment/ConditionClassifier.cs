namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Condition Classifier Based on Naive Bayes Classifier
    /// </summary>
    public class ConditionClassifier : NaiveBayes
    {
        /// <summary>
        /// Condition Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Condition Code
        /// </summary>
        public string Code { get; set; }
    }
}