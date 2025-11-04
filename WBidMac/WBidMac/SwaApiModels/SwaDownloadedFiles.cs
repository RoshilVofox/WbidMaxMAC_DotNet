using System;
using ADT.Common.Models;

namespace WBidMac.SwaApiModels
{
	public class SwaDownloadedFiles
	{
		public SwaDownloadedFiles()
		{
		}
		public List<SWALine.LinesLine> swaLines { get; set; }
		public List<SWATrip.LinesPairing> swaTrips { get; set; }
		public List<SWASeniority.IFLineBaseAuctionSeniority> swaSeniority { get; set; }
		public List<SwaReserveAward.IFLineBaseAuctionReserveAward> swaReserveList { get; set; }
		public byte[] swaCoverLetter { get; set; }
	}
}

