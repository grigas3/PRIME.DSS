using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS.Fuzzy;

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


        /// <summary>
        /// Classifier based on Naive Bayes Model
        /// </summary>
        public TreatmentProbModel NaiveModel { get; set; }


        /// <summary>
        /// Classifier Based on Rules 
        /// </summary>
        public FuzzyCollection RuleModel { get; set; }


        [JsonIgnore]
        public IEnumerable<IVariable> Variables
        {

            get
            {
                if (NaiveModel != null)
                    return NaiveModel.Variables;


                if (RuleModel != null)
                    return RuleModel.Variables;

                return new List<IVariable>();
            }

        }

        public static ConditionClassifier FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<ConditionClassifier>(json);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



    }
}