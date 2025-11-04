// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("BidderVerificationWindowController")]
	partial class BidderVerificationWindowController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnCheckbox { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmitButton { get; set; }

		[Outlet]
		AppKit.NSTextField lblCertify { get; set; }

		[Outlet]
		AppKit.NSTextField lblWarningText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCheckbox != null) {
				btnCheckbox.Dispose ();
				btnCheckbox = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnSubmitButton != null) {
				btnSubmitButton.Dispose ();
				btnSubmitButton = null;
			}

			if (lblCertify != null) {
				lblCertify.Dispose ();
				lblCertify = null;
			}

			if (lblWarningText != null) {
				lblWarningText.Dispose ();
				lblWarningText = null;
			}
		}
	}
}
