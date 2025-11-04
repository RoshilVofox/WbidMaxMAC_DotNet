using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.CommuteDifference
{
    public partial class CommutDifferenceController : NSWindow
    {
        public CommutDifferenceController(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public CommutDifferenceController(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            

        }
    }
}
