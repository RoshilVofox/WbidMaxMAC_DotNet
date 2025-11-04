using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.ViewControllers.Tot2Col
{
	public partial class Tot2ColViewController : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public Tot2ColViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public Tot2ColViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}
