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
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnLogin { get; set; }

		[Outlet]
		AppKit.NSButton BtnPasswordEye { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmployee { get; set; }

		[Outlet]
		AppKit.NSTextField txtHiddenPassword { get; set; }

		[Outlet]
		AppKit.NSSecureTextField txtPassword { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}

			if (BtnPasswordEye != null) {
				BtnPasswordEye.Dispose ();
				BtnPasswordEye = null;
			}

			if (txtEmployee != null) {
				txtEmployee.Dispose ();
				txtEmployee = null;
			}

			if (txtHiddenPassword != null) {
				txtHiddenPassword.Dispose ();
				txtHiddenPassword = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}
		}
	}

	[Register ("LoginView")]
	partial class LoginView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
