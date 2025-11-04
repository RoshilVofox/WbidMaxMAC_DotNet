using System;
using System.Text.Json.Serialization;

namespace WBidMac.SwaApiModels
{
	public class SwaBuddyList
	{
		public SwaBuddyList()
		{
		}
        [JsonPropertyName("buddyIds")]
        public List<string> BuddyIds { get; set; } = new();

        [JsonPropertyName("_links")]
        public Links Links { get; set; }
    }
    public class Links
    {
        [JsonPropertyName("self")]
        public SelfLinks Self { get; set; }
    }

    public class SelfLinks
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("templated")]
        public bool Templated { get; set; }
    }
}

