using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.UserAccoutDifference
{
    public partial class UserAccountDifference : NSWindow
    {
        public UserAccountDifference(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public UserAccountDifference(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        
    }
}
