using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.DSS.Treatment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRIME.Core.Context.Entities;
using PRIME.Core.DSS.Fuzzy;

namespace PRIME.Core.DSS
{
    /// <summary>
    /// Clinical Decision Support Service
    /// </summary>
    public class CDSService : ICDSService
    {
        private readonly TreatmentClassifierFactory _factory = new TreatmentClassifierFactory();
        private readonly ConditionClassifierFactory _cfactory = new ConditionClassifierFactory();
        private readonly List<Tuple<string,string>> variables = new List<Tuple<string, string>>();
        private readonly IRepositoryService _service;
        private bool _initialized = false;


        public CDSService(IRepositoryService service)
        {
            _service = service;

        }

        private void Init()
        {
            _initialized = true;
            _factory.Load(Path.Combine(Environment.CurrentDirectory, "DATA\\Treatments"));
            _cfactory.Load(Path.Combine(Environment.CurrentDirectory, "DATA\\Conditions"));
        }
        private async Task Init(int clientId, bool loadClientModels)
        {
            _initialized = true;

            var models=await _service.GetAsync<DSSModel>(e => e.CDSClientId == clientId);

            //var conditions = await _service.GetAsync<DSSModel>(e => e.CDSClientId == clientId);
            var conditions = models.Where(e => !e.TreatmentSuggestion);
            var treatments = models.Where(e => e.TreatmentSuggestion);
            //Add Conditions
            foreach (var c in conditions )
            {
                var conditionClassifier=ConditionClassifier.FromJson(c.Config);
                _cfactory.Add(conditionClassifier);

                foreach(var v in conditionClassifier.Variables)
                    if(!(string.IsNullOrEmpty(v.CodeNameSpace)&&
                    variables.Any(e=>e.Item1==v.Code))
                        || variables.Any(e => e.Item1 == v.Code&&e.Item2==v.CodeNameSpace))
                {
                    variables.Add(Tuple.Create<string,string>(v.Code,string.IsNullOrEmpty(v.CodeNameSpace)?"PRIME":v.CodeNameSpace));
                }
            }

            if (!loadClientModels)
                return;
            //Add Treatments
            foreach (var c in treatments)
            {
                
                var treatmentClassifier = TreatmentClassifier.FromJson(c.Config);
                _factory.Add(treatmentClassifier);
                foreach (var v in treatmentClassifier.Variables)
                    if (!(string.IsNullOrEmpty(v.CodeNameSpace) &&
                          variables.Any(e => e.Item1 == v.Code))
                        || variables.Any(e => e.Item1 == v.Code && e.Item2 == v.CodeNameSpace))
                    {
                        variables.Add(Tuple.Create<string, string>(v.Code, string.IsNullOrEmpty(v.CodeNameSpace) ? "PRIME" : v.CodeNameSpace));
                    }

            }

            //_factory.Load(Path.Combine(Environment.CurrentDirectory, "DATA\\Treatments"));
            //_cfactory.Load(Path.Combine(Environment.CurrentDirectory, "DATA\\Conditions"));
        }


