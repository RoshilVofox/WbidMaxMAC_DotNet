using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    public class FALinePositionsResponseDTO
    {
        public FALinePositionsResponseDTO()
        {
        }
        public List<PositionDetails> lstLinePositions { get; set; }
    }
    public class PositionDetails
    {
        public int LineNum { get; set; }
        public string FaPositions { get; set; }
    }
}
