using System;
namespace WBid.WBidiPad.Model
{
    public class BidDataRequestDTO
    {
        public BidDataRequestDTO()
        {
        }
        public int EmpNum { get; set; }
        public string Base { get; set; }
        public string Position { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Round { get; set; }
    }
}
