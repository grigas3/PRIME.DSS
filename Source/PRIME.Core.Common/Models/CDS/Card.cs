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


        public class CardSource
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class CardLink
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("url")]

        public string Url { get; set; }
        [JsonProperty("type")]
        public string UrlType { get; set; }

    }
    public class Card
    {

        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("indicator")]
        public string Indicator { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("source")]
        public CardSource Source { get; set; }

        [JsonProperty("links")]
        public List<CardLink> Links { get; set; }

    }
}
