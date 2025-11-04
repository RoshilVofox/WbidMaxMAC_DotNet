using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.ViewControllers.ServiceAgreement
{
	public partial class ServiceAgreementController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public ServiceAgreementController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ServiceAgreementController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public ServiceAgreementController () : base ("ServiceAgreement", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
            //btnOk.Activated += BtnOk_Activated;
		}

        private void BtnOk_Activated(object sender, EventArgs e)
        {
            NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
            CommonClass.Panel.OrderOut(this);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnOk.Activated += BtnOk_Activated;
        }
        #endregion

        //strongly typed view accessor
        public new ServiceAgreement View {
			get {
				return (ServiceAgreement) base.View;
			}
		}
	}
}
