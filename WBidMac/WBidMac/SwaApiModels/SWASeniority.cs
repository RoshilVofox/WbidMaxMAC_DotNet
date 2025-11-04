using ADT.Common.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADT.Common.Models
{
    public  class SWASeniority
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Embedded
        {
            [JsonProperty("IFLineBaseAuctionSeniorities")]
            public List<IFLineBaseAuctionSeniority> IFLineBaseAuctionSeniorities { get; set; }
        }

        public class First
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class IFLineBaseAuctionSeniority
        {
            [JsonProperty("department")]
            public string department { get; set; }

            [JsonProperty("employeeId")]
            public string employeeId { get; set; }

            [JsonProperty("legalName")]
            public string legalName { get; set; }

            [JsonProperty("base")]
            public string @base { get; set; }

            [JsonProperty("baseSeniority")]
            public int baseSeniority { get; set; }

            [JsonProperty("companySeniority")]
            public int companySeniority { get; set; }

            [JsonProperty("lodoQualification")]
            public LodoQualification lodoQualification { get; set; }

            [JsonProperty("vacation")]
            public List<VacationPeriod> Vacation { get; set; }

            [JsonProperty("_links")]
            public Links _links { get; set; }
        }

        public class Last
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class Links
        {
            [JsonProperty("self")]
            public Self self { get; set; }

            [JsonProperty("first")]
            public First first { get; set; }

            [JsonProperty("next")]
            public Next next { get; set; }

            [JsonProperty("last")]
            public Last last { get; set; }

            [JsonProperty("search")]
            public Search search { get; set; }
        }

        public class LodoQualification
        {
            [JsonProperty("language")]
            public string language { get; set; }

            [JsonProperty("lodoHireIndicator")]
            public bool lodoHireIndicator { get; set; }

            [JsonProperty("lodoObligationStartDate")]
            public object lodoObligationStartDate { get; set; }

            [JsonProperty("lodoObligationEndDate")]
            public object lodoObligationEndDate { get; set; }
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

        public class Root : IPaginatedResponse
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

            [JsonProperty("templated")]
            public bool templated { get; set; }
        }

        public class VacationPeriod
        {
            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }
        }

       

    }
}
