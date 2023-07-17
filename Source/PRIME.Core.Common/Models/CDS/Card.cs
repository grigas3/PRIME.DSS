using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Models.CDS
{
//    {
//  "cards": [
//    {
//      "summary": "Premier Inc:  Based on new culture information and facility antibiogram, the following anti-infectives has the highest likelihood (% susceptibility) of effectively treating the infections ",
//      "indicator": "info",
//      "detail": "<Table><tr><td><strong>Anti-infective Medications </strong><span style=\"color: #00ccff;\">(% Susceptable)</span></td></tr><TR><TD>Ampicillin-Sulbactam( 93%) </TD></TR><TR><TD>Levofloxacin( 88%) </TD></TR><TR><TD>Cefazolin(79%) </TD></TR><TR><TD>SMX-TMP(76%) </TD></TR><TR><TD>Gentamicin(73%) </TD></TR></Table>",
//      "source": {
//        "name": "Premier Inc",
//        "url": null
//      },
//      "links": [
//        {
//          "label": "Launch Premier TheraDoc for more details",
//          "url": "http://premiercdsapps-env.us-east-1.elasticbeanstalk.com/CDS_Hooks/antibiogramlink?hookinstance=9ae4bc55-2ccd-469b-b5cc-33e23d983998",
//          "type": "absolute"
//       }
//      ]
//    }
//  ]
//}



    /// <summary>
    /// Card Model
    /// https://www.hl7.org/fhir/clinicalreasoning-cds-on-fhir.html
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Summary
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }
        /// <summary>
        /// Indicator
        /// </summary>
        [JsonProperty("indicator")]
        public string Indicator { get; set; }
        /// <summary>
        /// Detail
        /// </summary>
        [JsonProperty("detail")]
        public string Detail { get; set; }
        /// <summary>
        /// Source
        /// </summary>
        [JsonProperty("source")]
        public CardSource Source { get; set; }
        /// <summary>
        /// Links
        /// </summary>
        [JsonProperty("links")]
        public List<CardLink> Links { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// Weight Or Score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Related Suggestions
        /// </summary>
        public List<Suggestion> Suggestions { get; set; }

       
    }


    public class SuggestionAction
    {
        [JsonProperty("type")]
        public string ActionType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

    }
    /// <summary>
    /// Card Suggestion
    /// </summary>
    public class Suggestion
    {
        [JsonProperty("links")]
        public string Label { get; set; }
        [JsonProperty("actions")]
        public List<SuggestionAction> Actions { get; set; }
    }
}
