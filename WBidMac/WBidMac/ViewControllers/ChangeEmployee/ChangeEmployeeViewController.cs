
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;
using System.IO;
using System.Text.RegularExpressions;

namespace WBid.WBidMac.Mac
{
	public partial class ChangeEmployeeViewController : AppKit.NSViewController
	{
        NSPanel overlayPanel;
        #region Constructors
        public SubmitWindowController submitWindow;
        public BidEditorFAWindowController bidEditorFAWindow;
        public BidEditorPrepWindowController bidEditorPrepWindow;
        // Called when created from unmanaged code
        public ChangeEmployeeViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangeEmployeeViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ChangeEmployeeViewController () : base ("ChangeEmployeeView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ChangeEmployeeView View {
			get {
				return (ChangeEmployeeView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				
				txtEmployeeNo.StringValue = GlobalSettings.TemporaryEmployeeNumber ?? string.Empty;
				
				btnKeepOld.Activated += (object sender, EventArgs e) => {
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
				};
				btnChange.Activated += (object sender, EventArgs e) => {

                    //Validate Employee Number!

                    bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                    if (isConnectionAvailable)
                    {
                        if(GlobalSettings.IsSWAApiTest && GlobalSettings.CurrentBidDetails.Postion=="FA")
                        {
                            var wbidStateContent= GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            var buddyBids = wbidStateContent.BuddyBids;
                            if ((GlobalSettings.JobShare2 != "0" && GlobalSettings.JobShare2 != null) || ((buddyBids.Buddy1 != null && buddyBids.Buddy1 != "0") || (buddyBids.Buddy2!=null && buddyBids.Buddy2!="0")))
                            {
                                NSAlert buddyJobShareAlert = new NSAlert();
                                buddyJobShareAlert.Window.Title = "Change Employee Number";
                                buddyJobShareAlert.MessageText = "Clear Buddy/Job Share";
                                buddyJobShareAlert.InformativeText = "You need to clear Job Share and Buddy bidders before changing Employee Number";
                                buddyJobShareAlert.AlertStyle = NSAlertStyle.Informational;
                                buddyJobShareAlert.AddButton("OK");
                                buddyJobShareAlert.AddButton("Cancel");
                                var alertRes = buddyJobShareAlert.RunSheetModal(this.View.Window);
                                if(alertRes==(nint)NSAlertButtonReturn.First)
                                {
                                    wbidStateContent.BuddyBids.Buddy1 = "0";
                                    wbidStateContent.BuddyBids.Buddy2 = "0";
                                    CommonClass.MainController.UpdateSaveButton(true);
                                    GlobalSettings.JobShare1 = "0";
                                    GlobalSettings.JobShare2 = "0";
                                    GlobalSettings.TemporaryEmployeeBuddy = false;
                                    GlobalSettings.TemporaryEmployeeJobShare = false;
                                    GlobalSettings.isJobShareContingencyBid = false;
                                    if (submitWindow!=null)
                                    {
                                        submitWindow.ReloadControls();
                                        submitWindow = null;
                                    }
                                        
                                    if(bidEditorFAWindow!=null)
                                    {
                                        bidEditorFAWindow.LoadBuddyDetails();
                                        bidEditorFAWindow = null;
                                    }
                                       
                                    if (bidEditorPrepWindow != null)
                                    {
                                        bidEditorPrepWindow.UpdateUI();
                                        bidEditorPrepWindow = null;
                                    }
                                        
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        string empNo = Regex.Replace(txtEmployeeNo.StringValue, "[^0-9]+", string.Empty);
                        string url = "GetEmployeeDetails/" + empNo + "/4";
                        StreamReader dr = ServiceUtils.GetRestData(url);
                        WBidDataDownloadAuthorizationService.Model.UserInformation responseData = ServiceUtils.ConvertJSonToObject<WBidDataDownloadAuthorizationService.Model.UserInformation>(dr.ReadToEnd());
                        bool isValidUser = false;
                        string message = string.Empty;
                        if (responseData != null && responseData.EmpNum != 0 && responseData.EmpNum.ToString() != string.Empty)
                        {
                            //user account exist
                            if (responseData.IsYearlySubscribed || responseData.IsMonthlySubscribed || responseData.IsFree)
                            {
                                isValidUser = true;

                            }
                            else if (responseData.WBExpirationDate != null && responseData.WBExpirationDate >= DateTime.Now)
                            {
                                isValidUser = true;

                            }
                            else
                            {
                                isValidUser = false;
                                message = "User " + empNo + " does not have a valid subscription with Wbid account.  Go to www.wbidmax.com to subscribe.";

                            }
                        }
                        else
                        {
                            message = "User " + empNo + " does not have a valid account with Wbid.  Go to www.wbidmax.com to create the user account.";
                            //No user account
                        }
                        if (isValidUser)
                        {
                            GlobalSettings.TemporaryEmployeeName = responseData.FirstName + " " + responseData.LastName;
                            GlobalSettings.TemporaryEmployeeNumber = empNo;
                            if (GlobalSettings.TemporaryEmployeeNumber != GlobalSettings.WbidUserContent.UserInformation.EmpNo)
                            {
                                if (submitWindow != null)
                                {
                                    submitWindow.isChangeEmployee = true;
                                    submitWindow = null;
                                }

                                if (bidEditorFAWindow != null)
                                {
                                    bidEditorFAWindow.isChangeEmployee = true;
                                    bidEditorFAWindow = null;
                                }

                                if (bidEditorPrepWindow != null)
                                {
                                    bidEditorPrepWindow.isChangeEmployee = true;
                                    bidEditorPrepWindow = null;
                                }
                            }
                                
                            NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
                            CommonClass.Panel.OrderOut(this);
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                var alert = new NSAlert();
                                alert.AlertStyle = NSAlertStyle.Informational;
                                alert.MessageText = "WBidMax";
                                alert.InformativeText = message;
                                alert.AddButton("OK");

                                alert.RunModal();

                               
                            });

                        }
                    
                    }

				};
			} catch (Exception ex) 
			{
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}
}

