using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac
{
	public partial class BidderVerificationWindow : NSWindow
	{
		public BidderVerificationWindow (NativeHandle handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public BidderVerificationWindow (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
