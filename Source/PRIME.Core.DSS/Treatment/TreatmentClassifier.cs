using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS.Fuzzy;

namespace PRIME.Core.DSS.Treatment
{
    public class TreatmentProbModel : NaiveBayes
    {

    }

    /// <summary>
    /// Treatment Classifier
    /// Each treatment option has a classifier 
    /// </summary>
    public class TreatmentClassifier: ITreatmentClassifier
    {
        /// <summary>
        /// Classifier based on Naive Bayes Model
        /// </summary>
        public TreatmentProbModel NaiveModel { get; set; }


        /// <summary>
        /// Classifier Based on Rules 
        /// </summary>
        public FuzzyCollection RuleModel { get; set; }

        /// <summary>
        /// Dexi Model
        /// </summary>
        public DSSConfig DexiModel { get; set; }

        [JsonIgnore]
        public IEnumerable<IVariable> Variables
        {

            get
            {

                List<IVariable> variables=new List<IVariable>();

                if(Preconditions!=null)
                    variables.AddRange(Preconditions);
                if (NaiveModel != null)
                    variables.AddRange(NaiveModel.Variables);

                if (DexiModel != null)
                    variables.AddRange(DexiModel.Input);

                if (RuleModel != null)
                    variables.AddRange(RuleModel.Variables);

                return variables;
            }

        }


        public IEnumerable<FuzzyVariable> Preconditions
        {

            get;
            set;

        }


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

        public string Source { get; set; }


        public static TreatmentClassifier FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TreatmentClassifier>(json);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public interface ITreatmentClassifier
    {



    }

}