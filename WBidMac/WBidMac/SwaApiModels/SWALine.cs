using ADT.Common.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBidMac.SwaApiModels;

namespace ADT.Common.Models
{
    public  class SWALine
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class BidPeriod
        {
            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("department")]
            public string department { get; set; }

            [JsonProperty("start")]
            public string start { get; set; }

            [JsonProperty("end")]
            public string end { get; set; }
        }

        public class BidRoundId
        {
            [JsonProperty("year")]
            public string year { get; set; }

            [JsonProperty("schedulePeriod")]
            public string schedulePeriod { get; set; }

            [JsonProperty("roundType")]
            public string roundType { get; set; }

            [JsonProperty("value")]
            public string value { get; set; }
        }

        public class Embedded
        {
            [JsonProperty("LinesLines")]
            public List<LinesLine> LinesLines { get; set; }
        }

        public class First
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class Last
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class LineKey
        {
            [JsonProperty("lineNumber")]
            public int lineNumber { get; set; }

            [JsonProperty("department")]
            public string department { get; set; }

            [JsonProperty("seatPositions")]
            public List<string> seatPositions { get; set; }
        }

        public class LinePairing
        {
            [JsonProperty("pairingKey")]
            public PairingKey pairingKey { get; set; }

            [JsonProperty("reserveInfo")]
            public object reserveInfo { get; set; }
        }

        public class LinesLine
        {
            [JsonProperty("bidRoundId")]
            public BidRoundId bidRoundId { get; set; }

            [JsonProperty("publishedVersion")]
            public int publishedVersion { get; set; }

            [JsonProperty("lineKey")]
            public LineKey lineKey { get; set; }

            [JsonProperty("bidPeriod")]
            public BidPeriod bidPeriod { get; set; }

            [JsonProperty("lineType")]
            public string lineType { get; set; }

            [JsonProperty("lineBaseIATAStationCode")]
            public string lineBaseIATAStationCode { get; set; }

            [JsonProperty("lineDaytimeCategory")]
            public string lineDaytimeCategory { get; set; }

            [JsonProperty("fleet")]
            public string fleet { get; set; }

            [JsonProperty("reportDateTimeUTC")]
            public DateTime reportDateTimeUTC { get; set; }

            [JsonProperty("releaseDateTimeUTC")]
            public DateTime releaseDateTimeUTC { get; set; }

            [JsonProperty("totalBlockTime")]
            public double totalBlockTime { get; set; }

            [JsonProperty("totalLineCredit")]
            public double totalLineCredit { get; set; }

            [JsonProperty("redeye")]
            public bool redeye { get; set; }

            [JsonProperty("missionType")]
            public string missionType { get; set; }

            [JsonProperty("international")]
            public bool international { get; set; }

            [JsonProperty("language")]
            public string language { get; set; }

            [JsonProperty("linePairings")]
            public List<LinePairing> linePairings { get; set; }

            [JsonProperty("_links")]
            public Links _links { get; set; }
        }

        public class Links
        {
            [JsonProperty("self")]
            public Self self { get; set; }

            [JsonProperty("up")]
            public Up up { get; set; }

            [JsonProperty("first")]
            public First first { get; set; }

            [JsonProperty("next")]
            public Next next { get; set; }

            [JsonProperty("last")]
            public Last last { get; set; }

            [JsonProperty("search")]
            public Search search { get; set; }
        }

        public class Next
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class Page
        {
            [JsonProperty("size")]
            public int size { get; set; }

            [JsonProperty("totalElements")]
            public int totalElements { get; set; }

            [JsonProperty("totalPages")]
            public int totalPages { get; set; }

            [JsonProperty("number")]
            public int number { get; set; }
        }

        public class PairingKey
        {
            [JsonProperty("pairingNumber")]
            public string pairingNumber { get; set; }

            [JsonProperty("department")]
            public string department { get; set; }

            [JsonProperty("pairingDate")]
            public string pairingDate { get; set; }
        }

        public class Root: IPaginatedResponse
        {
            [JsonProperty("_embedded")]
            public Embedded _embedded { get; set; }

            [JsonProperty("_links")]
            public Links _links { get; set; }

            [JsonProperty("page")]
            public Page page { get; set; }

            public bool HasNextPage => _links?.next != null;

            public object Page => page;
        }

        public class Search
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("templated")]
            public bool templated { get; set; }
        }

        public class Self
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class Up
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }





    }
}
