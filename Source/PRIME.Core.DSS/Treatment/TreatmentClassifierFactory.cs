using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;

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


            foreach (var c in files)
            {
                var json = File.ReadAllText(c);
                _treatmentClassifiers.Add(JsonConvert.DeserializeObject<TreatmentClassifier>(json));
            }
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
                var ec = repository.HasCondition(c.Code.ToLower(),null);
                bool? er = new bool?();
                if (!string.IsNullOrEmpty(c.ReplacementCode))
                    er = repository.HasCondition(c.ReplacementCode.ToLower(), null);


                if (c.Option == TreatmentAdmission.Add && ec.HasValue && ec.Value)
                    continue;

                if (c.Option == TreatmentAdmission.Remove && (!ec.HasValue || !ec.Value))
                    continue;

                if (c.Option == TreatmentAdmission.Replace &&
                    ((ec.HasValue && ec.Value) || (!er.HasValue || !er.Value)))
                    continue;

                List<TreatmentFactor> factors = new List<TreatmentFactor>();
                List<NaiveVarInstance> vInputs = new List<NaiveVarInstance>();

                foreach (var v in c.Variables)
                {
                    var b = repository.HasCondition(v.Code.ToLower(), null);

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

                        vInputs.Add(new NaiveVarInstance() {Code = v.Code.ToLower(), Name = v.Name, Present = b.Value});
                    }
                    else
                    {
                        factors.Add(new TreatmentFactor() {Effect = TreatmentFactorEffect.Undetermined, Name = v.Name});
                    }
                }


                var p = c.GetOutput(vInputs);

                if (p > 0.1)
                {
                    options.Add(new TreatmentOption()
                    {
                        Name = c.Name,
                        ReplacementCode = c.ReplacementCode,
                        Code = c.Code.ToLower(),
                        Option = c.Option,
                        Probability = p,
                        Factors = factors,
                        Summary = c.Summary
                    });
                }
            }

            return options;
        }
    }
}