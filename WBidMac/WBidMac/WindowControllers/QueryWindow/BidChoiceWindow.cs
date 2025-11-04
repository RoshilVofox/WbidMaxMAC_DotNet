using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac
{
	public partial class BidChoiceWindow : NSWindow
	{
		public BidChoiceWindow(NativeHandle handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public BidChoiceWindow(NSCoder coder) : base(coder)
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}
	}
}
