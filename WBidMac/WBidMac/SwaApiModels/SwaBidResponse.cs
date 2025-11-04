using System;
using Newtonsoft.Json;

namespace WBidMac.SwaApiModels
{
	public class SwaBidResponse
	{
		public SwaBidResponse()
		{
		}
        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("packetId")]
        public string PacketId { get; set; }

        [JsonProperty("employeeId")]
        public string EmployeeId { get; set; }

        [JsonProperty("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonProperty("receivedAt")]
        public DateTime ReceivedAt { get; set; }

        [JsonProperty("buddyId1")]
        public string BuddyId1 { get; set; }

        [JsonProperty("buddyId2")]
        public string BuddyId2 { get; set; }

        [JsonProperty("jobShareId1")]
        public string JobShareId1 { get; set; }

        [JsonProperty("jobShareId2")]
        public string JobShareId2 { get; set; }

        [JsonProperty("bidSource")]
        public string BidSource { get; set; }

        [JsonProperty("confirmationNumber")]
        public string ConfirmationNumber { get; set; }

        [JsonProperty("jobShareContingent")]
        public bool JobShareContingent { get; set; }

        [JsonProperty("mrtContingent")]
        public bool MrtContingent { get; set; }

        [JsonProperty("bidChoices")]
        public List<BidChoice> BidChoices { get; set; }

        [JsonProperty("_links")]
        public BidLinks Links { get; set; }
    }
    public class BidChoice
    {
        [JsonProperty("choice")]
        public string Choice { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class BidLinks
    {
        [JsonProperty("self")]
        public SelfLink Self { get; set; }
    }

    public class SelfLink
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class EmbeddedBids
    {
        [JsonProperty("IFLineBaseAuctionBids")]
        public List<SwaBidResponse> IFLineBaseAuctionBids { get; set; }
    }

    public class RootObject
    {
        [JsonProperty("_embedded")]
        public EmbeddedBids Embedded { get; set; }

        [JsonProperty("_links")]
        public BidLinks Links { get; set; }
    }

}

