using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    
    public class BuddyBids
    {
        [XmlAttribute("Buddy1")]
        //[XmlIgnore]
        public string Buddy1 { get; set; }

        [XmlAttribute("Buddy2")]
        //[XmlIgnore]
        public string Buddy2 { get; set; }
    }
}
