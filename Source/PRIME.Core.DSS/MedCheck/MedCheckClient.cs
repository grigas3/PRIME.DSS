using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.MedCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PRIME.Core.DSS.MedCheck
{
    /// <summary>
    /// Medication Check Service
    /// </summary>
    public class MedCheckService : IMedCheckService
    {
        #region Definitions
        private const string HostD2D = "https://rxnav.nlm.nih.gov/";
        private const string UrlD2D = "/REST/interaction/interaction.json?t=0&rxcui={0}";
        private const string UrlD = "/REST/rxcui?name={0}";
        private const string HostD2G = "http://www.dgidb.org";
        private const string UrlD2G = "/api/v2/interactions.json?t=0";
        #endregion

        #region Private Methods
        private readonly IDrugRepository _drugRepository;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="drugRepository"></param>
        public MedCheckService(IDrugRepository drugRepository)
        {
            _drugRepository = drugRepository;
        }

        #endregion



        /// <summary>
        /// Drug to gene interactions
        /// </summary>
        /// <param name="genes"></param>
        /// <param name="drugs"></param>
        /// <returns></returns>
        public async Task<List<Core.MedCheck.Gene.Interaction>> CheckD2GInteractions(string genes, string drugs)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(HostD2G);
            var url = UrlD2G + BuildGeneParameters(null, drugs);

            List<string> geneList = new List<string>();
            if (!string.IsNullOrEmpty(genes))
            {
                if (genes.Contains(","))
                {
                    geneList.AddRange(genes.Split(","));
                }
                else
                {
                    geneList.Add(genes);
                }
            }
            var response = await client.GetAsync(url);

            List<Core.MedCheck.Gene.Interaction> geneInteractions = new List<Core.MedCheck.Gene.Interaction>();
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                var ret = Core.MedCheck.Gene.GeneResponse
                    .FromJson(s);
                if (ret != null)
                {
                    foreach (var c in ret.MatchedTerms)
                    {
                        foreach (var i in c.Interactions)
                        {
                            i.DrugName = c.SearchTerm;
                            if (geneList.Contains(i.GeneName))
                                geneInteractions.Add(i);
                        }
                    }
                }
            }

            return geneInteractions;
        }

        /// <summary>
        /// Drug 2 drug interactions
        /// </summary>
        /// <param name="drugs"></param>
        /// <returns></returns>
        public async Task<List<Core.MedCheck.Drug.InteractionPair>> CheckD2DInteractions(string drugs)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(HostD2D);
            var interactionPairs = new List<Core.MedCheck.Drug.InteractionPair>();
            var drugList = new List<string>();
            var drugItems = new List<DrugItem>();
            if (drugs.Contains(","))
            {
                drugList.AddRange(drugs.Split(","));
            }
            else
            {
                drugList.Add(drugs);
            }

            //Find Medication RX
            foreach (var d in drugList)
            {
                var di = await _drugRepository.GetRXCUI(d);

                if (di == null)
                    throw new HttpRequestException("Drug is not present in our repository");
                drugItems.Add(di);
            }
            foreach (var c in drugItems)
            {
                
                var url = BuildDrugParameters(c.RXCUI); //Create URL
                var response = await client.GetAsync(url); 
                if (!response.IsSuccessStatusCode) continue;


                var s = await response.Content.ReadAsStringAsync();
                var ret = Core.MedCheck.Drug.DrugResponse
                    .FromJson(s); 


                foreach (var ii in ret.InteractionTypeGroup)
                {
                    foreach (var it in ii.InteractionType)
                    {
                        foreach (var ip in it.InteractionPair)
                        {
                            if (ip.InteractionConcept.All(
                                r => drugItems.Any(i => i.RXCUI == r.MinConceptItem.Rxcui)))
                            {
                                interactionPairs.Add(ip);
                            }
                        }
                    }
                }
            }


            return interactionPairs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genes"></param>
        /// <param name="drugs"></param>
        /// <returns></returns>
        public async Task<Core.MedCheck.Gene.CheckResponse> Check(string genes, string drugs)
        {
            var res = new Core.MedCheck.Gene.CheckResponse();
            res.Drugs = new List<Core.MedCheck.Gene.PRIMEInteraction>();
            res.Genes = new List<Core.MedCheck.Gene.PRIMEInteraction>();
            // Get D2D Interactions
            var dResponse = await CheckD2DInteractions(drugs);
            // Check D2G Interactions
            var gResponse = await CheckD2GInteractions(genes, drugs);


            foreach (var c in dResponse)
            {
                if (c.InteractionConcept.Count < 2) continue;

                var source = c.InteractionConcept[0].MinConceptItem.Name;
                var target = c.InteractionConcept[1].MinConceptItem.Name;
                if (res.Drugs.Any(e =>
                    (e.Source == source && e.Target == target) || (e.Source == target && e.Target == source)))
                    continue;
                
                res.Drugs.Add(new Core.MedCheck.Gene.PRIMEInteraction()
                {
                    Interaction = c.Description,
                    Source = source,
                    Target = target
                });
            }

            foreach (var c in gResponse)
            {
                if (c.InteractionTypes != null)
                {
                    res.Genes.Add(new Core.MedCheck.Gene.PRIMEInteraction()
                    {
                        Interaction = string.Join(",", c.InteractionTypes),
                        Source = c.DrugName,
                        Target = c.GeneName
                    });
                }
            }

            return res;
        }


        #region Private Methods

        private string BuildDrugParameters(long drug)
        {
            return String.Format(UrlD2D, drug);
        }

        private string BuildGeneParameters(string genes, string drugs)
        {
            StringBuilder str = new StringBuilder();

            if (!string.IsNullOrEmpty(genes))
            {
                str.Append("&genes=" + genes);
            }

            if (!string.IsNullOrEmpty(drugs))
            {
                str.Append("&drugs=" + drugs);
            }

            return str.ToString();
        }

        #endregion
    }
}