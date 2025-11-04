using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.LoginNew
{
    public partial class NewLoginWindow : NSWindow
    {
        public NewLoginWindow(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public NewLoginWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
