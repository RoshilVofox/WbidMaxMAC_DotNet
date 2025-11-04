using System;
namespace WBidMac.SwaApiModels
{
	public class SwaAwardModel
	{
		public SwaAwardModel()
		{
		}
        public List<SwaBidAward.IFLineBaseAuctionAward> LineAwards { get; set; }
        public List<SwaJobShareBidAward.IFLineBaseAuctionJobShareAward> JobShareAwards { get; set; }
        public List<SwaMrtBidAward.IFLineBaseAuctionMrtAward> MrtAwards { get; set; }
		public List<SwaReserveAward.IFLineBaseAuctionReserveAward> ReserveAwards { get; set; }
    }
}

