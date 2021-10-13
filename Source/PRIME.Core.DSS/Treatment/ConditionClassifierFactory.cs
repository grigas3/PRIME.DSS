using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS.Fuzzy;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// 
    /// </summary>
    public class ConditionClassifierFactory
    {
        private readonly List<ConditionClassifier> _conditionClassifiers = new List<ConditionClassifier>();

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            var files = Directory.GetFiles(path, "*.json");


            foreach (var c in files)
            {
                var json = File.ReadAllText(c);
                _conditionClassifiers.Add(JsonConvert.DeserializeObject<ConditionClassifier>(json));
            }
        }


        /// <summary>
        /// Add Treatment Classifier
        /// </summary>
        /// <param name="classifier"></param>
        public void Add(ConditionClassifier classifier)
        {
            _conditionClassifiers.Add(classifier);

        }

        /// <summary>
        /// Get Condition Options
        /// </summary>
        /// <param name="treatment"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<Condition> GetConditions(IConditionRepository repository)
        {
            List<Condition> options = new List<Condition>();
            foreach (var c in _conditionClassifiers)
            {

                if (c.Variables == null) continue;


                if (c.NaiveModel != null)
                {
                    options.AddRange(GetConditionsNaive(repository, c, c.NaiveModel));
                }

                if (c.RuleModel != null)
                {
                    options.AddRange(GetConditionsRule(repository, c, c.RuleModel));
                }


            }

            return options;
        }


        /// <summary>
        /// Get Condition Options
        /// </summary>
        /// <param name="treatment"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<Condition> GetConditionOptions(TreatmentOption treatment, IConditionRepository repository)
        {
            List<Condition> options = new List<Condition>();
            foreach (var c in _conditionClassifiers)
            {
                if(c.Variables==null)continue;
                

                if (c.NaiveModel != null)
                {
                    options.AddRange(GetConditionsNaive(repository, c, c.NaiveModel));
                }

                if (c.RuleModel != null)
                {
                    options.AddRange(GetConditionsRule(repository, c, c.RuleModel));
                }

            }

            return options;
        }
     

        /// <summary>
        /// Get Treatment Options based on Naive Bayes Classifier
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="classifier"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<Condition> GetConditionsNaive(IConditionRepository repository, ConditionClassifier classifier, NaiveBayes model)
        {
            List<Condition> options = new List<Condition>();
            List<NaiveVarInstance> vInputs = new List<NaiveVarInstance>();
            var p = model.GetOutput(vInputs);
            foreach (var v in model.Variables)
            {
              
                var    codeNameSpace = "PRIME";
                var b = repository.HasCondition(v.Code.ToLower(), codeNameSpace);

                if (b.HasValue)
                {
                    vInputs.Add(new NaiveVarInstance() { Code = v.Code.ToLower(), Name = v.Name, Present = b.Value });
                }
            }
            if (p > 0.1)
            {
                options.Add(new Condition()
                {
                    Name = classifier.Name,
                    Code = classifier.Code.ToLower(),
                    Value = p,
                 
                });
            }

            return options;
        }

        /// <summary>
        /// Get Treatment Options based on Rules
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="classifier"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<Condition> GetConditionsRule(IConditionRepository repository, ConditionClassifier classifier, FuzzyCollection model)
        {
            List<Condition> options = new List<Condition>();
            Dictionary<string, object> valueDictionary = new Dictionary<string, object>();
            foreach (var v in model.Variables)
            {

                string codeNameSpace = v.CodeNameSpace;
                if (v.CodeNameSpace == null)
                    codeNameSpace = "PRIME";

                var b = repository.GetCondition(v.Code, codeNameSpace);

                if (b.HasValue)
                {
                    valueDictionary.Add(v.Code, b.Value);
                }

            }
            var p = FuzzyEngine.GetInference(model, valueDictionary);



            options.Add(new Condition()
            {
                Name = classifier.Name,
                Code = classifier.Code.ToLower(),
                Value = p.Result,

            });



            return options;

        }

    }
}