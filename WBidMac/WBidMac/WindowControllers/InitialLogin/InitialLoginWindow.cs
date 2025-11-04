using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.initialLogin
{
    public partial class InitialLoginWindow : AppKit.NSWindow
    {
        public InitialLoginWindow(NativeHandle handle) : base(handle)
        {
           
        }

        [Export("initWithCoder:")]
        public InitialLoginWindow(NSCoder coder) : base(coder)
        {
          
        }
       
        
    }
}