using System;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow
{
	public partial class SwaLoginWindow : NSWindow
	{
		public SwaLoginWindow (NativeHandle handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public SwaLoginWindow (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
