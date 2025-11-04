
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;
//using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
//using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using WBid.WBidiPad.SharedLibrary.SWA;
//using iOSPasswordStorage;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;
using WBid.WBidiPad.Core;
using System.Threading.Tasks;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Core.Enum;
using System.Collections.Specialized;
using WBid.WBidMac.Mac.WindowControllers.CustomAlert;
using WBid.WBidMac.Mac.ViewControllers.CustomAlertView;
using CoreGraphics;
using ObjCRuntime;
using WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow;
using WBidMac.SwaApiModels;
using System.Globalization;
using ADT.Common.Utility;
using System.Text;

namespace WBid.WBidMac.Mac
{
	public partial class QueryWindowController : AppKit.NSWindowController
	{


		WBidDataDwonloadAuthServiceClient client;
		Guid token;
		private string _sessionCredentials = string.Empty;
		#region Constructors
		NSObject notif;
		NSPanel overlayPanel;
		OverlayViewController overlay;
		BidChoiceWindowController bidChoiceWindow;

		// Called when created from unmanaged code
		public QueryWindowController (NativeHandle handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QueryWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public QueryWindowController () : base ("QueryWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new QueryWindow Window {
			get {
				return (QueryWindow)base.Window;
			}
		}

		static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
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
				this.Window.Title = "Query Bid";
				lblAvoidanceInfo.Hidden = true;
				string employeeNumber = GlobalSettings.TemporaryEmployeeNumber;
				lblTitle.StringValue = "Submitting " + GlobalSettings.SubmitBid.TotalBidChoices + " Bid Choices for";
				txtEmployee.StringValue = employeeNumber;
				
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
				client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
				client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
				
				if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
					string str = GetBuddyBid ();
					if (str == string.Empty) {
						lblChoices.StringValue = "No Buddy Bids";
						txtChoices.StringValue = string.Empty;
                        if (GlobalSettings.IsSWAApiTest)
                        {

                            if (GlobalSettings.SubmitBid.JobShare2 != "0" && GlobalSettings.SubmitBid.JobShare2 != string.Empty && GlobalSettings.SubmitBid.JobShare2 != null)
                            {
                                txtChoices.StringValue = $"{GlobalSettings.SubmitBid.JobShare1},{GlobalSettings.SubmitBid.JobShare2}";
                                lblChoices.StringValue = "Job Share Bid Choices";
                            }
                        }
                    } else {
						lblChoices.StringValue = "Buddy Bid Choices";
						txtChoices.StringValue = str;
					}
				} else if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
					string str = GetAvoidanceBid ();
					if (str == string.Empty) {
						lblChoices.StringValue = "No Avoidance Bids";
						txtChoices.StringValue = string.Empty;
					} else {
						lblChoices.StringValue = "Avoidance Bid Choices";
						txtChoices.StringValue = str;
					}
				} else {
					lblChoices.StringValue = "No Avoidance Bids";
					txtChoices.StringValue = string.Empty;
					lblAvoidanceInfo.Hidden = false;
				}
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					//				this.Window.OrderOut (this);
					//				NSApplication.SharedApplication.StopModal ();
				};
				btnSubmit.Activated += (object sender, EventArgs e) => {
					GlobalSettings.IsSWAApi = (GlobalSettings.CurrentBidDetails.Postion=="FA" && GlobalSettings.IsSWAApiTest);//pkce
					if (!GlobalSettings.IsSWAApi)
					{
						LoginViewController login = new LoginViewController();
						login.isFromQueryWindow = true;
						var panel = new NSPanel();
						CommonClass.Panel = panel;
						panel.SetContentSize(new CoreGraphics.CGSize(450, 250));
						panel.ContentView = login.View;
						NSApplication.SharedApplication.BeginSheet(panel, this.Window);
						notif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LoginSuccess", HandleLoginSuccess);
					}
					else //PKCE changes
					{
                        notif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"PKCE_loginSuccess", HandlePKCELoginSuccess);
                        SwaLoginWindowController swaloginWindow = new SwaLoginWindowController();
                        this.Window.AddChildWindow(swaloginWindow.Window, NSWindowOrderingMode.Above);
                        swaloginWindow.Window.MakeKeyAndOrderFront(this);
                        NSApplication.SharedApplication.RunModalForWindow(swaloginWindow.Window);

                    }
				
				};
				btnShowBidChoices.Activated += (object sender, EventArgs e) =>
				{
					var bidChoice = new BidChoiceWindowController();
					this.Window.AddChildWindow(bidChoice.Window, NSWindowOrderingMode.Above);
					NSApplication.SharedApplication.RunModalForWindow(bidChoice.Window);
					//bidChoice.ShowWindow(null);
					//bidChoice.Window.MakeKeyAndOrderFront(null);


				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
        void HandleLoginSuccess(NSNotification obj)
        {
            try
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
                if (obj.Object.ToString() == "Login")
                {
                    // Start Submission Process!
                    overlayPanel = new NSPanel();
                    overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                    overlay = new OverlayViewController();
                    overlay.OverlayText = "Authenticating..";
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

                    new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                        this.AuthenticationCheck();
                    })).Start();

                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        void HandlePKCELoginSuccess(NSNotification obj)
		{
			try {
				NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
				if (obj.Object.ToString () == "Success") {
					// Start Submission Process!
					if (GlobalSettings.SubmitBid.Bidder == CommonClass.UserName.Replace("e", "").Replace("E", ""))
					{

                        overlayPanel = new NSPanel();
                        overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                        overlay = new OverlayViewController();
                        overlay.OverlayText = "Authenticating..";
                        overlayPanel.ContentView = overlay.View;
                        NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

                        new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                        {
                            this.AuthenticationCheck();
                        })).Start();

                    }
					else
					{
						GlobalSettings.IsBidderCertified = false;
                        notif=NSNotificationCenter.DefaultCenter.AddObserver((NSString)"BidderCertify", BidderCertify);
                        BidderVerificationWindowController bidderVerify = new BidderVerificationWindowController();
						NSApplication.SharedApplication.BeginSheet(bidderVerify.Window, this.Window);
						//this.Window.AddChildWindow(bidderVerify.Window, NSWindowOrderingMode.Above);
						//bidderVerify.Window.MakeKeyAndOrderFront(this);
						//NSApplication.SharedApplication.RunModalForWindow(bidderVerify.Window);
                    }
				
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

        void BidderCertify(NSNotification n)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
            overlayPanel = new NSPanel();
            overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
            overlay = new OverlayViewController();
            overlay.OverlayText = "Authenticating..";
            overlayPanel.ContentView = overlay.View;
            NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                this.AuthenticationCheck();
            })).Start();
        }


        [Export("AuthenticationCheck")]
		private void AuthenticationCheck()
		{
			string userName = CommonClass.UserName;//KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
			string password = CommonClass.Password;//KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);

			//checking  the internet connection available
			//==================================================================================================================
            if (Reachability.CheckVPSAvailable())
			{
				//  NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
				//checking CWA credential
				//==================================================================================================================
				if(GlobalSettings.IsSWAApi)
				{
                    InvokeOnMainThread(() =>
                    {
                        overlay.UpdateText("Authorization Checking...");
                    });

                    ClientRequestModel clientRequestModel = new ClientRequestModel();
                    clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
                    clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
                    clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
                    clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
                    clientRequestModel.Platform = CommonClass.Platform;
                    clientRequestModel.RequestType = (int)RequestTypes.SubmitBid;
                    token = new Guid();
                    clientRequestModel.Token = token;
                    clientRequestModel.Version = CommonClass.AppVersion;
                    clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(userName, @"\d+").Value);
                    client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);
                }
				else
				{
                    Authentication authentication = new Authentication();
                    string authResult = authentication.CheckCredential(userName, password);
                    if (authResult.Contains("ERROR: "))
                    {
                        WBidLogEvent objlogs = new WBidLogEvent();
                        objlogs.LogBadPasswordUsage(userName, false, authResult);

                        InvokeOnMainThread(() =>
                        {
                            //var alert = new NSAlert ();
                            //alert.AlertStyle = NSAlertStyle.Warning;
                            //alert.MessageText = "WBidMax";
                            //alert.InformativeText = "Invalid Username or Password";
                            //alert.RunModal();
                            var panel = new NSPanel();
                            var customAlert = new CustomAlertViewController();
                            customAlert.AlertType = "InvalidCredential";
                            customAlert.objQueryWindow = this;
                            CommonClass.Panel = panel;
                            panel.SetContentSize(new CGSize(430, 350));
                            panel.ContentView = customAlert.View;
                            panel.WillClose += (sender, e) =>
                            {
                                NSApplication.SharedApplication.EndSheet(panel);
                                NSApplication.SharedApplication.StopModal();
                            };
                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                            this.Window.Close();
                            this.Window.OrderOut(this);
                            //NSApplication.SharedApplication.BeginSheet(panel, this.Window);
                            NSApplication.SharedApplication.RunModalForWindow(panel);


                            //							alert.AddButton("OK");
                            //							((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
                            //								NSApplication.SharedApplication.EndSheet(overlayPanel);
                            //								this.Window.Close();
                            //								this.Window.OrderOut(this);
                            //								NSApplication.SharedApplication.StopModal();
                            //							};
                            //							alert.BeginSheet (this.overlayPanel);

                            //							UIAlertView alert = new UIAlertView("WBidMax", "Invalid Username or Password", null, "OK", null);
                            //							alert.Show();
                            //							loadingOverlay.RemoveFromSuperview();

                        });
                    }
                    else if (authResult.Contains("Exception"))
                    {
                        InvokeOnMainThread(() =>
                        {
                            WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                            obgWBidLogEvent.LogTimeoutBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber, authResult);

                            var alert = new NSAlert();
                            alert.AlertStyle = NSAlertStyle.Warning;
                            alert.MessageText = "Warning";
                            //alert.InformativeText = "The company server is down.  They have been notified.  We don?t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.";
                            alert.InformativeText = "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a Server Timeout.\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\nChange your internet connection.You can also try to use your cell phone as a hotspot for your internet connection \n\nFinally, send us an email if you are continuously having trouble.";

                            alert.RunModal();
                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                            this.Window.Close();
                            this.Window.OrderOut(this);
                            NSApplication.SharedApplication.StopModal();

                            //							alert.AddButton("OK");
                            //							((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
                            //								NSApplication.SharedApplication.EndSheet(this.overlayPanel);
                            //								this.Window.Close();
                            //								this.Window.OrderOut(this);
                            //								NSApplication.SharedApplication.StopModal();
                            //							};
                            //							alert.BeginSheet (this.overlayPanel);

                            //							UIAlertView alert = new UIAlertView("Warning", "The company server is down.  They have been notified.  We don?t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.", null, "OK", null);
                            //							alert.Show();
                            //							this.DismissViewController(true, null);
                            //							loadingOverlay.RemoveFromSuperview();

                        });
                        WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "3rdPartyDown", "0", "0");
                    }
                    else
                    {
                        _sessionCredentials = authResult;

                        InvokeOnMainThread(() =>
                        {
                            overlay.UpdateText("Authorization Checking...");
                        });

                        ClientRequestModel clientRequestModel = new ClientRequestModel();
                        clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
                        clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                        clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
                        clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
                        clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
                        clientRequestModel.Platform = CommonClass.Platform;
                        clientRequestModel.RequestType = (int)RequestTypes.SubmitBid;
                        token = new Guid();
                        clientRequestModel.Token = token;
                        clientRequestModel.Version = CommonClass.AppVersion;
                        clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(userName, @"\d+").Value);
                        client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);

                    }
                }
			
				
			}
			else
			{
				InvokeOnMainThread(() =>
					{
                    string alertmessage = GlobalSettings.VPSDownAlert;
                        if (Reachability.IsSouthWestWifiOr2wire())
                        {
                            alertmessage = GlobalSettings.SouthWestConnectionAlert;
                        }
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.MessageText = "WBidMax";
                    alert.InformativeText = alertmessage;
						alert.RunModal();
						NSApplication.SharedApplication.EndSheet(overlayPanel);
						this.Window.Close();
						this.Window.OrderOut(this);
						NSApplication.SharedApplication.StopModal();

//						alert.AddButton("OK");
//						((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
//							NSApplication.SharedApplication.EndSheet(this.overlayPanel);
//							this.Window.Close();
//							this.Window.OrderOut(this);
//							NSApplication.SharedApplication.StopModal();
//						};
//						alert.BeginSheet (this.overlayPanel);

//						UIAlertView alert = new UIAlertView("WBidMax", "Connectivity not available", null, "OK", null);
//						alert.Show();
//						//NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
//						this.DismissViewController(true, null);
//						loadingOverlay.RemoveFromSuperview();

					});
			}
		}

		#region WCFEvents
		private async void client_GetAuthorizationforMultiPlatformCompleted(object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
		{
			ServiceResponseModel serviceResponseModel = new ServiceResponseModel ();
			try {
				if (e.Result != null) {
					serviceResponseModel = e.Result;
				}
			} catch (Exception ex) {
				serviceResponseModel.IsAuthorized = true;
				try {
					client.LogTimeOutDetailsAsync(token);
				} catch (Exception exc) {

				}
			}

			if (serviceResponseModel.IsAuthorized) {

				SubmitBid submitBid = GlobalSettings.SubmitBid;

				SWASubmitBid swaSubmit = new SWASubmitBid ();
				InvokeOnMainThread (() => {
					overlay.UpdateText ("Submitting Bid...");
				});

				if (GlobalSettings.IsSWAApi)
				{
					var bidresult = await SubmitSwaFAbid();
                    if (bidresult.Count > 0)
                    {
                        InvokeOnMainThread( () => {
							NSApplication.SharedApplication.EndSheet(this.overlayPanel);
							overlayPanel.OrderOut(this);
					//this.Window.Close();
					
						string bidString = string.Empty;
						foreach (var item in bidresult)
						{
							string fileName = item.EmployeeId + "Rct.pdf";
							bidString = (GlobalSettings.SubmitBid.Bidder == item.EmployeeId) ? string.Empty : item.JobShareId1!=null?" Job Share":" Buddy";
                                var alert = new NSAlert();
                                alert.AlertStyle = NSAlertStyle.Warning;
                                alert.MessageText = "WBidMax";
                                string message = "Your" + bidString + " Bid Was Successfully Submitted.\n\n A bid Reciept was saved in the " + fileName + " file .This Receipt will Display next.\n\nPlease Review your bid and check for accuracy!";


                                alert.InformativeText = message;
                                alert.AddButton("OK");
                                alert.RunSheetModal(this.Window);


                                var fileViewer = new FileWindowController();
                                fileViewer.Window.Title = "Bid Receipt";
                                fileViewer.LoadPDF(fileName);
                                //CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
                                this.Window.AddChildWindow(fileViewer.Window, NSWindowOrderingMode.Above);
                                fileViewer.Window.MakeKeyAndOrderFront(this);
                                NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
                                //this.Window.Close();
                                //this.Window.OrderOut(this);

                                //if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
                                //{
                                //    //VerifySubmittedData(item.SubmitResult);
                                //}
                        }


					});
				  }
                }
				else
				{
                    List<SubmittedDatas> bidrecipt = Submitbid();
                    string bidString = string.Empty;
                    int bidIndex = 0;
                    InvokeOnMainThread(() => {
                        NSApplication.SharedApplication.EndSheet(this.overlayPanel);
                        overlayPanel.OrderOut(this);
                        this.Window.Close();
                        foreach (SubmittedDatas item in bidrecipt)
                        {
                            bidString = (bidIndex == 0) ? string.Empty : " Buddy ";

                            var alert = new NSAlert();
                            alert.AlertStyle = NSAlertStyle.Warning;
                            alert.MessageText = "WBidMax";
                            string message = "Your" + bidString + " Bid Was Successfully Submitted.\n\n A bid Reciept was saved in the " + item.fileName + " file .This Receipt will Display next.\n\nPlease Review your bid and check for accuracy!";


                            alert.InformativeText = message;
                            alert.AddButton("OK");
                            alert.RunSheetModal(this.Window);


                            var fileViewer = new FileWindowController();
                            fileViewer.Window.Title = "Bid Receipt";
                            fileViewer.LoadPDF(item.fileName);
                            //CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
                            this.Window.AddChildWindow(fileViewer.Window, NSWindowOrderingMode.Above);
                            fileViewer.Window.MakeKeyAndOrderFront(this);
                            NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
                            //this.Window.Close();
                            //this.Window.OrderOut(this);
                            bidIndex++;

                            //if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
                            //{
                            //    VerifySubmittedData(item.SubmitResult);
                            //}

                        }
                    });

                }


            } else {
				InvokeOnMainThread (() => {
					var alert = new NSAlert ();
					alert.AlertStyle = NSAlertStyle.Warning;
					alert.MessageText = "Error";
					alert.InformativeText = serviceResponseModel.Message;
					alert.RunModal();
					NSApplication.SharedApplication.EndSheet(overlayPanel);
					this.Window.Close();
					this.Window.OrderOut(this);
					NSApplication.SharedApplication.StopModal();

				});
			}

		}
		#endregion


		/// <summary>
		/// Get Buddy Bid String
		/// </summary>
		/// <returns></returns>
		private string GetBuddyBid()
		{
			string buddyBidStr = string.Empty;
			//BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
			//disable buddy bid
			buddyBidStr += (GlobalSettings.SubmitBid.Buddy1 != null) ? GlobalSettings.SubmitBid.Buddy1.ToString() + "," : "";
			buddyBidStr += (GlobalSettings.SubmitBid.Buddy2 != null) ? GlobalSettings.SubmitBid.Buddy2.ToString() + "," : "";
			buddyBidStr = buddyBidStr.TrimEnd(',');
			return buddyBidStr;

		}
		/// <summary>
		/// Get Avoidance Bid string
		/// </summary>
		/// <returns></returns>
		private string GetAvoidanceBid()
		{
			string avoidanceBidsStr = string.Empty;
			//AvoidanceBids avoidancebids = GlobalSettings.WBidINIContent.AvoidanceBids;
			avoidanceBidsStr += (GlobalSettings.SubmitBid.Pilot1 != null) ? GlobalSettings.SubmitBid.Pilot1.ToString() + "," : "";
			avoidanceBidsStr += (GlobalSettings.SubmitBid.Pilot2 != null) ? GlobalSettings.SubmitBid.Pilot2.ToString() + "," : "";
			avoidanceBidsStr += (GlobalSettings.SubmitBid.Pilot3 != null) ? GlobalSettings.SubmitBid.Pilot3.ToString() : "";
			avoidanceBidsStr = avoidanceBidsStr.TrimEnd(',');
			return avoidanceBidsStr;
		}

		/// <summary>
		/// Submit the bid.it return the List<string> which contains the list of the submitted result.
		/// </summary>
		/// <returns></returns>
		///
		private async Task<List<SwaBidResponse>> SubmitSwaFAbid()
		{
			List<SwaBidResponse> bidResult = new List<SwaBidResponse>();
			try
			{
				var submitBid = GlobalSettings.SubmitBid;
				SwaApiSubmitBid submitRequest = new SwaApiSubmitBid();
				submitRequest.BidChoices = new List<string>(submitBid.Bid.Split(','));
				submitRequest.BidSource = "WEBBID";
				submitRequest.Department = "IF";
				submitRequest.EmployeeId = submitBid.Bidder;
				submitRequest.SubmittedBy = CommonClass.UserName.Replace("e", "").Replace("E", "");
				//adding buddy bids
				Dictionary<string, List<string>> buddyBids = new Dictionary<string, List<string>>();
				if(submitBid.Buddy1!=null && submitBid.Buddy1!="0")
				{
					var buddy1 = submitBid.Buddy1;
					var buddy1Bid = submitBid.BuddyBidderBids.FirstOrDefault(x => x.BuddyBidder == buddy1).BidLines;
					buddyBids[buddy1] = new List<string>(buddy1Bid.Split(','));
					//buddyBids["6431"] = new List<string>(buddy1Bid.Split(','));

				}
                if (submitBid.Buddy2 != null && submitBid.Buddy2 != "0")
                {
                    var buddy2 = submitBid.Buddy2;
                    var buddy2Bid = submitBid.BuddyBidderBids.FirstOrDefault(x => x.BuddyBidder == buddy2).BidLines;
					buddyBids[buddy2] = new List<string>(buddy2Bid.Split(','));
					//buddyBids["45519"] = new List<string>(buddy2Bid.Split(','));
				}
                if (submitBid.Buddy3 != null && submitBid.Buddy3 != "0")
                {
                    var buddy3 = submitBid.Buddy3;
                    var buddy3Bid = submitBid.BuddyBidderBids.FirstOrDefault(x => x.BuddyBidder == buddy3).BidLines;
					buddyBids[buddy3] = new List<string>(buddy3Bid.Split(','));
				}
				if(submitBid.JobShare1!=null && submitBid.JobShare1!="0"&& submitBid.JobShare1!=string.Empty)
				{
					submitRequest.JobShare1 = submitBid.JobShare1;
				}
                if (submitBid.JobShare2 != null && submitBid.JobShare2 != "0" && submitBid.JobShare2 != string.Empty)
                {
                    submitRequest.JobShare2 = submitBid.JobShare2;
					submitRequest.JobShareContingent = GlobalSettings.isJobShareContingencyBid;
                }

                submitRequest.BuddyBids = buddyBids;
				submitRequest.SubmittedAt = DateTime.Now;
				submitRequest.PacketId = new PacketId()
				{
					Base=GlobalSettings.CurrentBidDetails.Domicile,
					RoundType=GlobalSettings.CurrentBidDetails.Round=="M"?"PRIMARY":"SECONDARY",
					SchedulePeriod = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(GlobalSettings.CurrentBidDetails.Month).ToUpper(),
					Year = GlobalSettings.CurrentBidDetails.Year

				};

				//pkce for test
				if(GlobalSettings.ModifiedEmployeeNumber!=string.Empty && GlobalSettings.ModifiedEmployeeNumber !=null)
				{
					submitRequest.EmployeeId = GlobalSettings.ModifiedEmployeeNumber;
				}
				//

				//string packetID = GenaratePacketId();
				string packetID = WBidHelper.GetAPIPackgeId(GlobalSettings.CurrentBidDetails);
				bidResult = await ServiceHelper.SubmitBid(submitRequest, packetID);
				if(bidResult.Count>0)
				{
					bidResult = bidResult.OrderByDescending(x => x.EmployeeId == submitBid.Bidder).ToList();	
					foreach(var bid in bidResult)
					{
						bool saveReceipt = false;
						saveReceipt = CommonClass.SaveSwaFaFormatBidReceipt(bid);
	
						if(!saveReceipt)
						{
							try
							{
								WBidMail objMailAgent = new WBidMail();
								objMailAgent.SendMailtoAdmin("User got bid receipt, but failed to save it in the app data folder.", GlobalSettings.WbidUserContent.UserInformation.Email, "User has Invalid Account");
							}
							catch (Exception)
							{

							}
						}
						//Log Submit Bid Details

						LogSWAAPIBidSubmission(bid);
					}
                    try
                    {
                        await Task.Factory.StartNew(() =>
                        {
							string message = string.Empty;
							if(bidResult.Count>0)
							{
                                StringBuilder msg = new StringBuilder();
                                msg.Append("Submit Bid");
								foreach(var item in bidResult)
								{
									msg.Append($" {item.EmployeeId}-{item.ConfirmationNumber},");
								}
								message = msg.ToString().TrimEnd(',');
							}
                            WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
							obgWBidLogEvent.LogBidSubmitDetailsSWAAPI(bidResult[0], "submitBid", message);

                        });
                    }
                    catch (Exception ex)
                    {

                    }

                }
				//SendSwaFABidReceiptsToEmail(bidResult, bidResult.FirstOrDefault().SubmittedBy);

                if (bidResult.Count>0)
				{
					AddCertifyEventLog();
				}

            }
			catch(Exception ex)
			{
                if (ex.Message.Contains("Bid submitted successfully but was not able") || ex.Message.Contains("Failed to submit bid"))
                {

					_ = Task.Run(() =>
					{
						try
						{
                            WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                            obgWBidLogEvent.LogBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.SubmitBid.Bidder, "bidSubmitTimeOut", "bidSubmitTimeOut",ex.Message);
                            SaveSubmittedRawDataToDB(GlobalSettings.SubmitBid, _sessionCredentials="");
                        }
						catch(Exception)
						{

						}
					});
                 
                    InvokeOnMainThread(() =>
                    {
                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        overlayPanel.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();
                        
                    });
                    BeginInvokeOnMainThread(() =>
                    {
                        var alert = new NSAlert
                        {
                            AlertStyle = NSAlertStyle.Warning,
                            MessageText = "WBidMax",
                            InformativeText = ex.Message
                        };
                        alert.RunSheetModal(this.Window);

                    });

                }
				else
				{
                    InvokeOnMainThread(() =>
                    {

                        CommonClass.AppDelegate.ErrorLog(ex);
                        CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                    });
                }
               
            }
			return bidResult;
		}

		private async void LogSWAAPIBidSubmission(SwaBidResponse bid)
		{
            
            try
            {
                await Task.Run(() =>
                {
                    SaveSubmittedSWAAPIDataToDB(bid);
                });
            }
            catch (Exception ex)
            {

            }
            try
            {
                await Task.Run(() =>
                {
                    SaveSubmittedRawDataToDBSWAAPI(GlobalSettings.SubmitBid, bid);


                });
            }
            catch (Exception ex)
            {

            }
        }

        private string GenaratePacketId()
        {
            string packetid = string.Empty;
            packetid = GlobalSettings.BidPrepDetails.Domicile + GlobalSettings.BidPrepDetails.BidYear + GlobalSettings.BidPrepDetails.BidPeriod.ToString("d2");

            //Set-round-numbers:
            //1 - F/A monthly bids
            //2 - F/A supplemental bids
            //3 - reserved
            //4 - Pilot monthly bids
            //5 - Pilot supplemental bids

            //D first Round  B second Round
            if (GlobalSettings.BidPrepDetails.Position == "FA")
            {
                packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "1" : "2";
            }
            else if (GlobalSettings.BidPrepDetails.Position == "CP" || GlobalSettings.BidPrepDetails.Position == "FO")
            {
                packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "4" : "5";
            }
            else
            {
                packetid = "3";
            }


            return packetid;
        }
        /// <summary>
        /// PURPOSE : Generate Bid lines
        /// </summary>
        /// <returns></returns>
     
        private List<SubmittedDatas> Submitbid()
		{           
			List<SubmittedDatas> bidrecipt = new List<SubmittedDatas>();
			try
			{
				SWASubmitBid swasubmitbid = new SWASubmitBid();
                string submittedby = string.Empty;

                List<string> bidders = new List<string>();
				bidders.Add(GlobalSettings.SubmitBid.Bidder);

				//When submitting a  buddy bid, it should submit either 2 or 3 bids, depending upon if there are 2 or 3 buddy bidders.
				if (GlobalSettings.CurrentBidDetails.Postion == "FA")
				{
					if (GlobalSettings.SubmitBid.Buddy1 != null && GlobalSettings.SubmitBid.Buddy1 != "0")
						bidders.Add(GlobalSettings.SubmitBid.Buddy1);
					if (GlobalSettings.SubmitBid.Buddy2 != null && GlobalSettings.SubmitBid.Buddy2!="0")
						bidders.Add(GlobalSettings.SubmitBid.Buddy2);
				}
				int count = 0;
                string submbittingbid = GlobalSettings.SubmitBid.Bid;
				foreach (string buddybidder in bidders)
				{
					string bidder = buddybidder;
					GlobalSettings.SubmitBid.Bidder = bidder;

					if (GlobalSettings.CurrentBidDetails.Postion == "FA")
					{
						//first bidder is always the user.other usesr are buddy bidders for that user.so we need to set the 
						if (count != 0)
						{
							var bid = GlobalSettings.SubmitBid.BuddyBidderBids.FirstOrDefault(x => x.BuddyBidder == bidder);
							if (bid != null)
								GlobalSettings.SubmitBid.Bid = GlobalSettings.SubmitBid.BuddyBidderBids.FirstOrDefault(x => x.BuddyBidder == bidder).BidLines;
						}
						//When submitting a  buddy bid, it should submit either 2 or 3 bids, depending upon if there are 2 or 3 buddy bidders.
						//this will get the buddy bidders other than the bidder.
						List<string> buddybidders = bidders.Where(x => x != bidder).ToList();
						if (buddybidders.Count() == 1)
						{
							GlobalSettings.SubmitBid.Buddy1 = buddybidders[0];
						}
						else if (buddybidders.Count() == 2)
						{
							GlobalSettings.SubmitBid.Buddy1 = buddybidders[0];
							GlobalSettings.SubmitBid.Buddy2 = buddybidders[1];
						}
						if (GlobalSettings.SubmitBid.Buddy1 == "0")
							GlobalSettings.SubmitBid.Buddy1 = null;
						if (GlobalSettings.SubmitBid.Buddy2 == "0")
							GlobalSettings.SubmitBid.Buddy2 = null;


						count++;

					}
				
					SubmitBid submitBid = GlobalSettings.SubmitBid;
					SWASubmitBid swaSubmit = new SWASubmitBid();
					InvokeOnMainThread(() =>
						{
							overlay.UpdateText("Submitting Bid for " + GlobalSettings.SubmitBid.Bidder);
						});

					//string submitResult = string.Empty;
					string submitResult = swaSubmit.SubmitBid(submitBid, _sessionCredentials);

					if (submitResult.Contains("SUBMITTED BY:"))
					{
						string fileName = submitResult.Substring(0, submitResult.IndexOf("\n")) + "Rct.pdf";
						if (fileName != null)
						{
							//System.IO.File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + fileName, submitResult);
							bool SubmitScucess = false;

							if (GlobalSettings.CurrentBidDetails.Postion == "FA")
							{
								SubmitScucess = CommonClass.SaveFormatBidReceipt(submitResult);
							}
							else
							{
								SubmitScucess = CommonClass.SaveFormattedBidReceiptForPilot(submitResult);
							}

							if (SubmitScucess)
                            {
                                Task.Factory.StartNew(() =>
                                    {
                                        WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                                        obgWBidLogEvent.LogBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.SubmitBid.Bidder, "submitBid","Submit Bid");

									});
                            }
                            else
                            {
                                try
                                {
                                    WBidMail objMailAgent = new WBidMail();
                                    objMailAgent.SendMailtoAdmin("User got bid receipt, but failed to save it in the app data folder.", GlobalSettings.WbidUserContent.UserInformation.Email, "User has Invalid Account");
                                }
                                catch(Exception)
                                {
                                    
                                }
                            }
							bidrecipt.Add(new SubmittedDatas {fileName=fileName,SubmitResult=submitResult, bidder=bidder });
							//if (GlobalSettings.WBidINIContent.User.IsNeedBidReceipt)
							//{
							//	SendEmailBidReceipt(fileName);
							//}

						}
                        try
                        {


                           
                            string submittedfor = string.Empty;
                            string submitteddtg = string.Empty;
                            string submittedbid = string.Empty;
                            string[] stringSeparators = new string[] { "SUBMITTED BY:" };
                            var splittedstring = submitResult.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                            string[] lastlineseparator = new string[] { "  " };
                            var lastsplittedString = splittedstring[1].Split(lastlineseparator, StringSplitOptions.RemoveEmptyEntries);
                            if (lastsplittedString.Count() > 2)
                            {
                                submittedby = Regex.Replace(lastsplittedString[0], @"[^\d]", "");
                                submittedfor = lastsplittedString[1];
                                submitteddtg = lastsplittedString[2].Replace("\r\n", "");
                                submittedbid = splittedstring[0].Substring(splittedstring[0].IndexOf('\n'), splittedstring[0].Length - splittedstring[0].IndexOf('\n')).Replace('\n', ',').Replace("*E,", ",").Trim().TrimEnd(',').TrimStart(',');

                            }
                            else
                            {
                                submittedbid = GlobalSettings.SubmitBid.Bid;
                            }
                            int submitbidder = 0;
                            if ((GlobalSettings.SubmitBid.Bidder.Contains('x')) || (GlobalSettings.SubmitBid.Bidder.Contains('X')))
                                submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("x", "").Replace("X", ""));
                            else
                                submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", ""));
							
							if (submittedby == string.Empty || submittedby == null)
							{

								InvokeOnMainThread(() =>
								{
									var alert = new NSAlert();
									alert.AlertStyle = NSAlertStyle.Warning;
									alert.MessageText = "WBidMax";
									alert.InformativeText = " Your bid receipt has been returned with NO employee number.This can occur when you are on a leave of absence.Please contact us if you are not on a leave of absence";

									alert.RunModal();
									NSApplication.SharedApplication.EndSheet(overlayPanel);
									this.Window.Close();
									this.Window.OrderOut(this);
									NSApplication.SharedApplication.StopModal();
															

								});

								Task.Factory.StartNew(() =>
								{
									WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
									obgWBidLogEvent.LogBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.SubmitBid.Bidder, "MissingEmpNumReceipt", "Missing EmpNum in Bid Receipt");
									

								});
							}

							SaveSubmittedDataToDB(submitbidder, submittedbid, submittedby, submittedfor, submitteddtg, submitBid, _sessionCredentials);
                            GlobalSettings.WBidStateCollection.SubmittedResult = submbittingbid;
                            //int submitbidder = 0;
                            //if ((GlobalSettings.SubmitBid.Bidder.Contains('x')) || (GlobalSettings.SubmitBid.Bidder.Contains('X')))
                            //    submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("x", "").Replace("X", ""));
                            //else
                            //    submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", ""));

                            //SaveSubmittedDataToDB(submitbidder, GlobalSettings.SubmitBid.Bid);
                            //GlobalSettings.WBidStateCollection.SubmittedResult = submbittingbid;
                        }
                        catch (Exception ex)
                        {
                        }


					}
					else if (submitResult == "server failure")
					{
						WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
						obgWBidLogEvent.LogTimeoutBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber, submitResult);
						SaveSubmittedRawDataToDB(submitBid, _sessionCredentials);
						InvokeOnMainThread(() =>
							{
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								//alert.InformativeText = "The company server is down.  They have been notified.  We don’t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.\n\nIf you are running out of time to bid, we suggest you use CWA – EBS to submit your bid.  CWA uses a  different server.\n\nIf you have time to wait, you can attempt to submit your bid later.";
								alert.InformativeText = "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a Server Timeout.\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\nChange your internet connection.You can also try to use your cell phone as a hotspot for your internet connection \n\nFinally, send us an email if you are continuously having trouble.";

								alert.RunModal();
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								this.Window.Close();
								this.Window.OrderOut(this);
								NSApplication.SharedApplication.StopModal();

//								alert.AddButton("OK");
//								((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
//									NSApplication.SharedApplication.EndSheet(this.overlayPanel);
//									this.Window.Close();
//									this.Window.OrderOut(this);
//									NSApplication.SharedApplication.StopModal();
//								};
//								alert.BeginSheet (this.overlayPanel);

//								UIAlertView alert = new UIAlertView("WBidMax", "The company server is down.  They have been notified.  We don’t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.\n\nIf you are running out of time to bid, we suggest you use CWA – EBS to submit your bid.  CWA uses a  different server.\n\nIf you have time to wait, you can attempt to submit your bid later.", null, "OK", null);
//								alert.Show();
//								loadingOverlay.RemoveFromSuperview();
							});
					}
					else
					{
						WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
						obgWBidLogEvent.LogTimeoutBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber, submitResult);
						SaveSubmittedRawDataToDB(submitBid, _sessionCredentials);
						string submitalert = submitResult;
						if (submitResult.Contains("STRINGINDEXOUTOFBOUNDSEXCEPTION"))
						{
							submitalert= " Your bid receipt has been returned with NO employee number.This can occur when you are on a leave of absence.Please contact us if you are not on a leave of absence";
							WBidLogEvent obgWBidLogEvent1 = new WBidLogEvent();
							obgWBidLogEvent1.LogBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.SubmitBid.Bidder, "MissingEmpNumReceipt", "Missing EmpNum in Bid Receipt");
						}
							InvokeOnMainThread(() =>
							{
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								alert.InformativeText = submitalert;
								alert.RunModal();
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								this.Window.Close();
								this.Window.OrderOut(this);
								NSApplication.SharedApplication.StopModal();

//								((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
//									NSApplication.SharedApplication.EndSheet(this.overlayPanel);
//									this.Window.Close();
//									this.Window.OrderOut(this);
//									NSApplication.SharedApplication.StopModal();
//								};
//								alert.BeginSheet (this.overlayPanel);

//								UIAlertView alert = new UIAlertView("WBidMax", submitResult, null, "OK", null);
//								alert.Show();
//								loadingOverlay.RemoveFromSuperview();
							});
					}
				}

				SendBidReceiptsToEmail(bidrecipt,submittedby);

                if (bidrecipt.Count() > 0)
                {
                    AddCertifyEventLog();

                }
            }
			catch (Exception ex)
			{

				InvokeOnMainThread(() =>
					{

						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					});
			}
			return bidrecipt;
		}

        private void AddCertifyEventLog()
        {
            if (GlobalSettings.IsBidderCertified)
            {

                string message = string.Empty;
                message = "Certify with empnum " + CommonClass.UserName.Substring(1) + " and " + GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", "") + " as the bid4EmpNum";
                WBidHelper.LogDetails(CommonClass.UserName.Substring(1), "certify", message, "0", "0", GlobalSettings.SubmitBid.Bidder);
            }
        }
        /// <summary>
        /// Check the FA users submitted positions are matached with the Fa position line data stored in the server.
        /// </summary>
        public void VerifySubmittedData(string submittedString)
		{
			try
			{
				

				UserBidDetails inputParama = new UserBidDetails();

				inputParama.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
				inputParama.Position = GlobalSettings.CurrentBidDetails.Postion;
				inputParama.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
				inputParama.Month = GlobalSettings.CurrentBidDetails.Month;
				inputParama.Year = GlobalSettings.CurrentBidDetails.Year;


				var jsonData = ServiceUtils.JsonSerializer(inputParama);
				StreamReader dr = ServiceUtils.GetRestData("GetFALinePositionDatasForABid", jsonData);
				FALinePositionsResponseDTO FalInePositionData = WBidCollection.ConvertJSonStringToObject<FALinePositionsResponseDTO>(dr.ReadToEnd());

				string errorBid = string.Empty;
				if (FalInePositionData.lstLinePositions.Count > 0)
				{
					List<string> lstSubmittedData = new List<string>();
					var splitresult = submittedString.Split('*');
					if (splitresult.Length > 1)
					{
						lstSubmittedData = splitresult[0].Replace('\n', ',').Split(',').ToList();

						lstSubmittedData.RemoveAt(0);
						lstSubmittedData.RemoveAt(lstSubmittedData.Count - 1);
					}
					foreach (var item in lstSubmittedData)
					{
						if (item != "R")
						{
							int lineNum = Convert.ToInt32(Regex.Match(item, @"\d+").Value);
							var dbFapositions = FalInePositionData.lstLinePositions.FirstOrDefault(x => x.LineNum == lineNum);
							if (dbFapositions != null)
							{
								var dbposition = dbFapositions.FaPositions;
								string linePOsition = item[item.Length - 1].ToString();
								if (!dbposition.Contains(linePOsition))
								{
									errorBid += (item + " , ");
								}
							}
						}
					}

					if (errorBid.Length > 0)
					{
						errorBid = errorBid.Remove(errorBid.Length - 2, 2);

						InvokeOnMainThread(() =>
						{
							var alert = new NSAlert();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.MessageText = "WBidMax";
							alert.InformativeText = "We have detected corruption in your below bid submission \nBids :" + errorBid + "\nPlease contact us immediately: support@wbidmaxapp.zendesk.com";

							alert.RunModal();
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							this.Window.Close();
							this.Window.OrderOut(this);
							NSApplication.SharedApplication.StopModal();

						});
												
						WBidMail objMailAgent = new WBidMail();
						string biddata = inputParama.Domicile + " - " + inputParama.Position + " - " + ((inputParama.Round==1)?"First Round":"Second round") + " - " + inputParama.Month + " - " + inputParama.Year;
						objMailAgent.SendMailtoAdmin("\nThis user submitted the below error bids:\n<br></br> " + errorBid + "\n<br></br>BidData :" + biddata, GlobalSettings.WbidUserContent.UserInformation.Email, "Bid Submission Corruption");

					}
					else
					{
						InvokeOnMainThread(() =>
						{
							var alert = new NSAlert();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.MessageText = "WBidMax";
							alert.InformativeText = "We have verified the accuracy of your bid data.";

							alert.RunModal();
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							this.Window.Close();
							this.Window.OrderOut(this);
							NSApplication.SharedApplication.StopModal();

						});
						
					}
				}
			}
			catch (Exception ex)
			{

			}
		}
		private void SaveSubmittedDataToDB(int empnum, string bid,string submittedby,string submittedfor,string submitteddtg,SubmitBid submitBid, string sessioncredential)
        {
            try
            {
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                if (isConnectionAvailable)
                {
                   
                    BidSubmittedData biddetails = new BidSubmittedData();
                    biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    biddetails.EmpNum = empnum;
                    biddetails.SubmittedResult = bid;
                    biddetails.SubmitBy = submittedby;
                    biddetails.SubmitFor = submittedfor;
                    biddetails.SubmitDTG = submitteddtg;
                    biddetails.FromApp = (int)WBidiPad.Core.Enum.FromApp.WbidmaxMACApp;
					biddetails.RawData = GenerateSubmittedRawData(submitBid, sessioncredential);
					var jsonData = ServiceUtils.JsonSerializer(biddetails);
                    StreamReader dr = ServiceUtils.GetRestData("SaveBidSubmittedData", jsonData);
                    GlobalSettings.WBidStateCollection.SubmittedResult = (WBidCollection.ConvertJSonStringToObject<BidSubmittedData>(dr.ReadToEnd())).SubmittedResult;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SaveSubmittedSWAAPIDataToDB(SwaBidResponse submitBid)
        {
            try
            {
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                if (isConnectionAvailable)
                {

                    BidSubmittedData biddetails = new BidSubmittedData();
                    biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    biddetails.EmpNum = Convert.ToInt32(submitBid.EmployeeId);
                    biddetails.SubmittedResult = string.Join(",",submitBid.BidChoices.Where(x=>x.Status=="SUBMITTED").Select(x=>x.Choice).ToList());
                    biddetails.SubmitBy = submitBid.SubmittedBy;
                    biddetails.SubmitFor = submitBid.EmployeeId;
                    biddetails.SubmitDTG = submitBid.ReceivedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    biddetails.FromApp = (int)WBidiPad.Core.Enum.FromApp.WbidmaxMACApp;
                    biddetails.RawData = GenerateSubmittedRawDataSWAAPI(submitBid,GlobalSettings.SubmitBid);
                    var jsonData = ServiceUtils.JsonSerializer(biddetails);
                    StreamReader dr = ServiceUtils.GetRestData("SaveBidSubmittedData", jsonData);
                    GlobalSettings.WBidStateCollection.SubmittedResult = (WBidCollection.ConvertJSonStringToObject<BidSubmittedData>(dr.ReadToEnd())).SubmittedResult;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void SaveSubmittedRawDataToDB(SubmitBid submitBid, string sessioncredential)
		{
			try
			{
				bool isConnectionAvailable = Reachability.CheckVPSAvailable();
				if (isConnectionAvailable)
				{
					int employeenumber = 0;
					if ((GlobalSettings.SubmitBid.Bidder.Contains('x')) || (GlobalSettings.SubmitBid.Bidder.Contains('X')))
						employeenumber = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("x", "").Replace("X", ""));
					else
						employeenumber = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", ""));
					
					SubmittedDataRaw biddetails = new SubmittedDataRaw();
					biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
					biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
					biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
					biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
					biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
					biddetails.EmployeeNumber = employeenumber.ToString();
					biddetails.RawData = GenerateSubmittedRawData(submitBid, sessioncredential);
					biddetails.FromApp = (int)WBidiPad.Core.Enum.FromApp.WbidmaxIpad;
					

					var jsonData = ServiceUtils.JsonSerializer(biddetails);
					StreamReader dr = ServiceUtils.GetRestData("AddSubmittedRawDataToServer", jsonData);
					
				}
			}
			catch (Exception ex)
			{
			}
		}
        private void SaveSubmittedRawDataToDBSWAAPI(SubmitBid submitBid,SwaBidResponse submitResult)
        {
            try
            {
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                if (isConnectionAvailable)
                {
                    
                    SubmittedDataRaw biddetails = new SubmittedDataRaw();
                    biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    biddetails.EmployeeNumber = submitResult.EmployeeId;
                    biddetails.RawData = GenerateSubmittedRawDataSWAAPI(submitResult,submitBid);
                    biddetails.FromApp = (int)WBidiPad.Core.Enum.FromApp.WbidmaxIpad;


                    var jsonData = ServiceUtils.JsonSerializer(biddetails);
                    StreamReader dr = ServiceUtils.GetRestData("AddSubmittedRawDataToServer", jsonData);

                }
            }
            catch (Exception ex)
            {
            }
        }

        private string GenerateSubmittedRawData(SubmitBid submitBid, string sessioncredential)
		{
			//set the formdata values
			NameValueCollection formData = new NameValueCollection();
			formData["REQUEST"] = "UPLOAD_BID";
			formData["BASE"] = submitBid.Base;
			formData["BID"] = submitBid.Bid;
			formData["BIDDER"] = submitBid.Bidder;
			formData["BIDROUND"] = submitBid.BidRound;
			//formData["CLOSEDBIDSIM"] = "N";
			formData["CREDENTIALS"] = sessioncredential;
			formData["PACKETID"] = submitBid.PacketId;
			formData["SEAT"] = submitBid.Seat;
			formData["VENDOR"] = "WBidMax";
			// should always be null for CP and FA
			if (submitBid.Pilot1 != null) formData["PILOT1"] = submitBid.Pilot1;
			if (submitBid.Pilot2 != null) formData["PILOT2"] = submitBid.Pilot2;
			if (submitBid.Pilot3 != null) formData["PILOT3"] = submitBid.Pilot3;
			// should always be null for CP and FO
			if (submitBid.Buddy1 != null) formData["BUDDY1"] = submitBid.Buddy1;
			if (submitBid.Buddy2 != null) formData["BUDDY2"] = submitBid.Buddy2;
			string submittedraw = string.Empty;
			foreach (var item in formData.Keys)
			{
				submittedraw += item.ToString() + ":" + formData.GetValues(item.ToString())[0] + ",";
			}
			return submittedraw;
		}

        private string GenerateSubmittedRawDataSWAAPI(SwaBidResponse submitBidResult, SubmitBid submitBid)
        {
            //set the formdata values
            NameValueCollection formData = new NameValueCollection();
            formData["REQUEST"] = "UPLOAD_BID";
            formData["BASE"] = submitBid.Base;
            formData["BID"] = submitBid.Bid;
            formData["BIDDER"] = submitBid.Bidder;
            formData["BIDROUND"] = submitBid.BidRound;
            //formData["CLOSEDBIDSIM"] = "N";
            formData["CREDENTIALS"] = "";
            formData["PACKETID"] = submitBid.PacketId;
            formData["SEAT"] = submitBid.Seat;
            formData["VENDOR"] = "WBidMax";
            // should always be null for CP and FO
            if (submitBidResult.BuddyId1 != null) formData["BUDDY1"] = submitBidResult.BuddyId1;
            if (submitBidResult.BuddyId2 != null) formData["BUDDY2"] = submitBidResult.BuddyId2;
            string submittedraw = string.Empty;
            foreach (var item in formData.Keys)
            {
                submittedraw += item.ToString() + ":" + formData.GetValues(item.ToString())[0] + ",";
            }
            return submittedraw;
        }

        /// <summary>
        /// Send Bid receipt all users including buddy bidders using one single service
        /// </summary>
        /// <param name="submittedresult"></param>
        private void SendBidReceiptsToEmail(List<SubmittedDatas> submittedresult, string submittedby)
        {
            BidReceiptMailInputParam input = new BidReceiptMailInputParam();
            input.Empnum = submittedby;
			input.IsQATest = GlobalSettings.buddyBidTest;
            if (submittedresult.Count > 0)
            {
                input.Bidder = submittedresult[0].bidder;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + submittedresult[0].fileName))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + submittedresult[0].fileName);
                    input.BidderBidReceipt = attachment;
                }

            }
            if (submittedresult.Count == 2)
            {
                input.Buddy1 = submittedresult[1].bidder;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + submittedresult[1].fileName))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + submittedresult[1].fileName);
                    input.Buddy1Receipt = attachment;
                }

            }
            else if (submittedresult.Count == 3)
            {
                input.Buddy1 = submittedresult[1].bidder;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + submittedresult[1].fileName))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + submittedresult[1].fileName);
                    input.Buddy1Receipt = attachment;
                }

                input.Buddy2 = submittedresult[2].bidder;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + submittedresult[2].fileName))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + submittedresult[2].fileName);
                    input.Buddy2Receipt = attachment;
                }
            }
            try
            {
                var jsonData = ServiceUtils.JsonSerializer(input);
                StreamReader dr = ServiceUtils.PostDataToWbidCoreService("BidData/SendBidReceiptsToUsersEmail", jsonData);
                var status = WBidCollection.ConvertJSonStringToObject<bool>(dr.ReadToEnd());
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
            }

        }
        private void SendSwaFABidReceiptsToEmail(List<SwaBidResponse> submittedresult, string submittedby)
        {
            BidReceiptMailInputParam input = new BidReceiptMailInputParam();
            input.Empnum = submittedby;
            input.IsQATest = GlobalSettings.buddyBidTest;
            if (submittedresult.Count > 0)
            {
				string fileName = submittedresult[0].EmployeeId + "Rct.pdf";
                input.Bidder = submittedresult[0].EmployeeId;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + submittedresult[0].EmployeeId ))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + fileName);
                    input.BidderBidReceipt = attachment;
                }

            }
            if (submittedresult.Count == 2)
            {
                string buddy1File = submittedresult[1].EmployeeId + "Rct.pdf";
				input.Buddy1 = submittedresult[1].EmployeeId;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + buddy1File))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + buddy1File);
                    input.Buddy1Receipt = attachment;
                }

            }
            else if (submittedresult.Count == 3)
            {
                string buddy1File = submittedresult[1].EmployeeId + "Rct.pdf";
                string buddy2File = submittedresult[2].EmployeeId + "Rct.pdf";
                input.Buddy1 = submittedresult[1].EmployeeId;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + buddy1File))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + buddy1File);
                    input.Buddy1Receipt = attachment;
                }

                input.Buddy2 = submittedresult[2].EmployeeId;
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + buddy2File))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + buddy2File);
                    input.Buddy2Receipt = attachment;
                }
            }
            try
            {
                var jsonData = ServiceUtils.JsonSerializer(input);
                StreamReader dr = ServiceUtils.PostDataToWbidCoreService("BidData/SendBidReceiptsToUsersEmail", jsonData);
                var status = WBidCollection.ConvertJSonStringToObject<bool>(dr.ReadToEnd());
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
            }

        }
        private void SendEmailBidReceipt(string bidFileName)
		{

			try {
				WBidMail objMailAgent = new WBidMail ();
				//if (GlobalSettings.WBidINIContent.User != null && GlobalSettings.WBidINIContent.User.IsOn && GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null && !string.IsNullOrEmpty(GlobalSettings.WbidUserContent.UserInformation.Email))
				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null && !string.IsNullOrEmpty (GlobalSettings.WbidUserContent.UserInformation.Email)) {
					if (File.Exists (WBidHelper.GetAppDataPath () + "/" + bidFileName)) {
						byte[] attachment = System.IO.File.ReadAllBytes (WBidHelper.GetAppDataPath () + "/" + bidFileName);
						objMailAgent.SendMailtoUser ("Hi <Br/> Please find the attached Bid Receipt. <Br/><Br/> WBidMax", GlobalSettings.WbidUserContent.UserInformation.Email, "Bid Receipt", attachment, bidFileName);
					}
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		private void ShowBidReceipt(string filename)
		{
			try {
				string filepath = WBidHelper.GetAppDataPath () + "\\" + filename;
				
				var fileViewer = new FileWindowController ();
				fileViewer.Window.Title = "Bid Recept";
				fileViewer.LoadPDF(filepath);
				CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
				fileViewer.Window.MakeKeyAndOrderFront (this);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		public void DismissCurrentView()
		{
			this.Window.Close();
			this.Window.OrderOut(this);
			//			CommonClass.DownloadController.Window.ResignKeyWindow ();
			NSApplication.SharedApplication.StopModal();
		}
		public class SubmittedDatas
		{
			public string fileName { get; set; }
			public string SubmitResult { get; set; }
            public string bidder { get; set; }
        }
	}
}

