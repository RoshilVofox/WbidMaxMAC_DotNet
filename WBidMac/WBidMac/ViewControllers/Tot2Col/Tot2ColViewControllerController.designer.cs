// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.ViewControllers.Tot2Col
{
	[Register ("Tot2ColViewControllerController")]
	partial class Tot2ColViewControllerController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnCancellTapped { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnColum1 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnColumn2 { get; set; }

		[Outlet]
		AppKit.NSButton btnOk { get; set; }

		[Outlet]
		AppKit.NSButton btnOKTapped { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnColum1 != null) {
				btnColum1.Dispose ();
				btnColum1 = null;
			}

			if (btnCancellTapped != null) {
				btnCancellTapped.Dispose ();
				btnCancellTapped = null;
			}

			if (btnOKTapped != null) {
				btnOKTapped.Dispose ();
				btnOKTapped = null;
			}

			if (btnColumn2 != null) {
				btnColumn2.Dispose ();
				btnColumn2 = null;
			}

			if (btnOk != null) {
				btnOk.Dispose ();
				btnOk = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
		}
	}
}
