using System;
using ADT.Common.Utility;
using Newtonsoft.Json;

namespace WBidMac.SwaApiModels
{
	public class SwaMrtBidAward
	{
		public SwaMrtBidAward()
		{
		}

        public class Embedded
        {
            [JsonProperty("IFLineBaseAuctionMrtAwards")]
            public List<IFLineBaseAuctionMrtAward> IFLineBaseAuctionMrtAwards { get; set; }
        }

        public class IFLineBaseAuctionMrtAward
        {
            [JsonProperty("employeeId")]
            public string EmployeeId { get; set; }

            [JsonProperty("legalName")]
            public string LegalName { get; set; }

            [JsonProperty("baseSeniority")]
            public int BaseSeniority { get; set; }

            [JsonProperty("contingent")]
            public bool Contingent { get; set; }

            [JsonProperty("_links")]
            public Links Links { get; set; }
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

        // Reuse Links, Page, Self, Next, etc.
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

        public class Self
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("templated")]
            public bool templated { get; set; }
        }

        public class First
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class Next
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class Last
        {
            [JsonProperty("href")]
            public string href { get; set; }
        }

        public class Search
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("templated")]
            public bool templated { get; set; }
        }

        public class Page
        {
            [JsonProperty("size")]
            public int Size { get; set; }

            [JsonProperty("totalElements")]
            public int TotalElements { get; set; }

            [JsonProperty("totalPages")]
            public int TotalPages { get; set; }

            [JsonProperty("number")]
            public int Number { get; set; }
        }
    }
}

