// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.JobShareWindow
{
	[Register ("JobShareWindowController")]
	partial class JobShareWindowController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnCheckBox { get; set; }

		[Outlet]
		AppKit.NSButton btnClearFields { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddyDomicile { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddyName { get; set; }

		[Outlet]
		AppKit.NSTextField txtJobShare1 { get; set; }

		[Outlet]
		AppKit.NSTextField txtJobShare2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnCheckBox != null) {
				btnCheckBox.Dispose ();
				btnCheckBox = null;
			}

			if (btnClearFields != null) {
				btnClearFields.Dispose ();
				btnClearFields = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (txtJobShare1 != null) {
				txtJobShare1.Dispose ();
				txtJobShare1 = null;
			}

			if (txtJobShare2 != null) {
				txtJobShare2.Dispose ();
				txtJobShare2 = null;
			}

			if (lblBuddyName != null) {
				lblBuddyName.Dispose ();
				lblBuddyName = null;
			}

			if (lblBuddyDomicile != null) {
				lblBuddyDomicile.Dispose ();
				lblBuddyDomicile = null;
			}
		}
	}
}
