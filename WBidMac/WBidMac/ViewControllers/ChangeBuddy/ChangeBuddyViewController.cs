
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
using WBid.WBidiPad.iOS.Utility;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
using System.ServiceModel;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using ADT.Common.Utility;
using WBidMac.SwaApiModels;
using WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow;

namespace WBid.WBidMac.Mac
{
	public partial class ChangeBuddyViewController : AppKit.NSViewController
	{
		NSPanel overlayPanel;
		OverlayViewController overlay;
		public Dictionary <int, FalistData> EmployeeList { get; set; }
        public Dictionary<string, List<string>> EmployeeBuddyList { get; set; }
        public bool isFaPositionChoiceWindow { get; set; }
        public bool isChangeEmployee;
        #region Constructors


        // Called when created from unmanaged code
        public ChangeBuddyViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangeBuddyViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ChangeBuddyViewController () : base ("ChangeBuddyView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ChangeBuddyView View {
			get {
				return (ChangeBuddyView)base.View;
			}
		}
		WBidState wBidStateContent;

        public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
                 wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                txtBuddy1.StringValue = wBidStateContent.BuddyBids.Buddy1;
                txtBuddy2.StringValue = wBidStateContent.BuddyBids.Buddy2;
                EmployeeBuddyList = new Dictionary<string, List<string>>();
                LoadBidderDetails ();

                txtBuddy1.Changed += TxtBuddy1_Changed;
                txtBuddy2.Changed += TxtBuddy2_Changed;
				
