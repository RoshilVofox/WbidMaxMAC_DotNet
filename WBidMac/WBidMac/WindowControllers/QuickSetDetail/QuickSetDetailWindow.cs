
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace WBid.WBidMac.Mac
{
	public partial class QuickSetDetailWindow : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public QuickSetDetailWindow (NativeHandle handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QuickSetDetailWindow (NSCoder coder) : base (coder)
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

