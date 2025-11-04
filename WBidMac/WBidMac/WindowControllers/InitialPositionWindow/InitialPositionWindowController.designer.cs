// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBidMac.WindowControllers.InitialPositionWindow
{
	[Register ("InitialPositionWindowController")]
	partial class InitialPositionWindowController
	{
		[Outlet]
		AppKit.NSButton btnSwaAPI { get; set; }

		[Action ("btnFATapped:")]
		partial void btnFATapped (Foundation.NSObject sender);

		[Action ("btnPilotTapped:")]
		partial void btnPilotTapped (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnSwaAPI != null) {
				btnSwaAPI.Dispose ();
				btnSwaAPI = null;
			}
		}
	}
}