				btnCancel.Activated += (object sender, EventArgs e) => {
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
				};
				btnOK.Activated += async (object sender, EventArgs e) => {



					EmployeeDetails empdetails = new EmployeeDetails();
					empdetails.EmployeeNumbers=new Employee[2];
					var obj=new List<Employee>();
					if(string.IsNullOrEmpty(txtBuddy1.StringValue.Trim()))
						txtBuddy1.StringValue="0";
					if(string.IsNullOrEmpty(txtBuddy2.StringValue.Trim()))
						txtBuddy2.StringValue="0";

                    
                    LoadBidderDetails();
                    bool isInvalidBuddy = false;
                    string InvalidBuddyBidders = string.Empty;

                    if (txtBuddy1.StringValue != "0" && lblBuddy1.StringValue == "< No Matching Element >")
                    {
                        isInvalidBuddy = true;
                        InvalidBuddyBidders = txtBuddy1.StringValue;
                    }

                    if (txtBuddy2.StringValue != "0" && lblBuddy2.StringValue == "< No Matching Element >")
                    {
                        isInvalidBuddy = true;
                        if (InvalidBuddyBidders != string.Empty)
                            InvalidBuddyBidders += " , ";
                        InvalidBuddyBidders += txtBuddy2.StringValue;
                    }
                    if(GlobalSettings.TemporaryEmployeeNumber==txtBuddy1.StringValue || GlobalSettings.TemporaryEmployeeNumber==txtBuddy2.StringValue)
                    {
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Informational;
                        alert.MessageText = "WBidMax";
                        alert.InformativeText = "Bidder and Buddy cannot be the same";
                        alert.AddButton("OK");

                        alert.RunModal();
                        return;
                    }
                    if (txtBuddy1.StringValue==txtBuddy2.StringValue && txtBuddy1.StringValue!="0" && txtBuddy2.StringValue!="0")
                    {
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Informational;
                        alert.MessageText = "WBidMax";
                        alert.InformativeText = "Buddy 1 and Buddy 2 cannot be the same";
                        alert.AddButton("OK");

                        alert.RunModal();
                        return;
                    }
                    if (isInvalidBuddy)
                    {
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Informational;
                        alert.MessageText = "WBidMax";
                        alert.InformativeText = "Employee number " + InvalidBuddyBidders + " does not exist in the seniority list";
                        alert.AddButton("OK");

                        alert.RunModal();
                        return;

                    }
                    if ((lblBudd1Domicile.StringValue != "" && lblBudd1Domicile.StringValue != GlobalSettings.CurrentBidDetails.Domicile) || (lblBuddy2Domicile.StringValue != "" && lblBuddy2Domicile.StringValue != GlobalSettings.CurrentBidDetails.Domicile))
                    {
                        

                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Informational;
                        alert.MessageText = "WBidMax";
						alert.InformativeText = "One of the Buddy Bidders is NOT in " + GlobalSettings.CurrentBidDetails.Domicile;
                        alert.AddButton("OK");

                        alert.RunModal();
                        wBidStateContent.BuddyBids.Buddy1 = "0";
                        wBidStateContent.BuddyBids.Buddy2 = "0";
                        txtBuddy1.StringValue = "0";
                        txtBuddy2.StringValue = "0";
                        lblBudd1Domicile.StringValue = "";
                        lblBuddy2Domicile.StringValue = "";
                        lblBuddy1.StringValue = "";
                        lblBuddy2.StringValue = "";

                        GlobalSettings.isModified = true;
                        CommonClass.MainController.UpdateSaveButton(true);


                        return;
                    }
                   // wBidStateContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim();
                   // wBidStateContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim();
                    GlobalSettings.isModified = true;
                    CommonClass.MainController.UpdateSaveButton(true);
                    //save the state of the INI File
                    //WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

                    


					if ((!string.IsNullOrEmpty(txtBuddy1.StringValue.Trim())) && txtBuddy1.StringValue.Trim() != "0")
					{
						obj.Add(new Employee { EmpNumber = txtBuddy1.StringValue.Trim() });
					}
					if ((!string.IsNullOrEmpty(txtBuddy2.StringValue.Trim())) && txtBuddy2.StringValue.Trim() != "0")
					{
						obj.Add(new Employee { EmpNumber = txtBuddy2.StringValue.Trim() });
					}
					empdetails.EmployeeNumbers = obj.ToArray();

					if (empdetails.EmployeeNumbers.Count() > 0)
					{

						if (Reachability.IsHostReachable("google.com"))
						{

							overlayPanel = new NSPanel();
							overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
							overlay = new OverlayViewController();
							overlay.OverlayText = "Checking Authorization for Buddy Bidders..";
							overlayPanel.ContentView = overlay.View;
							NSApplication.SharedApplication.BeginSheet(overlayPanel, this.View.Window);

							try
							{
								empdetails.Platform = "PC";
								var jsonData = ServiceUtils.JsonSerializer(empdetails);
								StreamReader dr = ServiceUtils.PostDataToWbidCoreService("User/CheckValidSubscriptionForEmployees", jsonData);
								List<AuthStatusModel> authStatus = WBidCollection.ConvertJSonStringToObject<List<AuthStatusModel>>(dr.ReadToEnd());

								var authfailedmembers = authStatus.Where(x => x.IsValid == false).ToList();
                                if(GlobalSettings.IsSWAApiTest)
                                {
                                    if (WBidHelper.IsTokenExpired())
                                    {
                                        this.View.Window.EndSheet(overlayPanel);
                                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                                        overlayPanel.OrderOut(this.View.Window);
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
                                                this.View.Window.AddChildWindow(swaloginWindow.Window, NSWindowOrderingMode.Above);
                                                swaloginWindow.Window.MakeKeyAndOrderFront(this);
                                                NSApplication.SharedApplication.RunModalForWindow(swaloginWindow.Window);
                                            }


                                        });
                                    }
                                    else
                                    {
                                        //SWA Buddy check
                                        var isValidBuddy = await SwaBuddyListCheck(empdetails);
                                        if (!isValidBuddy.isValid)
                                        {
                                            string message = "";

                                            if (isValidBuddy.message.FirstOrDefault().Contains("Failed to retrieve Buddy List"))
                                            {
                                                //message = $"Failed to retrieve buddies for {GlobalSettings.TemporaryEmployeeNumber}";
                                                message = isValidBuddy.message.FirstOrDefault();
                                            }
                                            else
                                            {
                                                //message = (isValidBuddy.message.Count() == 1) ? $"The buddy {String.Join(',', isValidBuddy.message)} is not valid for {GlobalSettings.TemporaryEmployeeNumber}" : $"The buddies {String.Join(',', isValidBuddy.message)} are not valid for {GlobalSettings.TemporaryEmployeeNumber}";
                                                message = string.Join("\n", isValidBuddy.message);
                                            }
                                            InvokeOnMainThread(() =>
                                            {

                                                var alert = new NSAlert();
                                                alert.AlertStyle = NSAlertStyle.Informational;
                                                alert.MessageText = "WBidMax";
                                                alert.InformativeText = message;
                                                alert.AddButton("OK");

                                                alert.RunModal();
                                                this.View.Window.EndSheet(overlayPanel);
                                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                                overlayPanel.OrderOut(this.View.Window);

                                            });
                                        }
                                        else if (authfailedmembers.Count() > 0)
                                        {

                                            var user = (authfailedmembers.Count() == 1) ? authfailedmembers[0].EmployeeNumber.ToString() : authfailedmembers[0].EmployeeNumber.ToString() + " and " + authfailedmembers[1].EmployeeNumber.ToString();
                                            string message = "User " + user + " does not have a valid subscription";

                                            InvokeOnMainThread(() =>
                                            {

                                                var alert = new NSAlert();
                                                alert.AlertStyle = NSAlertStyle.Informational;
                                                alert.MessageText = "WBidMax";
                                                alert.InformativeText = "All bidders must have a WBidMax account .See the details.\n\n" + message;
                                                alert.AddButton("OK");

                                                alert.RunModal();
                                                this.View.Window.EndSheet(overlayPanel);
                                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                                overlayPanel.OrderOut(this.View.Window);

                                            });

                                        }
                                        else
                                        {
                                            InvokeOnMainThread(() =>
                                            {

                                                string buddy1=wBidStateContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim();
                                                string buddy2=wBidStateContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim();
                                                if(isChangeEmployee)
                                                {
                                                    GlobalSettings.TemporaryEmployeeBuddy = true;
                                                    isChangeEmployee = false;
                                                }
                                                GlobalSettings.isModified = true;
                                                if(isFaPositionChoiceWindow)
                                                {
                                                    GlobalSettings.SubmitBid.Buddy1 = (buddy1!=null && buddy1!=string.Empty && buddy1!="0")? buddy1:null;
                                                    GlobalSettings.SubmitBid.Buddy2 = (buddy2 != null && buddy2 != string.Empty && buddy2 != "0") ? buddy2: null;
                                                }
                                                this.View.Window.EndSheet(overlayPanel);
                                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                                overlayPanel.OrderOut(this.View.Window);

                                                NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
                                                CommonClass.Panel.OrderOut(this);

                                            });

                                        }
                                    }
                                }
                                else
                                {
                                    if (authfailedmembers.Count() > 0)
                                    {

                                        var user = (authfailedmembers.Count() == 1) ? authfailedmembers[0].EmployeeNumber.ToString() : authfailedmembers[0].EmployeeNumber.ToString() + " and " + authfailedmembers[1].EmployeeNumber.ToString();
                                        string message = "User " + user + " does not have a valid subscription";

                                        InvokeOnMainThread(() =>
                                        {

                                            var alert = new NSAlert();
                                            alert.AlertStyle = NSAlertStyle.Informational;
                                            alert.MessageText = "WBidMax";
                                            alert.InformativeText = "All bidders must have a WBidMax account .See the details.\n\n" + message;
                                            alert.AddButton("OK");

                                            alert.RunModal();
                                            this.View.Window.EndSheet(overlayPanel);
                                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                                            overlayPanel.OrderOut(this.View.Window);

                                        });

                                    }
                                    else
                                    {
                                        InvokeOnMainThread(() =>
                                        {

                                            string buddy1=wBidStateContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim();
                                            string buddy2=wBidStateContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim();
                                            GlobalSettings.isModified = true;
                                            if (isFaPositionChoiceWindow)
                                            {
                                                GlobalSettings.SubmitBid.Buddy1 = (buddy1 != null && buddy1 != string.Empty && buddy1 != "0") ? buddy1 : null;
                                                GlobalSettings.SubmitBid.Buddy2 = (buddy2 != null && buddy2 != string.Empty && buddy2 != "0") ? buddy2 : null;
                                            }
                                            this.View.Window.EndSheet(overlayPanel);
                                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                                            overlayPanel.OrderOut(this.View.Window);

                                            NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
                                            CommonClass.Panel.OrderOut(this);

                                        });

                                    }
                                }
                              
								
							}
							catch (Exception ex)
							{
                                wBidStateContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim();
                                wBidStateContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim();
                                GlobalSettings.isModified = true;
                                isChangeEmployee = false;
                                this.View.Window.EndSheet(overlayPanel);
                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                overlayPanel.OrderOut(this.View.Window);
                                NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
                                CommonClass.Panel.OrderOut(this);
                            }


                        }
						else
						{
							InvokeOnMainThread(() =>
							{
								var alert = new NSAlert();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								alert.InformativeText = "Internet Connectivity not available. Please try again";
								alert.AddButton("OK");

								alert.RunModal();
                                this.View.Window.EndSheet(overlayPanel);
                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                overlayPanel.OrderOut(this.View.Window);
                            });
						}
					}

				};
			}
			catch(Exception ex) {

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}


        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            if(GlobalSettings.IsSWAApiTest)
            {
                string employeeNumber = "";
                if(GlobalSettings.TemporaryEmployeeNumber==null || GlobalSettings.TemporaryEmployeeNumber ==string.Empty)
                {
                    GlobalSettings.TemporaryEmployeeNumber= (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) ? GlobalSettings.WbidUserContent.UserInformation.EmpNo : string.Empty;
                }

                GetBidderBuddyList(GlobalSettings.TemporaryEmployeeNumber);
            }
        }

        private void TxtBuddy2_Changed(object sender, EventArgs e)
		{
			if (EmployeeList != null)
			{
				if (txtBuddy2.StringValue != "0")
				{
					var buddyname2 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtBuddy2.StringValue).Value;
					lblBuddy2.StringValue = (buddyname2 == null || buddyname2.Name == string.Empty || buddyname2.Name == null) ? "< No Matching Element >" : buddyname2.Name;
					lblBuddy2Domicile.StringValue = (buddyname2 == null || buddyname2.Domicile == string.Empty || buddyname2.Domicile == null) ? "" : buddyname2.Domicile;
				}
				else
				{
					lblBuddy2.StringValue = "";
					lblBuddy2Domicile.StringValue = "";

                }

            }
        }

        private async void GetBidderBuddyList(string bidderEmpNum)
        {
            try
            {

                var bidderEmpList = await ServiceHelper.GetBuddyListFromSWAAsync(bidderEmpNum);
                if (bidderEmpList != null)
                {
                    if (!EmployeeBuddyList.ContainsKey(bidderEmpNum))
                    {
                        EmployeeBuddyList.Add(bidderEmpNum, bidderEmpList.BuddyIds);
                    }
                }
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
					var alert = new NSAlert();

                    alert.AlertStyle = NSAlertStyle.Warning;
                    alert.MessageText = "WBidMax";
                    alert.InformativeText = ex.Message;
                    alert.AddButton("OK");
					var res=alert.RunSheetModal(this.View.Window);
                    NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
                    CommonClass.Panel.OrderOut(this);
                });
                

            }
        }

        private void TxtBuddy1_Changed(object sender, EventArgs e)
        {
			if (EmployeeList != null)
			{
				if (txtBuddy1.StringValue != "0")
				{
					var buddyname1 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtBuddy1.StringValue).Value;
					lblBuddy1.StringValue = (buddyname1 == null || buddyname1.Name == string.Empty || buddyname1.Name == null) ? "< No Matching Element >" : buddyname1.Name;
					lblBudd1Domicile.StringValue = (buddyname1 == null || buddyname1.Domicile == string.Empty || buddyname1.Domicile == null) ? "" : buddyname1.Domicile;
				}
				else
				{
					lblBuddy1.StringValue = "";
					lblBudd1Domicile.StringValue = "";
                }
            }
        }

        //private async Task<(bool isValid,List<string> message)> SwaBuddyListCheck(EmployeeDetails employeeDetails)
        //{
        //          bool isValid=true;
        //	List<string> invalidBuddy = new List<string>();
        //          try
        //	{

        //              SwaBuddyList buddyList = new SwaBuddyList();
        //              foreach (var employee in employeeDetails.EmployeeNumbers)
        //		{

        //			if(buddyList==null || buddyList.BuddyIds.Count==0)
        //			{
        //                      buddyList = await ServiceHelper.GetBuddyListFromSWAAsync(GlobalSettings.TemporaryEmployeeNumber);
        //                  }
        //			bool isbuddy=buddyList.BuddyIds.Contains(employee.EmpNumber);
        //			isValid = isValid && isbuddy;
        //			if(!isbuddy)
        //			{
        //				invalidBuddy.Add(employee.EmpNumber);
        //			}
        //		}
        //	}
        //	catch(Exception ex)
        //	{
        //		invalidBuddy.Add(ex.Message);
        //		return (false,invalidBuddy);
        //	}
        //	return (isValid,invalidBuddy);
        //}

        private async Task<(bool isValid, List<string> message)> SwaBuddyListCheck(EmployeeDetails employeeDetails)
        {
            bool isValid = true;
            List<string> invalidBuddy = new List<string>();
            try
            {
                List<string> emplNumbers = employeeDetails.EmployeeNumbers.Select(x => x.EmpNumber).ToList();
                //emplNumbers.Insert(0, GlobalSettings.TemporaryEmployeeNumber);
                var taskList = emplNumbers;
                if (EmployeeBuddyList.Count > 0)
                {
                    taskList = taskList.Where(x => !EmployeeBuddyList.ContainsKey(x)).ToList();
                }
                var tasks = taskList.ToDictionary(id => id, id => ServiceHelper.GetBuddyListFromSWAAsync(id));
                if (tasks != null && tasks.Count != 0)
                {
                    await Task.WhenAll(tasks.Values);
                    var buddyies = tasks.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Result.BuddyIds.ToList());
                    foreach (var buddy in buddyies)
                    {
                        if (!EmployeeBuddyList.ContainsKey(buddy.Key))
                        {
                            EmployeeBuddyList.Add(buddy.Key, buddy.Value);
                        }
                    }
                }
                var bidderEmpNum = GlobalSettings.TemporaryEmployeeNumber;
                var bidderList = EmployeeBuddyList[bidderEmpNum];
                foreach (var employee in emplNumbers)
                {
                    var buddyEmpNumber = employee;
                    var empBuddyList = EmployeeBuddyList[buddyEmpNumber];
                    List<string> filteredEmployeeList = new List<string>();
                    bool isBuddy = (bidderList.Contains(buddyEmpNumber));
                    bool isReciprocal = (empBuddyList.Contains(bidderEmpNum));
                    isValid = isValid && isBuddy && isReciprocal;
                    if (!isBuddy)
                    {
                        string message = $"{buddyEmpNumber} not found in {bidderEmpNum} buddy list";
                        invalidBuddy.Add(message);
                    }
                    if (!isReciprocal)
                    {
                        string message = $"{bidderEmpNum} not found in {buddyEmpNumber} buddy list";
                        invalidBuddy.Add(message);
                    }
                }
            }
            catch (Exception ex)
            {
                invalidBuddy.Add(ex.Message);
                return (false, invalidBuddy);
            }
            return (isValid, invalidBuddy);
        }

        void CheckValidSubscriptionForEmployeesCompleted (object sender, CheckValidSubscriptionForEmployeesCompletedEventArgs e)
		{
			InvokeOnMainThread (() => {
				if (e.Result != null) {
					List<AuthStatusModel> authStatus = e.Result.ToList ();
					var authfailedmembers = authStatus.Where (x => x.IsValid == false).ToList ();

					if (authfailedmembers.Count () > 0) {
						string message = string.Empty;
						foreach (var item in authfailedmembers) {
							message += item.EmployeeNumber + " : " + item.Message + "\n\n";
						}

						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.MessageText = "WBidMax";
						alert.InformativeText = "All bidders must have a WBidMax account .See the details.\n\n" + message;
						alert.AddButton ("OK");

						alert.RunModal ();

					} else {


                        wBidStateContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim ();
                        wBidStateContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim ();
						GlobalSettings.isModified = true;
						//save the state of the INI File
						//WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

						NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
						CommonClass.Panel.OrderOut (this);
					
					}
				}
				this.View.Window.EndSheet (overlayPanel);
				NSApplication.SharedApplication.EndSheet (overlayPanel);
				overlayPanel.OrderOut (this.View.Window);
			});
		}
		private void LoadBidderDetails ()
		{
			if (GlobalSettings.ClearBuddyBid) {
				txtBuddy1.StringValue = txtBuddy2.StringValue = "0";
				lblBuddy1.StringValue = lblBuddy2.StringValue = "< No Matching Element >";
			} else {
				if (File.Exists (WBidHelper.GetAppDataPath () + "/falistwb4.json")) {
                    var jsondata = File.ReadAllText(WBidHelper.GetAppDataPath() + "/falistwb4.json");
                    EmployeeList = WBidCollection.ConvertJSonStringToObject<Dictionary<int, FalistData>>(jsondata);

					//txtBuddy1.StringValue = wBidStateContent.BuddyBids.Buddy1;
					//txtBuddy2.StringValue = wBidStateContent.BuddyBids.Buddy2;
					if (txtBuddy1.StringValue != "0")
					{
						var buddyname1 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtBuddy1.StringValue).Value;
						lblBuddy1.StringValue = (buddyname1 == null || buddyname1.Name == string.Empty || buddyname1.Name == null) ? "< No Matching Element >" : buddyname1.Name;
						lblBudd1Domicile.StringValue = (buddyname1 == null || buddyname1.Domicile == string.Empty || buddyname1.Domicile == null) ? "" : buddyname1.Domicile;
					}
					else
					{
						lblBuddy1.StringValue = "";
						lblBudd1Domicile.StringValue = "";
                    }

					if (txtBuddy2.StringValue != "0")
					{
						var buddyname2 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtBuddy2.StringValue).Value;
						lblBuddy2.StringValue = (buddyname2 == null || buddyname2.Name == string.Empty || buddyname2.Name == null) ? "< No Matching Element >" : buddyname2.Name;
						lblBuddy2Domicile.StringValue = (buddyname2 == null || buddyname2.Domicile == string.Empty || buddyname2.Domicile == null) ? "" : buddyname2.Domicile;
					}
					else
					{
						lblBuddy2.StringValue = "";
						lblBuddy2Domicile.StringValue = "";

                    }
                }
			}
		}

	}
    public class FalistData
    {
        public string Name { get; set; }
        public string Domicile { get; set; }

    }
}

