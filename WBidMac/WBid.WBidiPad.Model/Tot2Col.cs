using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[Serializable]
    public class Tot2Col
    {
        [XmlAttribute("Column1")]
        public int Column1 { get; set; }

        [XmlAttribute("Column2")]
        public int Column2 { get; set; }
    }
}

