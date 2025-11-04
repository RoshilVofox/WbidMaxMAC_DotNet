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
	[Register ("LoginWindowController")]
	partial class LoginWindowController
	{
		[Outlet]
		AppKit.NSButton btnPasswordEye { get; set; }

		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSButton LoginButton { get; set; }

		[Outlet]
		AppKit.NSSecureTextField PasswordTextField { get; set; }

		[Outlet]
		AppKit.NSTextField UserIdTextField { get; set; }

		[Action ("CancelBtnTapped:")]
		partial void CancelBtnTapped (Foundation.NSObject sender);

		[Action ("LoginBtnTapped:")]
		partial void LoginBtnTapped (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}

			if (PasswordTextField != null) {
				PasswordTextField.Dispose ();
				PasswordTextField = null;
			}

			if (UserIdTextField != null) {
				UserIdTextField.Dispose ();
				UserIdTextField = null;
			}

			if (btnPasswordEye != null) {
				btnPasswordEye.Dispose ();
				btnPasswordEye = null;
			}
		}
	}

	[Register ("LoginWindow")]
	partial class LoginWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
