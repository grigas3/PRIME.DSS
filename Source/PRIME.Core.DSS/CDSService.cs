using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models.CDS;
using PRIME.Core.DSS.Treatment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PRIME.Core.DSS
{
    /// <summary>
    /// Clinical Decision Support Service
    /// </summary>
    public class CDSService : ICDSService
    {
        private readonly TreatmentClassifierFactory _factory = new TreatmentClassifierFactory();
        private readonly ConditionClassifierFactory _cfactory = new ConditionClassifierFactory();
        private bool _initialized = false;

        private void Init()
        {
            _initialized = true;

            _factory.Load(Path.Combine(Environment.CurrentDirectory, "DATA\\Treatments"));
            _cfactory.Load(Path.Combine(Environment.CurrentDirectory, "DATA\\Conditions"));
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


            List<Card> cards = new List<Card>();
            foreach (var o in options)
            {
                StringBuilder details = new StringBuilder();


                details.Append("<div _ngcontent-c2 class='card-inner'>");
                details.Append(o.Description);
                details.Append("</div>");


                details.Append("<div  class='card-inner'>");
                details.Append(String.Format("The specific treatment option did have a probability of <b>{0} %</b>",
                    Math.Round(100 * (o.Probability))));
                details.Append("</div>");

                details.Append("<div>");
                foreach (var c in o.Factors)
                {
                    var sclass = "label cardlabel ";
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

                details.Append("</div>");
                var conds = (_cfactory.GetConditionOptions(o, repository));
                details.Append("<div><strong>Possible implications</strong><ul>");
                foreach (var c in conds)
                {
                    details.Append("<li>" + c.Name + " (" + Math.Floor(100 * c.Probability) + "%)</li>");
                }

                details.Append("</ul></div>");

                cards.Add(new Card()
                {
                    Source = new CardSource() {Name = "PRIME"},
                    Summary = o.Summary,
                    Detail = details.ToString(),
                    Indicator = "info",
                });
            }


            return cards;
        }
    }
}