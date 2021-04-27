using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRIME.Core.Common.Models;

namespace PRIME.Core.DSS.MedCheck
{
    /// <summary>
    /// Drug ID Service
    /// Get the RXCUI of a service based on its name from
    /// https://rxnav.nlm.nih.gov/
    /// </summary>
    public class DrugService : IDrugRepository
    {
        private const string hostD2D = "https://rxnav.nlm.nih.gov/";
        private const string urlD = "/REST/rxcui?name={0}";

        /// <summary>
        /// Get RX CUI of medication
        /// </summary>
        /// <param name="drug"></param>
        /// <returns></returns>
        public async Task<DrugItem> GetRXCUI(string drug)
        {
            var id = await GetDrugId(drug);

            return new DrugItem() {RXCUI = id, Name = drug};
        }
        /// <summary>
        /// Get Drug id from the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<long> GetDrugId(string name)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(hostD2D);
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
            var response = await client.GetAsync(String.Format(urlD, name));
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                var ret = JsonConvert.DeserializeObject<Common.Models.RX.RxResponse>(s);

                if (ret != null && ret.IdGroup != null && ret.IdGroup.RxnormId != null)
                {
                    return ret.IdGroup.RxnormId.FirstOrDefault();
                }
                else
                {
                    throw new KeyNotFoundException("Getting drug id");
                }
            }

            else
            {
                throw new HttpRequestException("Getting drug id");
            }
        }
    }
}