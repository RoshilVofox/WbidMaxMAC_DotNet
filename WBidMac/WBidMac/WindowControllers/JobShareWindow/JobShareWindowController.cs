
using ADT.Common.Utility;
using MapKit;
using ObjCRuntime;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow;
using WBidDataDownloadAuthorizationService.Model;
using WBidMac.SwaApiModels;

namespace WBid.WBidMac.Mac.WindowControllers.JobShareWindow
{
	public partial class JobShareWindowController : NSWindowController
	{
        NSPanel overlayPanel;
        OverlayViewController overlay;
        public bool isChangeEmployee;
        public Dictionary<string, List<string>> EmployeeBuddyList { get; set; }
        public Dictionary<int, FalistData> EmployeeList { get; set; }
        public JobShareWindowController(NativeHandle handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public JobShareWindowController(NSCoder coder) : base (coder)
		{
		}

		public JobShareWindowController() : base ("JobShareWindow")
		{
		}
       
        public override void AwakeFromNib ()
		{

			try
			{
				btnCheckBox.State = GlobalSettings.isJobShareContingencyBid ? NSCellStateValue.On : NSCellStateValue.Off;
				txtJobShare1.StringValue = GlobalSettings.TemporaryEmployeeNumber;
				txtJobShare2.StringValue = GlobalSettings.JobShare2;
				txtJobShare1.Editable = false;
				txtJobShare1.Selectable = false;
                txtJobShare2.Changed += TxtJobShare2_Changed;
                EmployeeBuddyList = new Dictionary<string, List<string>>();
                LoadBidderDetails();
                btnCheckBox.Activated += (object sender, EventArgs e) =>
                {
                    if(btnCheckBox.State==NSCellStateValue.On)
                    {
                        GlobalSettings.isJobShareContingencyBid = true;
                    }
                    else
                    {
                        GlobalSettings.isJobShareContingencyBid = false;
                    }
                };

				btnCancel.Activated += (object sender, EventArgs e) =>
				{
                    
					this.Window.Close();
                    NSApplication.SharedApplication.EndSheet(this.Window);

				};

                this.Window.WillClose += (object sender, EventArgs e) =>
                {
                    NSApplication.SharedApplication.EndSheet(this.Window);
                };

				btnClearFields.Activated += (object sender, EventArgs e) =>
				{
					txtJobShare2.StringValue = "0";
					GlobalSettings.isJobShareContingencyBid = false;

				};

				btnOK.Activated += async (object sender, EventArgs e) =>
				{
                    EmployeeDetails empdetails = new EmployeeDetails();
                    empdetails.EmployeeNumbers = new Employee[2];
                    var obj = new List<Employee>();
                    if (string.IsNullOrEmpty(txtJobShare1.StringValue.Trim()))
                        txtJobShare1.StringValue = "0";
                    if (string.IsNullOrEmpty(txtJobShare2.StringValue.Trim()))
                        txtJobShare2.StringValue = "0";
                    LoadBidderDetails();

                    bool isInvalidBuddy = false;
                    string InvalidBuddyBidders = string.Empty;

                    if (txtJobShare2.StringValue != "0" && lblBuddyName.StringValue == "< No Matching Element >")
                    {
                        isInvalidBuddy = true;
                        if (InvalidBuddyBidders != string.Empty)
                            InvalidBuddyBidders += " , ";
                        InvalidBuddyBidders += txtJobShare2.StringValue;
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
                    if(txtJobShare1.StringValue==txtJobShare2.StringValue)
                    {
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Informational;
                        alert.MessageText = "WBidMax";
                        alert.InformativeText = "Job Share 1 and Job Share 2 cannot be the same";
                        alert.AddButton("OK");

                        alert.RunModal();
                        return;
                    }
                    if (lblBuddyDomicile.StringValue != "" && lblBuddyDomicile.StringValue != GlobalSettings.CurrentBidDetails.Domicile)
                    {


                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Informational;
                        alert.MessageText = "WBidMax";
                        alert.InformativeText = "One of the Buddy Bidders is NOT in " + GlobalSettings.CurrentBidDetails.Domicile;
                        alert.AddButton("OK");

                        alert.RunModal();
                        GlobalSettings.JobShare1 = "0";
                        GlobalSettings.JobShare2 = "0";
                        txtJobShare2.StringValue = "0";
                        lblBuddyDomicile.StringValue = "";
                        lblBuddyName.StringValue = "";
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



                    if ((!string.IsNullOrEmpty(txtJobShare1.StringValue.Trim())) && txtJobShare1.StringValue.Trim() != "0")
                    {
                        obj.Add(new Employee { EmpNumber = txtJobShare1.StringValue.Trim() });
                    }
                    if ((!string.IsNullOrEmpty(txtJobShare2.StringValue.Trim())) && txtJobShare2.StringValue.Trim() != "0")
                    {
                        obj.Add(new Employee { EmpNumber = txtJobShare2.StringValue.Trim() });
                    }
                    empdetails.EmployeeNumbers = obj.ToArray();

                    if (empdetails.EmployeeNumbers.Count() > 1)
                    {

                        if (Reachability.IsHostReachable("google.com"))
                        {

                            overlayPanel = new NSPanel();
                            overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                            overlay = new OverlayViewController();
                            overlay.OverlayText = "Checking Authorization for Job Share Bidders..";
                            overlayPanel.ContentView = overlay.View;
                            NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

                            try
                            {
                                empdetails.Platform = "PC";
                                var jsonData = ServiceUtils.JsonSerializer(empdetails);
                                StreamReader dr = ServiceUtils.PostDataToWbidCoreService("User/CheckValidSubscriptionForEmployees", jsonData);
                                List<AuthStatusModel> authStatus = WBidCollection.ConvertJSonStringToObject<List<AuthStatusModel>>(dr.ReadToEnd());

                                var authfailedmembers = authStatus.Where(x => x.IsValid == false).ToList();

                                if (WBidHelper.IsTokenExpired())
                                {
                                    this.Window.EndSheet(overlayPanel);
                                    NSApplication.SharedApplication.EndSheet(overlayPanel);
                                    overlayPanel.OrderOut(this.Window);
                                    InvokeOnMainThread(() =>
                                    {

                                        var alert = new NSAlert();
                                        alert.AlertStyle = NSAlertStyle.Informational;
                                        alert.MessageText = "WBidMax";
                                        alert.InformativeText = "Your session has expired,Please login again.";
                                        alert.AddButton("OK");
                                        var response = alert.RunModal();
                                        if (response == (nint)(NSAlertButtonReturn.First)) // OK button is typically the first button
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
                                    //SWA Buddy check
                                    var isValidBuddy = await SwaBuddyListCheck(empdetails);
                                    if (!isValidBuddy.isValid)
                                    {
                                        string message = "";

                                        if (isValidBuddy.message.FirstOrDefault().Contains("Failed to retrieve Buddy List"))
                                        {
                                            message = $"Failed to retrieve buddies for {GlobalSettings.TemporaryEmployeeNumber}";
                                        }
                                        else
                                        {
                                            message = (isValidBuddy.message.Count() == 1) ? $"The buddy {String.Join(',', isValidBuddy.message)} is not valid for {GlobalSettings.TemporaryEmployeeNumber}" : $"The buddies {String.Join(',', isValidBuddy.message)} are not valid for {GlobalSettings.TemporaryEmployeeNumber}";
                                        }
                                        InvokeOnMainThread(() =>
                                        {

                                            var alert = new NSAlert();
                                            alert.AlertStyle = NSAlertStyle.Informational;
                                            alert.MessageText = "WBidMax";
                                            alert.InformativeText = message;
                                            alert.AddButton("OK");

                                            alert.RunModal();
                                            this.Window.EndSheet(overlayPanel);
                                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                                            overlayPanel.OrderOut(this.Window);

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
                                            this.Window.EndSheet(overlayPanel);
                                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                                            overlayPanel.OrderOut(this.Window);

                                        });

                                    }
                                    else
                                    {
                                        InvokeOnMainThread(() =>
                                        {

                                           
                                            GlobalSettings.JobShare1 = txtJobShare1.StringValue.Trim();
                                            GlobalSettings.JobShare2 = txtJobShare2.StringValue.Trim();
                                            if(isChangeEmployee)
                                            {
                                                GlobalSettings.TemporaryEmployeeJobShare = true;
                                                isChangeEmployee = false;
                                            }
                                            GlobalSettings.isModified = true;
                                            NSNotificationCenter.DefaultCenter.PostNotificationName((NSString)"JobShareAdded", null);
                                            this.Window.EndSheet(overlayPanel);
                                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                                            overlayPanel.OrderOut(this.Window);

                                            NSApplication.SharedApplication.EndSheet(this.Window);
                                            

                                        });

                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                GlobalSettings.JobShare1 = txtJobShare1.StringValue.Trim();
                                GlobalSettings.JobShare2 = txtJobShare2.StringValue.Trim();
                                GlobalSettings.isModified = true;

                                this.Window.EndSheet(overlayPanel);
                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                overlayPanel.OrderOut(this.Window);

                                NSApplication.SharedApplication.EndSheet(this.Window);
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
                                this.Window.EndSheet(overlayPanel);
                                NSApplication.SharedApplication.EndSheet(overlayPanel);
                                overlayPanel.OrderOut(this.Window);
                            });
                        }
                    }
                };


			}
			catch (Exception ex)
			{

			}
            

        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            if (GlobalSettings.IsSWAApiTest)
            {
                GetBidderBuddyList(GlobalSettings.TemporaryEmployeeNumber);
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
                    var res=alert.RunSheetModal(this.Window);
                    NSApplication.SharedApplication.EndSheet(CommonClass.Panel);
                    CommonClass.Panel.OrderOut(this);
                    NSApplication.SharedApplication.EndSheet(this.Window);
                    this.Window.Close();
                });


            }
        }

        private async Task<(bool isValid, List<string> message)> SwaBuddyListCheck(EmployeeDetails employeeDetails)
        {
            bool isValid = true;
            List<string> invalidBuddy = new List<string>();
            try
            {
                List<string> emplNumbers = employeeDetails.EmployeeNumbers.Where(x => x.EmpNumber != GlobalSettings.TemporaryEmployeeNumber).Select(x => x.EmpNumber).ToList();
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

        private void TxtBuddy2_Changed(object sender, EventArgs e)
        {
            if (EmployeeList != null)
            {
                if (txtJobShare2.StringValue != "0")
                {
                    var buddyname2 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtJobShare2.StringValue).Value;
                    lblBuddyName.StringValue = (buddyname2 == null || buddyname2.Name == string.Empty || buddyname2.Name == null) ? "< No Matching Element >" : buddyname2.Name;
                    lblBuddyDomicile.StringValue = (buddyname2 == null || buddyname2.Domicile == string.Empty || buddyname2.Domicile == null) ? "" : buddyname2.Domicile;
                }
                else
                {
                    lblBuddyName.StringValue = "";
                    lblBuddyDomicile.StringValue = "";

                }

            }
        }

        private void LoadBidderDetails()
        {
            
                if (File.Exists(WBidHelper.GetAppDataPath() + "/falistwb4.json"))
                {
                    var jsondata = File.ReadAllText(WBidHelper.GetAppDataPath() + "/falistwb4.json");
                    EmployeeList = WBidCollection.ConvertJSonStringToObject<Dictionary<int, FalistData>>(jsondata);

                    if (txtJobShare2.StringValue != "0")
                    {
                        var buddyname2 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtJobShare2.StringValue).Value;
                        lblBuddyName.StringValue = (buddyname2 == null || buddyname2.Name == string.Empty || buddyname2.Name == null) ? "< No Matching Element >" : buddyname2.Name;
                        lblBuddyDomicile.StringValue = (buddyname2 == null || buddyname2.Domicile == string.Empty || buddyname2.Domicile == null) ? "" : buddyname2.Domicile;
                    }
                    else
                    {
                        lblBuddyName.StringValue = "";
                        lblBuddyDomicile.StringValue = "";

                    }
                }
        }

        private void TxtJobShare2_Changed(object sender, EventArgs e)
        {
            if (EmployeeList != null)
            {
                if (txtJobShare2.StringValue != "0")
                {
                    var buddyname2 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == txtJobShare2.StringValue).Value;
                    lblBuddyName.StringValue = (buddyname2 == null || buddyname2.Name == string.Empty || buddyname2.Name == null) ? "< No Matching Element >" : buddyname2.Name;
                    lblBuddyDomicile.StringValue = (buddyname2 == null || buddyname2.Domicile == string.Empty || buddyname2.Domicile == null) ? "" : buddyname2.Domicile;
                }
                else
                {
                    lblBuddyName.StringValue = "";
                    lblBuddyDomicile.StringValue = "";

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
