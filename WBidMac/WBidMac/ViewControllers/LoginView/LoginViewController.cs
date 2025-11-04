
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using System.Text.RegularExpressions;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class LoginViewController : AppKit.NSViewController
	{
		public bool isFromQueryWindow { get; set; }
		#region Constructors

		// Called when created from unmanaged code
		public LoginViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LoginViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public LoginViewController () : base ("LoginView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new LoginView View {
			get {
				return (LoginView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			txtEmployee.StringValue = (CommonClass.UserName != null) ? CommonClass.UserName : string.Empty;
			txtPassword.StringValue = (CommonClass.Password != null) ? CommonClass.Password : string.Empty;
			btnCancel.Activated += HandleBtnCancel;
			btnLogin.Activated += HandleBtnLogin;
            txtHiddenPassword.Hidden = true;


            BtnPasswordEye.Activated += BtnPasswordEye_Activated; ;
        }

        private void BtnPasswordEye_Activated(object sender, EventArgs e)
        {
            if (txtHiddenPassword.Hidden)
            {
                txtHiddenPassword.StringValue = txtPassword.StringValue;
                BtnPasswordEye.Image = NSImage.ImageNamed("closed-eyes.png");
            }
            else if (txtPassword.Hidden)
            {
                txtPassword.StringValue = txtHiddenPassword.StringValue;
                BtnPasswordEye.Image = NSImage.ImageNamed("show.png");
            }



            txtPassword.Hidden = !txtPassword.Hidden;

            txtHiddenPassword.Hidden = !txtHiddenPassword.Hidden;
            txtHiddenPassword.StringValue = txtPassword.StringValue;
        }

		void BidderCertify(NSNotification n)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver((NSString)"BidderCertify");
            DismissLogin();
            NSNotificationCenter.DefaultCenter.PostNotificationName("LoginSuccess", new NSString(btnLogin.Title));
        }
        void HandleBtnLogin (object sender, EventArgs e)
		{
			//if (string.IsNullOrEmpty (txtEmployee.StringValue.Trim ()) || !EmployeeNumberValidation (txtEmployee.StringValue))
				//return;
			//if (string.IsNullOrEmpty (txtPassword.StringValue.Trim ()))
				//return;
			try {
				if (!ValidateUI ())
					return;
				string uName = txtEmployee.StringValue.ToLower ();
				if (uName [0] != 'e' && uName [0] != 'x' && uName!="21221")
					uName = "e" + uName;
				else if(uName=="21221")
                    uName = "x" + uName;
                if (txtPassword.Hidden)
                    txtPassword.StringValue = txtHiddenPassword.StringValue;
                CommonClass.UserName = uName;
                CommonClass.Password = txtPassword.StringValue;
                GlobalSettings.IsBidderCertified = false;
                if (isFromQueryWindow)
				{

					if (uName.Substring(1) != GlobalSettings.SubmitBid.Bidder)
					{
						NSNotificationCenter.DefaultCenter.AddObserver((NSString)"BidderCertify", BidderCertify);
						BidderVerificationWindowController bidderVerify = new BidderVerificationWindowController();
						NSApplication.SharedApplication.BeginSheet(bidderVerify.Window, this.View.Window);

					}
					else
					{

						DismissLogin();
						NSNotificationCenter.DefaultCenter.PostNotificationName("LoginSuccess", new NSString(btnLogin.Title));
					}
				}
				else
				{
                    DismissLogin();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("LoginSuccess", new NSString(btnLogin.Title));
                }

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		private bool ValidateUI()
		{

			bool status = true;
			string message = string.Empty;


			if (string.IsNullOrEmpty (txtEmployee.StringValue.Trim ())) {

				txtEmployee.BecomeFirstResponder ();
				message = "Employee number required";
				status = false;
			}
			else if (!RegXHandler.EmployeeNumberValidation(txtEmployee.StringValue.Trim ())) {

				txtEmployee.BecomeFirstResponder ();
				message = "Invalid Employee number ";
				status = false;
			}
			else if (string.IsNullOrEmpty (txtPassword.StringValue.Trim ())) {
				txtPassword.BecomeFirstResponder ();
				message = "Password required";
				status = false;

			}





			if(!status)
			{

				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "WBidMax";
				alert.InformativeText = message;
				alert.AddButton("OK");
				alert.RunModal ();
			}

			return status;


		}


		void HandleBtnCancel (object sender, EventArgs e)
		{
			try {
				DismissLogin ();
				NSNotificationCenter.DefaultCenter.PostNotificationName ("LoginSuccess", new NSString (btnCancel.Title));
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void DismissLogin ()
		{
			txtEmployee.StringValue = string.Empty;
			txtPassword.StringValue = string.Empty;
			NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
			CommonClass.Panel.OrderOut (this);
		}

		public bool EmailValidation (string value)
		{
			string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

			if (!Regex.IsMatch (value, matchpattern)) {
				return false;
			}
			return true;
		}

		private bool EmployeeNumberValidation (string value)
		{
			if (!Regex.Match (value, "^[e,E,x,X,0-9][0-9]*$").Success) {
				return false;
			}
			return true;
		}

	}
}

