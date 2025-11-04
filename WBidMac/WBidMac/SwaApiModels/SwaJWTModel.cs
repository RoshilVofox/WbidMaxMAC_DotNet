using System;
using Newtonsoft.Json;

namespace WBid.WBidiPad.iOS.SwaApiModels
{
	public class SwaJWTModel
	{
		public SwaJWTModel()
		{
		}
		public string Token { get; set; }
		public DateTimeOffset tokenExpiry { get; set; }
    }
}

