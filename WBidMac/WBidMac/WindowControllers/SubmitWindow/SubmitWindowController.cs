
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

#region NameSpace
//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
//using iOSPasswordStorage;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Linq;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;
using System.ServiceModel;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Text.RegularExpressions;
using ObjCRuntime;
using WBid.WBidMac.Mac.WindowControllers.JobShareWindow;
using WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow;
using Org.BouncyCastle.Asn1.Ocsp;
#endregion

namespace WBid.WBidMac.Mac
{
	public partial class SubmitWindowController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubmitWindowController (NativeHandle handle) : base (handle)
		{
			Initialize ();
		}
        public SubmitWindowController(IntPtr handle) : base(handle)
        {
            Initialize();
        }
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
		public SubmitWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public SubmitWindowController () : base ("SubmitWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new SubmitWindow Window {
			get {
				return (SubmitWindow)base.Window;
			}
		}

		public NSObject UInotif;

        WBidState wBidStateContent;
		NSObject _notif;
		NSObject ChangeEmployeeNotif;
		FAPositionChoiceWindowController fapositionChoiceWindow;
		public bool isChangeEmployee;
        static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
                 wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					this.Window.Close ();
					//				this.Window.OrderOut (this);
					//				NSApplication.SharedApplication.StopModal ();
				};
				txtSeniorityNo.Enabled = false;
				btnSubmitType.Activated += (object sender, EventArgs e) => {
					if (btnSubmitType.SelectedTag == 0)
						txtSeniorityNo.Enabled = false;
					else
						txtSeniorityNo.Enabled = true;
				};
				btnChangeEmp.Activated += (object sender, EventArgs e) => {
					try {
						var panel = new NSPanel ();
						var changeEmp = new ChangeEmployeeViewController ();
						changeEmp.submitWindow = this;
						CommonClass.Panel = panel;
						panel.SetContentSize (new CoreGraphics.CGSize (400, 180));
						panel.ContentView = changeEmp.View;
						NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					} catch (Exception ex) {
						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					}
				};
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					//				this.Window.OrderOut (this);
					//				NSApplication.SharedApplication.StopModal ();
				};
				btnChangeAvoidance.Activated += (object sender, EventArgs e) => {
					try {
						if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
							var panel = new NSPanel ();
							var changeAvoid = new ChangeAvoidanceViewController ();
							CommonClass.Panel = panel;
							panel.SetContentSize (new CoreGraphics.CGSize (400, 270));
							panel.ContentView = changeAvoid.View;
							NSApplication.SharedApplication.BeginSheet (panel, this.Window);
						} else if (GlobalSettings.CurrentBidDetails.Postion == "FA") {

							if (!GlobalSettings.IsSWAApiTest)
							{

								var alert = new NSAlert();
								alert.AlertStyle = NSAlertStyle.Informational;
								alert.MessageText = "WBidMax";
								alert.InformativeText = "If you are Buddy Bidding, you need to verify that your Buddy Bidders are on your Buddy List, and they know you are Buddy Bidding with them!";
								alert.AddButton("I have verified");
								alert.AddButton("I am NOT buddy bidding");
								alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
								{
                                    NSApplication.SharedApplication.EndSheet(alert.Window);
                                    alert.Window.Close();
									

									var panel = new NSPanel();
									var changeBud = new ChangeBuddyViewController();
									CommonClass.Panel = panel;
									panel.SetContentSize(new CoreGraphics.CGSize(400, 190));
									panel.ContentView = changeBud.View;
									NSApplication.SharedApplication.BeginSheet(panel, this.Window);
								};
								alert.Buttons[1].Activated += (object sender1, EventArgs ex) =>
								{
									wBidStateContent.BuddyBids.Buddy1 = "0";
									wBidStateContent.BuddyBids.Buddy2 = "0";
									GlobalSettings.TemporaryEmployeeBuddy = false;
									CommonClass.MainController.UpdateSaveButton(true);
									ReloadControls();
                                    NSApplication.SharedApplication.EndSheet(alert.Window);
                                    alert.Window.Close();
									

								};
								alert.RunSheetModal(this.Window);
							}
							else
							{
                                if (WBidHelper.IsTokenExpired())
                                {
                                    
                                    InvokeOnMainThread(() =>
                                    {

                                        var alert = new NSAlert();
                                        alert.AlertStyle = NSAlertStyle.Informational;
                                        alert.MessageText = "WBidMax";
                                        alert.InformativeText = "Your session has expired,Please login again.";
                                        alert.AddButton("OK");
                                        var response = alert.RunSheetModal(this.Window);
                                        if (response == (nint)(NSAlertButtonReturn.First)) // OK button 
                                        {
                                            SwaLoginWindowController swaloginWindow = new SwaLoginWindowController();
                                            this.Window.AddChildWindow(swaloginWindow.Window, NSWindowOrderingMode.Above);
                                            swaloginWindow.Window.MakeKeyAndOrderFront(this);
                                            NSApplication.SharedApplication.RunModalForWindow(swaloginWindow.Window);
                                        }


                                    });
                                }
								else
								{
                                    var alert = new NSAlert();
                                    alert.AlertStyle = NSAlertStyle.Informational;
                                    alert.MessageText = "WBidMax";
                                    alert.InformativeText = "If you are Buddy Bidding, you need to verify that your Buddy Bidders are on your Buddy List, and they know you are Buddy Bidding with them!";
                                    alert.AddButton("I have verified");
                                    alert.AddButton("I am NOT buddy bidding");
                                    alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                                    {
                                        NSApplication.SharedApplication.EndSheet(alert.Window);
                                        alert.Window.Close();
                                        var panel = new NSPanel();
                                        var changeBud = new ChangeBuddyViewController();
										if (isChangeEmployee)
											changeBud.isChangeEmployee = true;
                                        CommonClass.Panel = panel;
                                        panel.SetContentSize(new CoreGraphics.CGSize(400, 190));
                                        panel.ContentView = changeBud.View;
                                        NSApplication.SharedApplication.BeginSheet(panel, this.Window);
                                    };
                                    alert.Buttons[1].Activated += (object sender1, EventArgs ex) =>
                                    {
                                        wBidStateContent.BuddyBids.Buddy1 = "0";
                                        wBidStateContent.BuddyBids.Buddy2 = "0";
										GlobalSettings.TemporaryEmployeeBuddy = false;
                                        CommonClass.MainController.UpdateSaveButton(true);
                                        ReloadControls();
                                        NSApplication.SharedApplication.EndSheet(alert.Window);
                                        alert.Window.Close();
                                        

                                    };
                                    alert.RunSheetModal(this.Window);
                                }
                            }

                        }
					} catch (Exception ex) {
						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					}
				};
				btnSubmit.Activated += (object sender, EventArgs e) => {
				
					try {

						if (!ValidateUI())
						{
							return;
						}
						if(GlobalSettings.CurrentBidDetails.Postion=="FA" && GlobalSettings.CurrentBidDetails.Round == "M")
						{
                            wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            BuddyBids buddyBids = wBidStateContent.BuddyBids;
                            if (btnRadioJobShare.SelectedCell.Tag == 0 && btnAvoidance.SelectedCell.Tag==0 && (GlobalSettings.JobShare2 != "0" && (buddyBids.Buddy1 != "0" || buddyBids.Buddy2 != "0")))
							{
								var alert = new NSAlert();
								alert.MessageText = "Clear Job Share or Buddy Bid\n";
								alert.InformativeText = "You can only bid using either Job Share or Buddy Bid. Please set one of them to \"Do not Use\" submitting your bid.";
								alert.AddButton("OK");
								alert.RunSheetModal(this.Window);
								return;

                            }

                        }

						GlobalSettings.SubmitBid = SetSubmitDetails ();
						UInotif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"UIRefresh",UIRefresh);
						if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M") {
							if(fapositionChoiceWindow==null)
							{
								fapositionChoiceWindow = new FAPositionChoiceWindowController();
                                this.Window.AddChildWindow(fapositionChoiceWindow.Window, NSWindowOrderingMode.Above);
                                fapositionChoiceWindow.Window.MakeKeyAndOrderFront(this);
                            }
							else
							{
								fapositionChoiceWindow.Window.MakeKeyAndOrderFront(this);
							}
							//var faChoice = new FAPositionChoiceWindowController ();
							
							//NSApplication.SharedApplication.RunModalForWindow (faChoice.Window);
							

                        } else {
							var query = new QueryWindowController ();
							this.Window.AddChildWindow (query.Window, NSWindowOrderingMode.Above);
							NSApplication.SharedApplication.RunModalForWindow (query.Window);
							//this.Window.AddChildWindow(query.Window,NSWindowOrderingMode.Above); 
                          //  query.Window.MakeKeyAndOrderFront(this);
						}
					} catch (Exception ex) {
						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					}
				};

				btnChangeJobShare.Activated += (object sender, EventArgs e) =>
				{
					try
					{
						if(WBidHelper.IsTokenExpired())
						{
                            InvokeOnMainThread(() =>
                            {

                                var alert = new NSAlert();
                                alert.AlertStyle = NSAlertStyle.Informational;
                                alert.MessageText = "WBidMax";
                                alert.InformativeText = "Your session has expired,Please login again.";
                                alert.AddButton("OK");
                                var response = alert.RunModal();
                                if (response == (nint)(NSAlertButtonReturn.First)) // OK button 
                                {
                                    SwaLoginWindowController swaloginWindow = new SwaLoginWindowController();
                                    this.Window.AddChildWindow(swaloginWindow.Window, NSWindowOrderingMode.Above);
                                    swaloginWindow.Window.MakeKeyAndOrderFront(this);
                                    NSApplication.SharedApplication.RunModalForWindow(swaloginWindow.Window);
                                }


                            });
                        }
						else
						{
                            InvokeOnMainThread(() =>
                            {

                                var alert = new NSAlert();
                                alert.AlertStyle = NSAlertStyle.Informational;
                                alert.MessageText = "WBidMax";
                                alert.InformativeText = "If you are Job Share Bidding, you need to verify that your Job Share Bidders are on your Buddy List, and they know you are Job Share Bidding with them!";
                                alert.AddButton("I have verified");
								alert.AddButton("I am NOT Job Share Bidding");
                                var response = alert.RunSheetModal(this.Window);
                                if (response == (nint)(NSAlertButtonReturn.First)) 
                                {
                                    _notif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"JobShareAdded", AfterJobShareAdded);
                                    JobShareWindowController jobShare = new JobShareWindowController();
									if (isChangeEmployee)
										jobShare.isChangeEmployee = true;
                                    this.Window.AddChildWindow(jobShare.Window, NSWindowOrderingMode.Above);
                                    NSApplication.SharedApplication.RunModalForWindow(jobShare.Window);
                                }
								else
								{
                                    GlobalSettings.JobShare1 = "0";
                                    GlobalSettings.JobShare2 = "0";
									GlobalSettings.TemporaryEmployeeJobShare = false;
									GlobalSettings.isJobShareContingencyBid = false;
                                    CommonClass.MainController.UpdateSaveButton(true);
                                    ReloadControls();
                                }
								

                            });

                            
                        }
                       
						
					}
					catch(Exception ex)
					{
                        CommonClass.AppDelegate.ErrorLog(ex);
                        CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                    }
				};
				
				this.Window.DidEndSheet += (object sender, EventArgs e) => {
					this.Window.Title = "Submit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
					if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
						ReloadControls ();
					} else if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
						ReloadControls ();
					}
				};
				
			    ValidateBuddyJobShare();
				
				SetUpView ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

       

        public void UIRefresh(NSNotification ns)
		{
			RefreshUI();
		}

		private void ValidateBuddyJobShare()
		{
            if (GlobalSettings.TemporaryEmployeeBuddy || GlobalSettings.TemporaryEmployeeJobShare)
            {
                var submitEmp = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
				var alert = new NSAlert();
				alert.InformativeText = $"You have Buddy/Job Share bidders set for Employee [{GlobalSettings.TemporaryEmployeeNumber}].It will be cleared before opening submission for [{submitEmp}]";
				alert.MessageText = "WBidMax";
				alert.RunModal();
                GlobalSettings.JobShare1 = "0";
                GlobalSettings.JobShare2 = "0";
                GlobalSettings.isJobShareContingencyBid = false;
                GlobalSettings.TemporaryEmployeeBuddy = false;
                GlobalSettings.TemporaryEmployeeJobShare = false;
                wBidStateContent.BuddyBids.Buddy1 = "0";
                wBidStateContent.BuddyBids.Buddy2 = "0";
                CommonClass.MainController.UpdateSaveButton(true);
                ReloadControls();
            }
        }

		private void RefreshUI()
		{
            string str = string.Empty;
            string jobshareStr = string.Empty;
            if (GlobalSettings.CurrentBidDetails.Round != "S")
            {
                str = GetBuddyBid();
                jobshareStr = GetJobShareBid();
            }
            txtJobShare.StringValue = "Job Share Bids: " + jobshareStr;
            lblAvoidanceBid.StringValue = "Buddy Bids: " + str;
            btnAvoidance.Enabled = true;
            if (str == string.Empty)
                btnAvoidance.SelectCellWithTag(1);
            else
                btnAvoidance.SelectCellWithTag(0);
            if (jobshareStr == string.Empty)
            {
                btnRadioJobShare.SelectCellWithTag(1);
            }
            else
            {
                btnRadioJobShare.SelectCellWithTag(0);
            }
        }

		public void AfterJobShareAdded(NSNotification ns)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(_notif);
			ReloadControls();
		}

		private bool ValidateUI()
		{

			bool status = true;
			string message = string.Empty;


			if (string.IsNullOrEmpty(txtSeniorityNo.StringValue.Trim()))
			{

				txtSeniorityNo.BecomeFirstResponder();
				message = "Seniority number required";
				status = false;
			}
			else if (!RegXHandler.NumberValidation(txtSeniorityNo.StringValue.Trim()))
			{

				txtSeniorityNo.BecomeFirstResponder();
				message = "Invalid Seniority number ";
				status = false;
			}

			else if (txtSeniorityNo.StringValue.Trim().Length > 7)
			{

				txtSeniorityNo.BecomeFirstResponder();
				message = "Invalid Seniority number ";
				status = false;
			}
			//else if (string.IsNullOrEmpty(txtSeniorityNo.StringValue.Trim()))
			//{
			//	txtSeniorityNo.BecomeFirstResponder();
			//	message = "Password required";
			//	status = false;

			//}





			if (!status)
			{

				var alert = new NSAlert();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "WBidMax";
				alert.InformativeText = message;
				alert.AddButton("OK");
				alert.RunModal();
			}

			return status;


		}
		public void SetUpView ()
		{
			//btnSubmitType = new NSMatrix();
			btnSubmitType.SelectCellWithTag (0);
			txtSeniorityNo.Enabled = false;
			GlobalSettings.TemporaryEmployeeNumber = (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) ? GlobalSettings.WbidUserContent.UserInformation.EmpNo : string.Empty;
			txtSeniorityNo.StringValue = GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber.ToString ();
            //pkce-remove
    //        if (GlobalSettings.ModifiedEmployeeNumber != null)
    //        {
    //            GlobalSettings.TemporaryEmployeeNumber = GlobalSettings.ModifiedEmployeeNumber;
				//GlobalSettings.TemporaryEmployeeName = "Test Name";
    //        }
            //pkce-remove
            this.Window.Title = "Submit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
			ReloadControls ();

		}

		public void ReloadControls ()
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "CP") {
				lblAvoidanceBid.StringValue = "No Avoidance Bids";
				btnAvoidance.Enabled = false;
				btnAvoidance.SelectCellWithTag (1);
				btnChangeAvoidance.Title = "Change Avoidance Bids";
				btnChangeAvoidance.Enabled = false;
				btnRadioJobShare.Hidden = true;
				btnChangeJobShare.Hidden = true;
				txtJobShare.Hidden = true;

			} else if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
				string str = GetAvoidanceBid ();
				lblAvoidanceBid.StringValue = "Avoidance Bids: " + str;
				btnAvoidance.Enabled = true;
				if (str == string.Empty)
					btnAvoidance.SelectCellWithTag (1);
				else
					btnAvoidance.SelectCellWithTag (0);
				btnChangeAvoidance.Title = "Change Avoidance Bids";
				btnChangeAvoidance.Enabled = true;
                btnRadioJobShare.Hidden = true;
                btnChangeJobShare.Hidden = true;
                txtJobShare.Hidden = true;
            } else if (GlobalSettings.CurrentBidDetails.Postion == "FA") 
			{
				if(!GlobalSettings.IsSWAApiTest)
				{
                    btnRadioJobShare.Hidden = true;
                    btnChangeJobShare.Hidden = true;
                    txtJobShare.Hidden = true;
                }
				else
				{
                    btnRadioJobShare.Hidden = false;
                    btnChangeJobShare.Hidden = false;
                    txtJobShare.Hidden = false;
                }
				string str = string.Empty;
				string jobshareStr = string.Empty;
				if (GlobalSettings.CurrentBidDetails.Round != "S") {
					 str = GetBuddyBid ();
					jobshareStr = GetJobShareBid();
				}
				txtJobShare.StringValue = "Job Share Bids: " + jobshareStr;
				lblAvoidanceBid.StringValue = "Buddy Bids: " + str;
				btnAvoidance.Enabled = true;
				if (str == string.Empty)
					btnAvoidance.SelectCellWithTag (1);
				else
					btnAvoidance.SelectCellWithTag (0);
				if(jobshareStr==string.Empty)
				{
					btnRadioJobShare.SelectCellWithTag(1);
				}
				else
				{
					btnRadioJobShare.SelectCellWithTag(0);
				}
				btnAvoidance.CellWithTag (0).Title = "Use These Buddy Bids";
				btnChangeAvoidance.Title = "Change Buddy Bids";
				btnChangeAvoidance.Enabled = true;

				if (GlobalSettings.CurrentBidDetails.Round == "S") {
					btnAvoidance.SelectCellWithTag (1);
					btnAvoidance.Enabled = false;
					btnChangeAvoidance.Enabled = false;
					btnRadioJobShare.Enabled = false;
					btnChangeJobShare.Enabled = false;
					btnRadioJobShare.SelectCellWithTag(1);
				}
			}
		}

		/// <summary>
		/// Get Avoidance Bid string
		/// </summary>
		/// <returns></returns>
		private string GetAvoidanceBid()
		{
			string avoidanceBidsStr = string.Empty;
			AvoidanceBids avoidancebids = GlobalSettings.WBidINIContent.AvoidanceBids;
			avoidanceBidsStr += (avoidancebids.Avoidance1 != "0") ? avoidancebids.Avoidance1.ToString() + "," : "";
			avoidanceBidsStr += (avoidancebids.Avoidance2 != "0") ? avoidancebids.Avoidance2.ToString() + "," : "";
			avoidanceBidsStr += (avoidancebids.Avoidance3 != "0") ? avoidancebids.Avoidance3.ToString() : "";
			avoidanceBidsStr = avoidanceBidsStr.TrimEnd(',');
			return avoidanceBidsStr;
		}

        public string GetJobShareBid()
        {
            string jobsharestr = string.Empty;
            string jobShare1 = GlobalSettings.JobShare1;
            string jobShare2 = GlobalSettings.JobShare2;
            //BuddyBids buddyBids = new BuddyBids() { Buddy1 = GlobalSettings.SubmitBid.Buddy1, Buddy2 = GlobalSettings.SubmitBid.Buddy2 };
            //disable buddy bid
            jobsharestr += (jobShare1 != null && jobShare1 != "0") ? jobShare1 + "," : "";
            jobsharestr += (jobShare2 != null && jobShare2 != "0") ? jobShare2 + "," : "";
            jobsharestr = jobsharestr.TrimEnd(',');
            return jobsharestr;
        }

        /// <summary>
        /// Get Buddy Bid string
        /// </summary>
        /// <returns></returns>
        public string GetBuddyBid()
		{
			string buddyBidStr = string.Empty;
            wBidStateContent=GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			BuddyBids buddyBids = wBidStateContent.BuddyBids;
			//BuddyBids buddyBids = new BuddyBids() { Buddy1 = GlobalSettings.SubmitBid.Buddy1, Buddy2 = GlobalSettings.SubmitBid.Buddy2 };
            //disable buddy bid
            buddyBidStr += (buddyBids.Buddy1!=null && buddyBids.Buddy1 != "0") ? buddyBids.Buddy1.ToString() + "," : "";
			buddyBidStr += (buddyBids.Buddy2!=null && buddyBids.Buddy2 != "0") ? buddyBids.Buddy2.ToString() + "," : "";
			buddyBidStr = buddyBidStr.TrimEnd(',');
			return buddyBidStr;
		}

		/// <summary>
		/// Set submit Details
		/// </summary>
		/// <returns></returns>
		private SubmitBid SetSubmitDetails()
		{
			WBid.WBidiPad.Model.BidDetails bidDetails = GlobalSettings.CurrentBidDetails;
			SubmitBid submitBid = new SubmitBid();
			//set the properties required to POST the webrequest to SWA server.
			submitBid.Base = bidDetails.Domicile;
			submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
			submitBid.BidRound = (bidDetails.Round == "S") ? "Round 2" : "Round 1";
			submitBid.PacketId = GenaratePacketId(bidDetails);
			submitBid.Seat = bidDetails.Postion;
			submitBid.IsSubmitAllChoices = (btnSubmitType.SelectedTag == 0);
			//int aa = sgNoAvoidanceSubmissionType.SelectedSegment;
			if (bidDetails.Postion == "FO" && (btnAvoidance.SelectedTag == 0))
			{
				AvoidanceBids avoidanceBids = GlobalSettings.WBidINIContent.AvoidanceBids;
				submitBid.Pilot1 = (avoidanceBids.Avoidance1 == "0") ? null : avoidanceBids.Avoidance1;
				submitBid.Pilot2 = (avoidanceBids.Avoidance2 == "0") ? null : avoidanceBids.Avoidance2;
				submitBid.Pilot3 = (avoidanceBids.Avoidance3 == "0") ? null : avoidanceBids.Avoidance3;
			}

			if (bidDetails.Postion == "FA" && (btnAvoidance.SelectedTag == 0))
			{
                BuddyBids buddyBids = wBidStateContent.BuddyBids;
				//comment out this to disable buddy bid
				submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
				submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;

			}

            if (bidDetails.Postion == "FA" && (btnRadioJobShare.SelectedTag == 0))
            {
				submitBid.JobShare1 = GlobalSettings.JobShare1;
				submitBid.JobShare2 = GlobalSettings.JobShare2;

            }


            int seniorityNumber = int.Parse((txtSeniorityNo.StringValue.Trim() == string.Empty) ? "0" : txtSeniorityNo.StringValue.Trim());
			if (submitBid.IsSubmitAllChoices)
			{
				submitBid.SeniorityNumber = GlobalSettings.Lines.Count();
				submitBid.TotalBidChoices = GlobalSettings.Lines.Count();
				//submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Select(x => x.LineNum));
			}
			else
			{
				submitBid.SeniorityNumber = seniorityNumber;
				submitBid.TotalBidChoices = seniorityNumber;
				//submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Take(seniorityNumber).Select(x => x.LineNum));

			}

			if (bidDetails.Postion == "FO" || bidDetails.Postion == "CP" ||(bidDetails.Postion=="FA" && bidDetails.Round=="S" ))
			{
				submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Take(submitBid.TotalBidChoices).Select(x => x.LineNum));
			}

			return submitBid;
		}

		/// <summary>
		/// Genarate Packet Id for Submit Bid Format:
		// Format: BASE || Year || Month || bid-round-number eg(Value=BWI2001032)
		/// </summary>
		/// <param name="bidDetails"></param>
		/// <returns></returns>
		private string GenaratePacketId(WBid.WBidiPad.Model.BidDetails bidDetails)
		{
			string packetid = string.Empty;
			packetid = bidDetails.Domicile + bidDetails.Year + bidDetails.Month.ToString("d2");

			//Set-round-numbers:
			//1 - F/A monthly bids
			//2 - F/A supplemental bids
			//3 - reserved
			//4 - Pilot monthly bids
			//5 - Pilot supplemental bids

			if (bidDetails.Round == "M" && bidDetails.Postion == "FA")
			{
				packetid += "1";
			}
			else if (bidDetails.Round == "S" && bidDetails.Postion == "FA")
			{
				packetid += "2";
			}
			else if (bidDetails.Round == "M" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
			{
				packetid += "4";
			}
			else if (bidDetails.Round == "S" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
			{
				packetid += "5";
			}
			return packetid;
		}

	}
}

