using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo
{
    public partial class UserUpdateInfo : NSWindow
    {
       
        public UserUpdateInfo(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public UserUpdateInfo(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            
        }
       
    }
}
