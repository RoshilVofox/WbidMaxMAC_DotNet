
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac
{
	public partial class ContactUsWindow : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public ContactUsWindow (NativeHandle handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ContactUsWindow (NSCoder coder) : base (coder)
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

