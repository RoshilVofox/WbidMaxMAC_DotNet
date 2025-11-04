using System;
using Org.BouncyCastle.Bcpg;
using System.Text.Json.Serialization;

namespace WBidMac.SwaApiModels
{
	public class SwaApiSubmitBid
	{
		public SwaApiSubmitBid()
		{

		}      
        public string Department { get; set; }
        public PacketId PacketId { get; set; }
        public string EmployeeId { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Dictionary<string, List<string>> BuddyBids { get; set; }
        public string JobShare1 { get; set; }
        public string JobShare2 { get; set; }
        public string BidSource { get; set; }
        public bool MrtContingent { get; set; }
        public bool JobShareContingent { get; set; }
        public List<string> BidChoices { get; set; }
    }
    public class PacketId
    {
        public string Base { get; set; }
        public int Year { get; set; }
        public string SchedulePeriod { get; set; }
        public string RoundType { get; set; }
    }

}

