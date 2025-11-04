using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.VacationDifference
{
    public partial class VacationDifferenceController : NSWindow
    {
        public VacationDifferenceController(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public VacationDifferenceController(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            
        }
    }
}
