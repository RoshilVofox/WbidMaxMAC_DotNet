using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.CustomAlert
{
    public partial class CustomAlertBox : NSWindow
    {
        public CustomAlertBox(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public CustomAlertBox(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            //
            base.AwakeFromNib();
        }
    }
}
