namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Naive Bayes Classifier Variable
    /// </summary>
    public class NaiveVar
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
        /// Variable Code System
        /// </summary>
        public string System { get; set; }
        /// <summary>
        /// Variable Prior
        /// </summary>
        public double Prior { get; set; }
        /// <summary>
        /// Variable Weight
        /// </summary>
        public double Weight { get; set; }

        

    }
}