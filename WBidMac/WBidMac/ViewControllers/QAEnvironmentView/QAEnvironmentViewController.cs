
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidMac.Mac
{
	public partial class QAEnvironmentViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public QAEnvironmentViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QAEnvironmentViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public QAEnvironmentViewController () : base ("QAEnvironmentView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new QAEnvironmentView View {
			get {
				return (QAEnvironmentView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			if (GlobalSettings.buddyBidTest)
				btnQAOptions.SelectCellWithTag (1);
			else
				btnQAOptions.SelectCellWithTag (0);

			if (GlobalSettings.WBidINIContent.IsConnectedVofox)
				btnServerOptions.SelectCellWithTag (1);
			else
				btnServerOptions.SelectCellWithTag (0);
			if(GlobalSettings.IsSWAApiTest)
			{
				btnSwaAPI.State = NSCellStateValue.On;
			}
			else
			{
				btnSwaAPI.State = NSCellStateValue.Off;
			}
			segmentSwaEnvironment.SelectedSegment = (nint)GlobalSettings.SwaEnvironmentType;

			btnCancel.Activated += delegate {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
			};
			btnOK.Activated += delegate {

				if(btnQAOptions.SelectedTag==1)
					GlobalSettings.buddyBidTest = true;
				else
					GlobalSettings.buddyBidTest = false;

				if(btnServerOptions.SelectedTag==1)
					GlobalSettings.WBidINIContent.IsConnectedVofox=true;
				else
					GlobalSettings.WBidINIContent.IsConnectedVofox=false;
				if(btnSwaAPI.State==NSCellStateValue.On)
				{
					GlobalSettings.IsSWAApiTest = true;
				}
                else
				{
					GlobalSettings.IsSWAApiTest = false;
				}
                if (segmentSwaEnvironment.SelectedSegment == (nint)SwaEnviromentType.PROD)
                {
                    GlobalSettings.SwaEnvironmentType = (int)SwaEnviromentType.PROD;
                }
                if (segmentSwaEnvironment.SelectedSegment == (nint)SwaEnviromentType.DEV)
                {
                    GlobalSettings.SwaEnvironmentType = (int)SwaEnviromentType.DEV;
                }
                if (segmentSwaEnvironment.SelectedSegment == (nint)SwaEnviromentType.QA)
                {
                    GlobalSettings.SwaEnvironmentType = (int)SwaEnviromentType.QA;
                }

                //save the state of the INI File
                WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
				// save changes
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
			};
			//btnQAOptions.SelectedTag==0 getter
			//btnQAOptions.SelectCellWithTag(0) setter
		}
	}
}

