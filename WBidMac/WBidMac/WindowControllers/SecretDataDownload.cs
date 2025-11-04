using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers
{
    public partial class SecretDataDownload : NSWindow
    {
        public SecretDataDownload(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SecretDataDownload(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
