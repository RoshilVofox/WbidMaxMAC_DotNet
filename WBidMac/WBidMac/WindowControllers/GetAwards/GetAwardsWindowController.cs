
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
//using System.Collections.Generic;
using WBid.WBidiPad.Model;
//using System.Linq;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidMac.Mac.WindowControllers.CustomAlert;
using WBid.WBidMac.Mac.ViewControllers.CustomAlertView;
using CoreGraphics;
using ObjCRuntime;
using WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow;
using WBidMac.SwaApiModels;
using ADT.Common.Utility;
using System.Text;
using System.Globalization;

namespace WBid.WBidMac.Mac
{
	public partial class GetAwardsWindowController : AppKit.NSWindowController
	{ 
		WBidDataDwonloadAuthServiceClient client;
		private string _sessionCredentials = string.Empty;
		NSObject notif;
		private DownloadInfo _downloadFileDetails;
        public string ErrorMessage { get; set; }
        #region Constructors
        string[] domicileArray = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
		NSPanel overlayPanel;
		OverlayViewController overlay;

		// Called when created from unmanaged code
		public GetAwardsWindowController (NativeHandle handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public GetAwardsWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public GetAwardsWindowController () : base ("GetAwardsWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new GetAwardsWindow Window {
			get {
				return (GetAwardsWindow)base.Window;
			}
		}

		static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					this.Window.Close ();
					this.Window.OrderOut(this);
				};
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				
				SetupViews ();
				SetupButtons ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void SetupViews ()
		{
			try {
				btnDomicile.AddItems (domicileArray);
				//btnDomicile.SelectItem (GlobalSettings.WbidUserContent.UserInformation.Domicile);
				
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
				client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
				    
				client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
				// Perform any additional setup after loading the view, typically from a nib.
				
				//int indexToSelect = GlobalSettings.WBidINIContent.Domiciles.IndexOf(GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile));
				btnDomicile.SelectItem (GlobalSettings.CurrentBidDetails.Domicile);
				//pckrDomicilePick.Select(indexToSelect, 0, true);
				//domicileName = GlobalSettings.CurrentBidDetails.Domicile;
				if (DateTime.Now.Day <= 19)
					btnAwards.SelectCellWithTag (0);
				else
					btnAwards.SelectCellWithTag (1);
				
				if (GlobalSettings.CurrentBidDetails.Postion == "CP")
					btnPosition.SelectCellWithTag (0);
				else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
					btnPosition.SelectCellWithTag (1);
				else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
					btnPosition.SelectCellWithTag (2);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		void SetupButtons ()
		{
			try {
				btnPosition.Activated += (object sender, EventArgs e) => {
				
				};
				btnAwards.Activated += (object sender, EventArgs e) => {
				
				};
				btnRetrieve.Activated += (object sender, EventArgs e) => {
					GlobalSettings.IsSWAApi = GlobalSettings.IsSWAApiTest && GlobalSettings.CurrentBidDetails.Postion == "FA";

                    if (GlobalSettings.IsSWAApiTest && GlobalSettings.CurrentBidDetails.Postion=="FA")
					{
                        notif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"PKCE_loginSuccess", PKCELoginSuccess);
                        SwaLoginWindowController swaLogin = new SwaLoginWindowController();
						this.Window.AddChildWindow(swaLogin.Window, NSWindowOrderingMode.Above);
						swaLogin.Window.MakeKeyAndOrderFront(this);
						NSApplication.SharedApplication.RunModalForWindow(swaLogin.Window);
					}
					else
					{
                        LoginViewController login = new LoginViewController();
                        var panel = new NSPanel();
                        CommonClass.Panel = panel;
                        panel.SetContentSize(new CoreGraphics.CGSize(450, 250));
                        panel.ContentView = login.View;
                        notif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LoginSuccess", HandleLoginSuccess);
                        NSApplication.SharedApplication.BeginSheet(panel, this.Window);
                        
                    }
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleLoginSuccess (NSNotification obj)
		{
			try {
                NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
                if (obj.Object.ToString () == "Login") {
					overlayPanel = new NSPanel ();
					overlayPanel.SetContentSize (new CoreGraphics.CGSize (400, 120));
					overlay = new OverlayViewController ();
					overlay.OverlayText = "Retreiving Bid Award..";
					overlayPanel.ContentView = overlay.View;
					NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
				
					InitiateDownloadProcess (overlayPanel);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

        void PKCELoginSuccess(NSNotification obj)
        {
            try
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(notif);
                if (obj.Object.ToString() == "Success")
                {
                    
                    overlayPanel = new NSPanel();
                    overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                    overlay = new OverlayViewController();
                    overlay.OverlayText = "Retreiving Bid Award..";
                    overlayPanel.ContentView = overlay.View;
                    NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

                    AuthCheckandDownload(overlayPanel);
                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

		public void AuthCheckandDownload(NSPanel overlay)
		{
            try
            {

                _downloadFileDetails = new DownloadInfo();
                _downloadFileDetails.UserId = CommonClass.UserName;
                _downloadFileDetails.Password = CommonClass.Password;

                //checking  the internet connection available
                //==================================================================================================================
                if (Reachability.CheckVPSAvailable())
                {
                   
                        NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);
                        // this.startProgress();

                        _sessionCredentials = "";

                        ClientRequestModel clientRequestModel = new ClientRequestModel();
                        clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
                        clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                        clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
                        clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
                        clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
                        clientRequestModel.Platform = CommonClass.Platform;
                        clientRequestModel.RequestType = (int)RequestTypes.DownloadAward;
                        clientRequestModel.Token = new Guid();
                        clientRequestModel.Version = CommonClass.AppVersion;
                        clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
                        client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);
                    
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
                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        overlayPanel.OrderOut(this);
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Warning;
                        alert.Window.Title = "WBidMax";
                        alert.MessageText = "Bid Award Download";
                        alert.InformativeText = alertmessage;
                        alert.AddButton("OK");
                        alert.RunModal();

                        //UIAlertView alert = new UIAlertView("WBidMax", "Connectivity not available", null, "OK", null);
                        //alert.Show();
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
                        //this.DismissViewController(true, null);
                    });
                }
            }
            catch (Exception ex)
            {



                InvokeOnMainThread(() =>
                {

                    CommonClass.AppDelegate.ErrorLog(ex);
                    CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                });
            }
        }

        /// <summary>
        /// start the download process ..
        /// </summary>
        public void InitiateDownloadProcess( NSPanel overlay)
		{
			try
			{
				
				_downloadFileDetails = new DownloadInfo();
				_downloadFileDetails.UserId = CommonClass.UserName;
				_downloadFileDetails.Password = CommonClass.Password;

				//checking  the internet connection available
				//==================================================================================================================
                if (Reachability.CheckVPSAvailable())
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
					//checking CWA credential
					//==================================================================================================================

					//this.startProgress();
					Authentication authentication = new Authentication();
					string authResult = authentication.CheckCredential(_downloadFileDetails.UserId, _downloadFileDetails.Password);
					if (authResult.Contains("ERROR: "))
					{
						WBidLogEvent objlogs = new WBidLogEvent();
						objlogs.LogBadPasswordUsage(_downloadFileDetails.UserId,false, authResult);
						InvokeOnMainThread(() =>
							{
								//KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);
								//UIAlertView alert = new UIAlertView("WBidMax", "Invalid Username or Password", null, "OK", null);
								//alert.Show();
								//overlay.RemoveFromSuperview();

								var panel = new NSPanel();
								var customAlert = new CustomAlertViewController();
								customAlert.AlertType = "InvalidCredential";
								customAlert.objAwardWindow = this;
								CommonClass.Panel = panel;
								panel.SetContentSize(new CGSize(430, 350));
								panel.ContentView = customAlert.View;
								NSApplication.SharedApplication.BeginSheet(panel, this.Window);

								//	var alert = new NSAlert ();
								//	alert.AlertStyle = NSAlertStyle.Warning;
								//	alert.Window.Title = "WBidMax";
								//	alert.MessageText = "Bid Award Download";
								//	alert.InformativeText = "Invalid Username or Password";
								//	alert.AddButton ("OK");
								//alert.Buttons[0].Activated += (object senderr, EventArgs ee) =>
								//{
								//	alert.Window.Close();
								//	//							menuCheckUpdate.Enabled = false;
								//	NSApplication.SharedApplication.StopModal();
								//	NSApplication.SharedApplication.EndSheet(overlayPanel);
								//	overlayPanel.OrderOut(this);
								//};
								//	alert.RunModal ();

							});
					}
					else if (authResult.Contains("Exception"))
					{

						InvokeOnMainThread(() =>
							{
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Informational;
								alert.Window.Title = "WBidMax";
								alert.MessageText = "Bid Award Download";
								//alert.InformativeText = "The company server is down.  They have been notified.  We don’t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also";
								alert.InformativeText = "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a Server Timeout.\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\nChange your internet connection.You can also try to use your cell phone as a hotspot for your internet connection \n\nFinally, send us an email if you are continuously having trouble.";

								alert.AddButton ("OK");
								alert.RunModal ();
								//UIAlertView alert = new UIAlertView("Warning", "The company server is down.  They have been notified.  We don’t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.", null, "OK", null);
								//alert.Show();
								//overlay.RemoveFromSuperview();
								//this.DismissViewController(true, null);
							});
                        WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "3rdPartyDown", "0", "0");
                    }
					else
					{
						NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);
						// this.startProgress();

						_sessionCredentials = authResult;

						ClientRequestModel clientRequestModel = new ClientRequestModel();
						clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
						clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
						clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
						clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
						clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
						clientRequestModel.Platform = CommonClass.Platform;
						clientRequestModel.RequestType = (int)RequestTypes.DownloadAward;
						clientRequestModel.Token = new Guid();
						clientRequestModel.Version = CommonClass.AppVersion;
						clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
						client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);
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
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							overlayPanel.OrderOut(this);
							var alert = new NSAlert ();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.Window.Title = "WBidMax";
							alert.MessageText = "Bid Award Download";
                        alert.InformativeText = alertmessage;
							alert.AddButton ("OK");
							alert.RunModal ();

							//UIAlertView alert = new UIAlertView("WBidMax", "Connectivity not available", null, "OK", null);
							//alert.Show();
							//NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
							//this.DismissViewController(true, null);
						});
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
			//}
		}
		/// <summary>
		/// Create the Filename for the Award file based on teh UI selection
		/// </summary>
		private void GenarateAwardFileName()
		{
			List<string> downLoadList = new List<string>();
			try
			{

				if (btnPosition.SelectedCell.Tag == 0)
				{
					string fileName = btnDomicile.SelectedItem.Title + "CP" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
					fileName = btnDomicile.SelectedItem.Title  + "FO" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
				}
				else if (btnPosition.SelectedCell.Tag  == 1)
				{
					string fileName = btnDomicile.SelectedItem.Title + "FO" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
					fileName = btnDomicile.SelectedItem.Title + "CP" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
				}
				else if (btnPosition.SelectedCell.Tag  == 2)
				{
					string fileName = btnDomicile.SelectedItem.Title + "FA" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
				}
				else
				{
					return;
				}
				_downloadFileDetails.DownloadList = downLoadList;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private async Task<SwaAwardModel> AwardSwaApiDownload(string round)
		{
            SwaAwardModel swaAward = new SwaAwardModel();
            try
            {
                var bidDetails = GlobalSettings.CurrentBidDetails;
                string packageID = bidDetails.Domicile + bidDetails.Year + bidDetails.Month.ToString().PadLeft(2, '0') + round;
                if (round=="1")
                {
                    var lineawardTask = ServiceHelper.GetBidAwardAsync(packageID);
                    var jobShareawardTask = ServiceHelper.GetJobShareBidAwardAsync(packageID);
                    var MrtawardTask = ServiceHelper.GetMRTBidAwardAsync(packageID);
                    var reserveAwardTask = ServiceHelper.GetReserveAwardAsync(packageID);
                    await Task.WhenAll(lineawardTask, jobShareawardTask, MrtawardTask, reserveAwardTask);
                    swaAward.LineAwards = await lineawardTask;
                    swaAward.JobShareAwards = await jobShareawardTask;
                    swaAward.MrtAwards = await MrtawardTask;
                    swaAward.ReserveAwards = await reserveAwardTask;
                }
                else
                {
                    swaAward.LineAwards = await ServiceHelper.GetBidAwardAsync(packageID);
                }
                
    //            swaAward.LineAwards = await ServiceHelper.GetBidAwardAsync(WBidHelper.GetAPIPackgeId(GlobalSettings.CurrentBidDetails));
    //            swaAward.JobShareAwards = await ServiceHelper.GetJobShareBidAwardAsync(WBidHelper.GetAPIPackgeId(GlobalSettings.CurrentBidDetails));
				//swaAward.MrtAwards = await ServiceHelper.GetMRTBidAwardAsync(WBidHelper.GetAPIPackgeId(GlobalSettings.CurrentBidDetails));

			}
            catch (Exception ex)
			{
                ErrorMessage = string.Empty;
                if(ex.Message.Contains("Awards are not published for the Packet"))
                {
                    ErrorMessage = "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.";
                }
                else
                {
                    ErrorMessage = "Error fetching award data";
                }
               
			
            }
            return swaAward;


        }

		private bool SaveSwaAwardOrigFormat(SwaAwardModel swaAwards,string fileName)
		{
			try
			{

				var currentBiddetails = GlobalSettings.CurrentBidDetails;
				string awardFileName = fileName;
				string filePath = WBidHelper.GetAppDataPath() + "/"+ awardFileName;
				using (var writer = new StreamWriter(filePath,false))
				{
                    //MRT Award Data Entry
                    writer.WriteLine(GetHeader("mrt"));

                    if (swaAwards.MrtAwards != null && swaAwards.MrtAwards.Count > 0)
                    {
                        try
                        {
							var mrtAwardList = swaAwards.MrtAwards.OrderBy(x => x.BaseSeniority).ToList();
							foreach(var mrtAward in mrtAwardList)
							{
                                string seniority = mrtAward.BaseSeniority.ToString().PadRight(8, ' ');
								string employeeID = mrtAward.EmployeeId.PadRight(10, ' ');
								string empName=mrtAward.LegalName.PadRight(45, ' ');
								string cont=mrtAward.Contingent?"Yes":"No";
								string finalLine = seniority + employeeID + empName + cont;
								writer.WriteLine(finalLine);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    else
                    {
                        writer.WriteLine("NONE");
                    }

                    writer.WriteLine(Environment.NewLine);

                    //Job Share Award Data Entry
                    writer.WriteLine(GetHeader("jobshare"));
                    if (swaAwards.JobShareAwards != null && swaAwards.JobShareAwards.Count > 0)
                    {
                        var jobShareList = swaAwards.JobShareAwards.OrderBy(x => Convert.ToInt32(x.Line)).ThenBy(x => x.JobSharePosition).ThenBy(x => x.Position).ToList();
                        bool isLastentry = false;
                        for (int index = 0; index < jobShareList.Count; index++)
                        {
                            try
                            {
                                var jobAward = jobShareList[index];
                                string seniority = jobAward.BaseSeniority.ToString().PadRight(6, ' ');
                                string employeeID = jobAward.EmployeeId.PadRight(8, ' ');
                                string empName = jobAward.LegalName.PadRight(40, ' ');
                                string jobPosition = "JS" + jobAward.JobSharePosition.ToString();
                                string line = jobAward.Line.PadRight(6, ' ');
                                string postion = jobAward.Position.PadRight(5, ' ');
                                string contingent = jobAward.Contingent ? "Yes" : "No";
                                string finalLine = seniority + employeeID + empName + jobPosition.PadRight(5, ' ') + line + postion + contingent;
                                writer.WriteLine(finalLine);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                    else
                    {
                        writer.WriteLine("NONE");  
                    }

                    writer.WriteLine(Environment.NewLine);


                    //Line Award Data Entry
                    writer.WriteLine(GetHeader("line"));
					bool isFirstEntry = true;
					bool isFinalEntry = false;
                    var awardList = swaAwards.LineAwards.OrderBy(x => Convert.ToInt32(x.Line)).ThenBy(x => x.Position).ToList();
                    for (int index=0;index<awardList.Count;index++)
					{
						var award = awardList[index];
                        string lineNum;
                        string seperator = string.Empty;
                        if (isFirstEntry)
                        {
                            lineNum = award.Line.PadLeft(3, ' ') + "-" + (award.Position ?? string.Empty).PadRight(4, ' ');

                        }
                        else
                        {
                            //lineNum = award.Line.PadLeft(3, ' ') + " " + award.Position.PadRight(4, ' ');
                            lineNum = (award.Position ?? string.Empty).PadLeft(5, ' ') + "   ";
                        }
                        if (index + 1 < awardList.Count)
                        {
                            isFirstEntry = award.Line != awardList[index + 1].Line;
                        }
                        else if (index + 1 == awardList.Count)
                        {
                            isFinalEntry = true;
                        }
                        //isFirstEntry = award.Line != awardList[index + 1].Line;
                        string seniority = award.BaseSeniority.ToString().PadRight(6, ' ');
                        string senName = string.Empty;
                        if (swaAwards.JobShareAwards.Any(x => x.EmployeeId == award.EmployeeId))
                        {
                            var jsAward = swaAwards.JobShareAwards.FirstOrDefault(x => x.EmployeeId == award.EmployeeId);
                            string temp = "- " + award.LegalName + $"...JS{jsAward.JobSharePosition}";
                            senName = temp.PadRight(45, '.');
                        }
                        else
                        {
                            string temp = "- " + award.LegalName;
                            senName = temp.PadRight(45, '.');
                        }
                        string empNumber = $"[{award.EmployeeId}]";
                        string empDisp = empNumber.PadRight(12, ' ');
                        string regulatory = award.Regulatory ? "Y" : "N";
                        string finalLine = lineNum + seniority + senName + empDisp + regulatory;
                        writer.WriteLine(finalLine);
						if(isFirstEntry && index!=0)
						{
                            writer.WriteLine("--------------------------------------------------------");
                        }

                    }

                    //Reserve List Data Entry
                    writer.WriteLine(Environment.NewLine);
                    writer.WriteLine(GetHeader("reserve"));
                    bool isreserve = false;//for testing 
                    if (swaAwards.ReserveAwards != null && swaAwards.ReserveAwards.Count > 0)
                    {
						var reserveList = swaAwards.ReserveAwards.OrderBy(x=>x.BaseSeniority).ToList();
						int count = 0;
						foreach(var reserveAward in reserveList)
						{
							count++;
							string index = count.ToString().PadRight(4);
                            string slNo = $"{index}- ";
							string name = reserveAward.LegalName.PadRight(45,'.');
							string empNum = $"[{reserveAward.EmployeeId}]";
							string finalLine = slNo + name + empNum;
							writer.WriteLine(finalLine);
						}
                    }
                    else
                    {
                        writer.WriteLine("NONE");      
                    }

                    writer.WriteLine(Environment.NewLine);

                    //Alphabetical ordered Line Award List Entry
                    writer.WriteLine(GetHeader("lineasc"));
                    var orderedAwardList = swaAwards.LineAwards.OrderBy(x => x.LegalName).ToList();
                    foreach (var award in orderedAwardList)
                    {
                        string seniority = award.BaseSeniority.ToString().PadRight(6, ' ') + "-";
                        string name = string.Empty;
                        if (swaAwards.JobShareAwards.Any(x => x.EmployeeId == award.EmployeeId))
                        {
                            var jsAward = swaAwards.JobShareAwards.FirstOrDefault(x => x.EmployeeId == award.EmployeeId);
                            string temp = award.LegalName + $"...JS{jsAward.JobSharePosition}";
                            name = temp.PadRight(45, '.');
                        }
                        else
                        {
                            name = award.LegalName.PadRight(45, '.');
                        }
                        string empNum = $"[{award.EmployeeId}]";
                        string emp = empNum.PadRight(10, ' ');
                        string linePosition = $"{award.Line}-{award.Position}";
                        string finalEntry = seniority + " " + name + emp + linePosition;
                        writer.WriteLine(finalEntry);
                    }

                }
			}
			catch(Exception ex)
			{
				return false;
			}
			return true;
		}

        private bool SaveSecondRoundSwaAwardOrigFormat(SwaAwardModel swaAwards, string fileName)
        {
            try
            {

                var currentBiddetails = GlobalSettings.CurrentBidDetails;
                string awardFileName = fileName;
                string filePath = WBidHelper.GetAppDataPath() + "/" + awardFileName;
                List<SwaBidAward.IFLineBaseAuctionAward> vrAwards = new List<SwaBidAward.IFLineBaseAuctionAward>();
                List<SwaBidAward.IFLineBaseAuctionAward> sarAwards = new List<SwaBidAward.IFLineBaseAuctionAward>();
                List<SwaBidAward.IFLineBaseAuctionAward> jarAwards = new List<SwaBidAward.IFLineBaseAuctionAward>();
                List<SwaBidAward.IFLineBaseAuctionAward> sprAwards = new List<SwaBidAward.IFLineBaseAuctionAward>();
                List<SwaBidAward.IFLineBaseAuctionAward> jprAwards = new List<SwaBidAward.IFLineBaseAuctionAward>();
                List<SwaBidAward.IFLineBaseAuctionAward> jlrAwards = new List<SwaBidAward.IFLineBaseAuctionAward>();
                var awardList = swaAwards.LineAwards.OrderBy(x => Convert.ToInt32(x.Line)).ToList();
                using (var writer = new StreamWriter(filePath, false))
                {
                    //VR Award Data Entry
                    writer.WriteLine(GetHeader("secondary"));
                    string vrHeader = $"VR   SEN#    EMP#                                                 Regulatory(Y/N)";
                    writer.WriteLine(vrHeader);
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(Environment.NewLine);
                    vrAwards = awardList.Where(x => x.ReserveType == "VR").ToList();
                    WriteReserveAwardData(writer, vrAwards);
                    

                    writer.WriteLine(Environment.NewLine);

                    //SAR Award Data Entry
                    string sarHeader = $"SAR  SEN#    EMP#                                                 Regulatory(Y/N)";
                    writer.WriteLine(sarHeader);
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(Environment.NewLine);
                    sarAwards = awardList.Where(x => x.ReserveType == "SAR").ToList();
                    WriteReserveAwardData(writer, sarAwards);
                   

                    writer.WriteLine(Environment.NewLine);


                    //JAR Award Data Entry
                    string jarHeader = $"JAR  SEN#    EMP#                                                 Regulatory(Y/N)";
                    writer.WriteLine(jarHeader);
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(Environment.NewLine);
                    jarAwards = awardList.Where(x => x.ReserveType == "JAR").ToList();
                    WriteReserveAwardData(writer, jarAwards);
                   

                    writer.WriteLine(Environment.NewLine);


                    //SPR Award Data Entry
                    string sprHeader = $"SPR  SEN#    EMP#                                                 Regulatory(Y/N)";
                    writer.WriteLine(sprHeader);
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(Environment.NewLine);
                    sprAwards = awardList.Where(x => x.ReserveType == "SPR").ToList();
                    WriteReserveAwardData(writer, sprAwards);
                   

                    writer.WriteLine(Environment.NewLine);


                    //JPR Award Data Entry
                    string jprHeader = $"JPR  SEN#    EMP#                                                 Regulatory(Y/N)";
                    writer.WriteLine(jprHeader);
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(Environment.NewLine);
                    jprAwards = awardList.Where(x => x.ReserveType == "JPR").ToList();
                    WriteReserveAwardData(writer, jprAwards);
                   

                    writer.WriteLine(Environment.NewLine);


                    //JLR Award Data Entry
                    string jlrHeader = $"JLR  SEN#    EMP#                                                 Regulatory(Y/N)";
                    writer.WriteLine(jlrHeader);
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(Environment.NewLine);
                    jlrAwards = awardList.Where(x => x.ReserveType == "JLR").ToList();
                    WriteReserveAwardData(writer, jlrAwards);
                    

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private void WriteReserveAwardData(StreamWriter writer,List<SwaBidAward.IFLineBaseAuctionAward> awardList)
        {
            for (int index = 0; index < awardList.Count; index++)
            {
                var award = awardList[index];
                string lineNum;
                string seperator = string.Empty;
                lineNum = award.Line.PadRight(5, ' ');
                string seniority = award.BaseSeniority.ToString().PadRight(6, ' ') + "- ";
                string senName = award.LegalName.PadRight(45, '.');
                string empNumber = $"[{award.EmployeeId}]";
                string empDisp = empNumber.PadRight(12, ' ');
                string regulatory = award.Regulatory ? "Y" : "N";
                string finalLine = lineNum + seniority + senName + empDisp + regulatory;
                writer.WriteLine(finalLine);

            }
        }

        private string GetHeader(string listType)
        {
            var currentBiddetails = GlobalSettings.CurrentBidDetails;
            StringBuilder header = new StringBuilder();
            switch (listType)
            {
                case "mrt":
                    header.Append("**Subject to Protest**" + Environment.NewLine);
                    header.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentBiddetails.Month)} {currentBiddetails.Year}" + Environment.NewLine);
                    header.Append($"MRT Employees Awards {currentBiddetails.Domicile} Base.");
                    header.Append(Environment.NewLine);
                    header.Append(Environment.NewLine);
                    header.Append("Sen     EmpID     Name                                         Cont. Bid");
                    header.Append(Environment.NewLine);
                    break;
                case "jobshare":
                    header.Append("**Subject to Protest**" + Environment.NewLine);
                    header.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentBiddetails.Month)} {currentBiddetails.Year}" + Environment.NewLine);
                    header.Append($"Job Share Employees Awards {currentBiddetails.Domicile} Base.");
                    header.Append(Environment.NewLine);
                    header.Append(Environment.NewLine);
                    header.Append("Sen   EmpID   Name                                    JPos L     Pos  Cont. Bid");
                    header.Append(Environment.NewLine);
                    break;
                case "line":
                    header.Append("**Subject to Protest**" + Environment.NewLine);
                    header.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentBiddetails.Month)} {currentBiddetails.Year}");
                    header.Append($"Award List - {currentBiddetails.Domicile}");
                    header.Append(Environment.NewLine);
                    header.Append(Environment.NewLine);
                    header.Append("Line-Pos Sen#  Name                               Emp #  NH        Regulatory (Y/N)");
                    header.Append(Environment.NewLine);
                    header.Append("------------------------------------------------------------------------------------\n");
                    break;
                case "reserve":
                    header.Append("**Subject to Protest**" + Environment.NewLine);
                    header.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentBiddetails.Month)} {currentBiddetails.Year}" + Environment.NewLine);
                    header.Append($"Reserve List - {currentBiddetails.Domicile} Base");
                    header.Append(Environment.NewLine);
                    break;
                case "lineasc":
                    header.Append("**Subject to Protest**" + Environment.NewLine);
                    header.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentBiddetails.Month)} {currentBiddetails.Year}" + Environment.NewLine);
                    header.Append($"Award List - {currentBiddetails.Domicile}");
                    header.Append(Environment.NewLine);
                    header.Append(Environment.NewLine);
                    header.Append("Sen     Name                                         Emp#      Line-Pos");
                    header.Append(Environment.NewLine);
                    header.Append("-----------------------------------------------------------------------\n");
                    break;
                case "secondary":
                    header.Append("**Subject to Protest**" + Environment.NewLine);
                    header.Append($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentBiddetails.Month)} {currentBiddetails.Year}" + Environment.NewLine);
                    header.Append($"Reserve Award List" + Environment.NewLine);
					header.Append($"{currentBiddetails.Domicile} Base");
                    header.Append(Environment.NewLine);
                    break;
            }
            return header.ToString();
        }

        

        /// <summary>
        /// Donwload the awards and show it ot the file viewer.
        /// </summary>
        private void AwardDownlaod()
		{
			try {
				DownloadAward downloadAward = new DownloadAward ();
				List<DownloadedFileInfo> lstDownloadedFiles = downloadAward.DownloadAwardDetails (_downloadFileDetails);

				if (lstDownloadedFiles [0].IsError) {


					InvokeOnMainThread (() => {
						NSApplication.SharedApplication.EndSheet(overlayPanel);
						overlayPanel.OrderOut(this);
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "Bid Award Download";
						alert.InformativeText = "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.";
						alert.AddButton ("OK");
						alert.RunModal ();

						//UIAlertView alert = new UIAlertView ("WBidMax", "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.", null, "OK", null);
						//alert.Show ();
						//overlay.RemoveFromSuperview ();

					});
				} else
				{


					foreach (DownloadedFileInfo fileinfo in lstDownloadedFiles) {
						FileStream fStream = new FileStream (Path.Combine (WBidHelper.GetAppDataPath (), fileinfo.FileName), FileMode.Create);
						fStream.Write (fileinfo.byteArray, 0, fileinfo.byteArray.Length);
						fStream.Dispose ();
					}
					bool isNeedtoShowAwardData = true;
                    var filename = lstDownloadedFiles[0].FileName;
                    if (filename.Substring(5, 1) == "M")
                    {
                        UserBidDetails biddetails = new UserBidDetails();
                        biddetails.Domicile = filename.Substring(0, 3);
                        biddetails.Position = filename.Substring(3, 2);
                        biddetails.Round = filename.Substring(5, 1) == "M" ? 1 : 2;
                        biddetails.Year = DateTime.Now.AddMonths(1).Year;
                        biddetails.Month = DateTime.Now.AddMonths(1).Month;
                        if (biddetails.Round == 1)
                        {

                            if (GlobalSettings.IsDifferentUser)
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0'), @"\d+").Value);
                            }
                            else
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
                            }
                            string alertmessage = WBidHelper.GetAwardAlert(biddetails);
                            if (alertmessage != string.Empty)
                            {
                                alertmessage = alertmessage.Insert(0, "\n\n");
                                alertmessage += "\n\n";
                                InvokeOnMainThread(() =>
                                {

									var alert = new NSAlert();
									alert.Window.Title = "WBidMax";
									alert.MessageText = alertmessage;
									alert.AddButton("Ok");
									
									alert.Buttons[0].Activated += delegate
									{
										isNeedtoShowAwardData = false;
										alert.Window.Close();
										NSApplication.SharedApplication.StopModal();

										NSApplication.SharedApplication.EndSheet(overlayPanel);
										overlayPanel.OrderOut(this);
										var fileViewer = new FileWindowController();
										fileViewer.Window.Title = "Bid Awards";
										fileViewer.LoadTXT(lstDownloadedFiles[0].FileName);
										fileViewer.Window.MakeKeyAndOrderFront(this);
										NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
										this.Window.Close();
									};
									
									alert.RunModal();


									

                                });
                            }

                        }
                    }
					if (isNeedtoShowAwardData)
					{
						InvokeOnMainThread(() =>
						{
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							overlayPanel.OrderOut(this);
							var fileViewer = new FileWindowController();
							fileViewer.Window.Title = "Bid Awards";
							fileViewer.LoadTXT(lstDownloadedFiles[0].FileName);
							fileViewer.Window.MakeKeyAndOrderFront(this);
							NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
							//CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
							//fileViewer.Window.MakeKeyAndOrderFront (this);
							this.Window.Close();

							//webPrint fileViewer = new webPrint ();
							//this.PresentViewController (fileViewer, true, () => {
							//fileViewer.loadFileFromUrl (lstDownloadedFiles [0].FileName);
							//});
						});
					}
				}
				InvokeOnMainThread (() => {
					DismissCurrentView();
					//Goooverlay.RemoveFromSuperview ();

				});
			} catch (Exception ex) {

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void DisplaySwaAPIAward(string fileName)
		{
            try
            {
                

                    bool isNeedtoShowAwardData = true;
                    var filename = fileName;
                    if (filename.Substring(5, 1) == "M")
                    {
                        UserBidDetails biddetails = new UserBidDetails();
                        biddetails.Domicile = filename.Substring(0, 3);
                        biddetails.Position = filename.Substring(3, 2);
                        biddetails.Round = filename.Substring(5, 1) == "M" ? 1 : 2;
                        biddetails.Year = DateTime.Now.AddMonths(1).Year;
                        biddetails.Month = DateTime.Now.AddMonths(1).Month;
                        if (biddetails.Round == 1)
                        {

                            if (GlobalSettings.IsDifferentUser)
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0'), @"\d+").Value);
                            }
                            else
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
                            }
                            string alertmessage = WBidHelper.GetAwardAlert(biddetails);
                            if (alertmessage != string.Empty)
                            {
                                alertmessage = alertmessage.Insert(0, "\n\n");
                                alertmessage += "\n\n";
                                InvokeOnMainThread(() =>
                                {

                                    var alert = new NSAlert();
                                    alert.Window.Title = "WBidMax";
                                    alert.MessageText = alertmessage;
                                    alert.AddButton("Ok");

                                    alert.Buttons[0].Activated += delegate
                                    {
                                        isNeedtoShowAwardData = false;
                                        alert.Window.Close();
                                        NSApplication.SharedApplication.StopModal();

                                        
                                        var fileViewer = new FileWindowController();
                                        fileViewer.Window.Title = "Bid Awards";
                                        fileViewer.LoadTXT(fileName);
                                        fileViewer.Window.MakeKeyAndOrderFront(this);
                                        NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
                                        this.Window.Close();
                                    };
                                    NSApplication.SharedApplication.EndSheet(overlayPanel);
                                    overlayPanel.OrderOut(this);
                                    alert.RunModal();




                                });
                            }

                        }
                    }
                    if (isNeedtoShowAwardData)
                    {
                        InvokeOnMainThread(() =>
                        {
                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                            overlayPanel.OrderOut(this);
                            var fileViewer = new FileWindowController();
                            fileViewer.Window.Title = "Bid Awards";
                            fileViewer.LoadTXT(fileName);
                            fileViewer.Window.MakeKeyAndOrderFront(this);
                            NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
                            //CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
                            //fileViewer.Window.MakeKeyAndOrderFront (this);
                            this.Window.Close();

                            //webPrint fileViewer = new webPrint ();
                            //this.PresentViewController (fileViewer, true, () => {
                            //fileViewer.loadFileFromUrl (lstDownloadedFiles [0].FileName);
                            //});
                        });
                    }
                
                InvokeOnMainThread(() => {
                    DismissCurrentView();
                    //Goooverlay.RemoveFromSuperview ();

                });
            }
            catch (Exception ex)
            {

                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

		#region WCF CompletedEvent
		private async void client_GetAuthorizationforMultiPlatformCompleted(object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
		{
			try
			{

				if (e.Result != null)
				{
					ServiceResponseModel serviceResponseModel = e.Result;

					if (serviceResponseModel.IsAuthorized)
					{
						//NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);
						//this.startProgress();
						_downloadFileDetails.SessionCredentials = _sessionCredentials;
						InvokeOnMainThread(() =>
							{
								GenarateAwardFileName();
							});
						if(!GlobalSettings.IsSWAApi)
						{
                            AwardDownlaod();
                        }
						else
						{
                            string round = btnAwards.SelectedCell.Tag == 0 ? "1" : "2";

                            var awards=await AwardSwaApiDownload(round);
							string filename = _downloadFileDetails.DownloadList[0];
							if(awards.LineAwards?.Count>0)
							{
                                bool isSaved = false;
                                if(btnAwards.SelectedCell.Tag == 0)
                                {
                                    isSaved = SaveSwaAwardOrigFormat(awards, filename);
                                }
                                else
                                {
                                    isSaved = SaveSecondRoundSwaAwardOrigFormat(awards, filename);
                                }
                                
                                if (isSaved)
                                {
                                    DisplaySwaAPIAward(filename);
                                }
								else
								{
                                    InvokeOnMainThread(() =>
                                    {
                                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                                        overlayPanel.OrderOut(this);
                                        var alert = new NSAlert();
                                        alert.AlertStyle = NSAlertStyle.Warning;
                                        alert.Window.Title = "WBidMax";
                                        alert.MessageText = "Bid Award Download";
                                        alert.InformativeText = "Error saving Award Data";
                                        alert.AddButton("OK");
                                        alert.RunModal();

                                        //UIAlertView alert = new UIAlertView("Error", serviceResponseModel.Message, null, "OK", null);
                                        //alert.Show();
                                        //overlay.RemoveFromSuperview();

                                    });
                                    DismissCurrentView();
                                }
                            }
							else
							{
                                string message = (ErrorMessage != string.Empty && ErrorMessage != null) ? ErrorMessage : "Error fetching award data";
                                InvokeOnMainThread(() =>
                                {
                                    NSApplication.SharedApplication.EndSheet(overlayPanel);
                                    overlayPanel.OrderOut(this);
                                    var alert = new NSAlert();
                                    alert.AlertStyle = NSAlertStyle.Warning;
                                    alert.Window.Title = "WBidMax";
                                    alert.MessageText = "Bid Award Download";
                                    alert.InformativeText = message;
                                    alert.AddButton("OK");
                                    alert.RunSheetModal(this.Window);

                                    //UIAlertView alert = new UIAlertView("Error", serviceResponseModel.Message, null, "OK", null);
                                    //alert.Show();
                                    //overlay.RemoveFromSuperview();

                                });
                                DismissCurrentView();
                            }
							
							

                        }
						
					}
					else
					{
						InvokeOnMainThread(() =>
							{
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.Window.Title = "WBidMax";
								alert.MessageText = "Bid Award Download";
								alert.InformativeText = serviceResponseModel.Message;
								alert.AddButton ("OK");
								alert.RunModal ();
								
								//UIAlertView alert = new UIAlertView("Error", serviceResponseModel.Message, null, "OK", null);
								//alert.Show();
								//overlay.RemoveFromSuperview();

							});
						DismissCurrentView();
					}
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
		}
		#endregion

		public void DismissCurrentView()
		{
			this.Window.Close();
			this.Window.OrderOut(this);
			//			CommonClass.DownloadController.Window.ResignKeyWindow ();
			NSApplication.SharedApplication.StopModal();
		}
	}
}

