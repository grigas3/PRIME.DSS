﻿using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS.Fuzzy;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PRIME.Core.Common.Models;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// 
    /// </summary>
    public class TreatmentClassifierFactory
    {
        private readonly List<TreatmentClassifier> _treatmentClassifiers = new List<TreatmentClassifier>();

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            var files = Directory.GetFiles(path, "*.json");
            _treatmentClassifiers.Clear();

            foreach (var c in files)
            {
                var json = File.ReadAllText(c);
                _treatmentClassifiers.Add(JsonConvert.DeserializeObject<TreatmentClassifier>(json));
            }
        }


        /// <summary>
        /// Add Treatment Classifier
        /// </summary>
        /// <param name="classifier"></param>
        public void Add(TreatmentClassifier classifier)
        {
            if(_treatmentClassifiers.All(e => e.Code != classifier.Code))
            _treatmentClassifiers.Add(classifier);

        }

        private static List<TreatmentOption> GetTreatmentDexiOptions(
            IConditionRepository repository,
            TreatmentClassifier classifier,
            DSSConfig deximodel)

        {
            List<TreatmentOption> options = new List<TreatmentOption>();
            var values = repository.ToDict(deximodel);
            var dssValues=DSSRunner.Run(deximodel, values);
            foreach (var c in dssValues)
            {
                if (c.Code == classifier.Code&&c.Value==deximodel.DexiOutputValue)
                {
                    options.Add(new TreatmentOption()
                    {
                        Name = classifier.Name,
                        ReplacementCode = classifier.ReplacementCode,
                        Code = classifier.Code.ToLower(),
                        Option = classifier.Option,
                        Probability = 1.0,
                        Outcome = c.Value,
                        Description = classifier.Description,
                        Summary = classifier.Summary,
                        Source = classifier.Source,
                    });
                }
            }
            return options;

        }
        private static IEnumerable<DSSValue> GetTreatmentDexiValues(
            IConditionRepository repository,
            TreatmentClassifier classifier,
            DSSConfig deximodel)

        {
            
            List<TreatmentOption> options = new List<TreatmentOption>();
            var values = repository.ToDict(deximodel);
            var dssValues = DSSRunner.Run(deximodel, values);
            return dssValues;
          

        }
        /// <summary>
        /// Get Treatment Options based on Naive Bayes Classifier
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="classifier"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<TreatmentOption> GetTreatmentNaiveOptions(IConditionRepository repository,
            TreatmentClassifier classifier,
            NaiveBayes model)
        {
            List<TreatmentOption> options = new List<TreatmentOption>();
            List<TreatmentFactor> factors = new List<TreatmentFactor>();
            List<NaiveVarInstance> vInputs = new List<NaiveVarInstance>();


            foreach (var v in model.Variables)
            {
                var b = repository.HasCondition(v.Code.ToLower(), classifier.CodeNamespace);

                if (b.HasValue)
                {
                    if (v.Weight > 0.5)
                    {
                        factors.Add(new TreatmentFactor()
                        {
                            Effect = b.Value ? TreatmentFactorEffect.Positive : TreatmentFactorEffect.Negative,
                            Name = v.Name
                        });
                    }
                    else
                    {
                        factors.Add(new TreatmentFactor()
                        {
                            Effect = b.Value ? TreatmentFactorEffect.Negative : TreatmentFactorEffect.Positive,
                            Name = v.Name
                        });
                    }

                    vInputs.Add(new NaiveVarInstance() { Code = v.Code.ToLower(), Name = v.Name, Present = b.Value });
                }
                else
                {
                    factors.Add(new TreatmentFactor() { Effect = TreatmentFactorEffect.Undetermined, Name = v.Name });
                }
            }


            var p = model.GetOutput(vInputs);

            if (p > 0.1)
            {
                options.Add(new TreatmentOption()
                {
                    Name = classifier.Name,
                    ReplacementCode = classifier.ReplacementCode,
                    Code = classifier.Code.ToLower(),
                    Option = classifier.Option,
                    Probability = p,
                    Factors = factors,
                    Summary = classifier.Summary,
                    Source=classifier.Source,
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
        private static List<TreatmentOption> GetTreatmentRuleOptions(IConditionRepository repository, TreatmentClassifier classifier, FuzzyCollection model)
        {
            List<TreatmentOption> options = new List<TreatmentOption>();
            List<TreatmentFactor> factors = new List<TreatmentFactor>();
            List<NaiveVarInstance> vInputs = new List<NaiveVarInstance>();

            Dictionary<string,object> valueDictionary=new Dictionary<string, object>();
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

            List<Precondition> preconditions=new List<Precondition>();
            if (classifier.Preconditions != null)
            {
                foreach (var pr in classifier.Preconditions)
                {

                    var b=repository.HasCondition(pr.Code, "PRIME");
                    preconditions.Add(new Precondition()
                    {
                        Code=pr.Code,
                        Name=pr.Name,
                        Presence = b

                    });

                    
                }
            }



            options.Add(new TreatmentOption()
                {
                    Name = classifier.Name,
                    ReplacementCode = classifier.ReplacementCode,
                    Code = classifier.Code.ToLower(),
                    Option = classifier.Option,
                    Probability = p.Result,
                    Factors = factors,
                    Summary = classifier.Summary,
                    Preconditions=preconditions,
                    MissingVariables=p.MissingVariables
                });
            


            return options;

        }

       

        /// <summary>
        /// Get Options
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<TreatmentOption> GetTreatmentOptions(IConditionRepository repository)
        {
            List<TreatmentOption> options = new List<TreatmentOption>();
            foreach (var c in _treatmentClassifiers)
            {

                if(c.Code==null)
                continue;

                var codeNamespace = c.CodeNamespace;
                if (string.IsNullOrEmpty(codeNamespace))
                    codeNamespace = "PRIME";
                
                var ec = repository.HasCondition(c.Code.ToLower(), codeNamespace);
                bool? er = new bool?();
                if (!string.IsNullOrEmpty(c.ReplacementCode)&& !string.IsNullOrEmpty(c.ReplacementCodeNamespace))
                    er = repository.HasCondition(c.ReplacementCode.ToLower(), c.ReplacementCodeNamespace.ToLower());


              

                if (c.Option == TreatmentAdmission.Add && ec.HasValue && ec.Value)
                    continue;

                if (c.Option == TreatmentAdmission.Remove && (!ec.HasValue || !ec.Value))
                    continue;

                if (c.Option == TreatmentAdmission.Replace &&
                    ((ec.HasValue && ec.Value) || (!er.HasValue || !er.Value)))
                    continue;

                if (c.NaiveModel != null)
                {
                    options.AddRange(GetTreatmentNaiveOptions(repository,c,c.NaiveModel));
                }

                if (c.DexiModel!=null)
                {

                    options.AddRange(GetTreatmentDexiOptions(repository, c, c.DexiModel));
                }

                if (c.RuleModel != null)
                {
                    options.AddRange(GetTreatmentRuleOptions(repository, c, c.RuleModel));
                }
            }

            return options;
        }


        /// <summary>
        /// Get Options
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<DSSValue> GetTreatmentOutputValues(IConditionRepository repository)
        {
            List<DSSValue> options = new List<DSSValue>();
            foreach (var c in _treatmentClassifiers)
            {

                if (c.Code == null)
                    continue;

                var codeNamespace = c.CodeNamespace;
                if (string.IsNullOrEmpty(codeNamespace))
                    codeNamespace = "PRIME";

                var ec = repository.HasCondition(c.Code.ToLower(), codeNamespace);
                bool? er = new bool?();
                if (!string.IsNullOrEmpty(c.ReplacementCode) && !string.IsNullOrEmpty(c.ReplacementCodeNamespace))
                    er = repository.HasCondition(c.ReplacementCode.ToLower(), c.ReplacementCodeNamespace.ToLower());




                if (c.Option == TreatmentAdmission.Add && ec.HasValue && ec.Value)
                    continue;

                if (c.Option == TreatmentAdmission.Remove && (!ec.HasValue || !ec.Value))
                    continue;

                if (c.Option == TreatmentAdmission.Replace &&
                    ((ec.HasValue && ec.Value) || (!er.HasValue || !er.Value)))
                    continue;

                if (c.NaiveModel != null)
                {
                   
                }

                if (c.DexiModel != null)
                {

                    options.AddRange(GetTreatmentDexiValues(repository, c, c.DexiModel));
                }

                if (c.RuleModel != null)
                {
                    
                }
            }

            return options;
        }




        public int GetTreatmentVariablesEvaluationCount(IConditionRepository repository)
        {
            int count = 0;
            foreach (var c in _treatmentClassifiers)
            {
                if (c.Code == null)
                    continue;

                if (c.DexiModel != null)
                {
                    var keys = repository.ToDict(c.DexiModel).Keys;
                    count += keys.Count;
                }
            }

            return count;
        }
    }
}