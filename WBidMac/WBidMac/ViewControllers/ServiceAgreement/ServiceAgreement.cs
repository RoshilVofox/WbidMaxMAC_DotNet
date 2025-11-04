using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.ViewControllers.ServiceAgreement
{
	public partial class ServiceAgreement : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public ServiceAgreement (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ServiceAgreement (NSCoder coder) : base (coder)
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
