using System;
using ObjCRuntime;
using WBid.WBidMac.Mac;

namespace WBidMac.WindowControllers.InitialPositionWindow
{
	public class InitialPositionWindow: NSWindow
    {
		

        public InitialPositionWindow(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public InitialPositionWindow(NSCoder coder) : base(coder)
        {
        }
        
        public override void AwakeFromNib()
        {
            //
            base.AwakeFromNib();
        }
        
    }
}