        /// <summary>
        /// Get CDSS Cards
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public List<Card> GetCards(IConditionRepository repository)
        {
            if (!_initialized)
                Init();

            var options = _factory.GetTreatmentOptions(repository);
            var initialConditions=(_cfactory.GetConditions(repository).ToList());
            var currentCodes = options.Select(e => e.Code).ToList();
            List<Card> cards = new List<Card>();
            foreach (var o in options)
            {

                if (cards.Any(e => e.Code == o.Code))
                    continue;

                StringBuilder details = new StringBuilder();


                details.Append("<div _ngcontent-c2 class='cdscard-inner'>");
                details.Append(o.Description);
                details.Append("</div>");


                details.Append("<div  class='cdscard-inner'>");
                details.Append(String.Format("The specific treatment option did have a probability of <b>{0} %</b>",
                    Math.Round(100 * (o.Probability))));
                details.Append("</div>");

                details.Append("<div>");
                foreach (var c in o.Factors)
                {
                    var sclass = "label cdscardlabel ";
                    if (c.Effect == TreatmentFactorEffect.Negative)
                    {
                        sclass += "label-danger";
                    }
                    else if (c.Effect == TreatmentFactorEffect.Positive)
                    {
                        sclass += "label-success";
                    }
                    else
                    {
                        sclass += "label-warning";
                    }

                    details.Append(String.Format("<span  class='{0}'>{1}</span><span> </span>", sclass, c.Name));
                }
                repository.AddCondition(o.Code, "PRIME");
                var additionalTreatments = _factory.GetTreatmentOptions(repository);


                additionalTreatments = additionalTreatments.Where(e => !currentCodes.Contains(e.Code)).ToList();
                details.Append("</div>");

                if (additionalTreatments.Count() > 0)
                {
                    details.Append("<div><strong>Additional Treatments to consider</strong><ul>");
                    foreach (var c in additionalTreatments)
                    {
                        details.Append("<li>" + c.Name +"</li>");
                    }

                    details.Append("</ul></div>");
                }


                var conds = (_cfactory.GetConditionOptions(o, repository));
                conds = conds.Where(e => !initialConditions.Any(p=>(p.Code==e.Code)&&e.Value<=p.Value)).ToList();

                if (conds.Count > 0)
                {
                    details.Append("<div><strong>Possible implications</strong><ul>");
                    foreach (var c in conds)
                    {
                        details.Append("<li>" + c.Name +"</li>");
                    }

                    details.Append("</ul></div>");
                }

                repository.RemoveCondition(o.Code, "PRIME");


                if (o.Preconditions.Count > 0)
                {

                    details.Append("<div><strong>Preconditions </strong><ul>");
                    foreach (var c in o.Preconditions)
                    {
                        if (c.Presence.HasValue)
                        {
                            if (c.Presence.Value)

                                details.Append("<li>" + c.Code + " <i class='bi-bookmark-check text-success'></i></li>");
                            else
                                details.Append("<li>" + c.Code + " <i class='bi-bookmark-x text-danger'></i></li>");
                        }
                        else{
                            details.Append("<li>" + c.Code + " <i class='bi-bookmark text-warning'></i></li>");
                        }

                }
                    details.Append("</ul></div>");
                }
            
                     
                



                cards.Add(new Card()
                {
                    Source = new CardSource() {Name = "PRIME"},
                    Summary = o.Summary,
                    Detail = details.ToString(),
                    Indicator = "info",
                    Code=o.Code
                    //AdditionalTreatments=additionalConditions.Where(e=>e.TreatmentSuggestion).Select(e=>e.Name).ToList()
                    //PossibleImplications = additionalConditions.Where(e => e.TreatmentSuggestion).Select(e => e.Name).ToList()
                });
            }


            return cards;
        }




        /// <summary>
        /// Get CDSS Cards
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public void EvalConditions(IConditionRepository repository)
        {
            if (!_initialized)
                Init();
            var conditions = _cfactory.GetConditions(repository);
            
            foreach(var c in conditions)
                repository.AddCondition(c.Code,"PRIME",c.Value);

            //If model variables are not identified in the conditions then ad
            //foreach (var c in variables)
            //{
            //    var codeNameSpace = c.Item2;
            //    if (codeNameSpace == null)
            //    {
            //        codeNameSpace = "PRIME";
            //    }

            //    if (!conditions.Any(e => e.Code == c.Item1&&e.CodeNameSpace== codeNameSpace))
            //    {

            //        repository.AddCondition(c.Item1, codeNameSpace);
            //    }

            //}

            
        }

        public async Task<IEnumerable<Card>> GetCardsAsync(IConditionRepository repo, DSSModel model)
        {
            var classifier = TreatmentClassifier.FromJson(model.Config);
            _initialized = true;
            _factory.Add(classifier);
            if(model.CDSClientId.HasValue)
            await Init(model.CDSClientId.Value, false);
            var cards=GetCards(repo);
            return cards;
        }

        public async Task<IEnumerable<Card>> GetCardsAsync(IConditionRepository repo, int clientId)
        {
            _initialized = true;
           
            await Init(clientId,true);

            EvalConditions(repo);
            var cards = GetCards(repo);
            return cards;
        }

        public async Task<IEnumerable<ICondition>> GetConditionsAsync(IConditionRepository repo, int clientId,bool userOnly=false)
        {
            _initialized = true;

            await Init(clientId, true);
            
            EvalConditions(repo);
            List<ICondition> conditions=new List<ICondition>();
            foreach (var c in variables)
            {

                var codeNamespace = c.Item2;
                //if (userOnly && (codeNamespace == "PRIME"))
                //    continue;
                if (string.IsNullOrEmpty(codeNamespace))
                {
                    codeNamespace = "PRIME";
                }

               
               var r= repo.GetConditionRes(c.Item1, codeNamespace);
               conditions.Add(r);

            }

            return conditions;



        }

    }
}