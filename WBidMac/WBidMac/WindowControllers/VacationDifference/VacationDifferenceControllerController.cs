using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using WBid.WBidiPad.Core;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary;
using System.Linq;
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.VacationDifference
{
    public partial class VacationDifferenceControllerController : NSWindowController
    {
        public List<VacationValueDifferenceOutputDTO> lstVacationDifferencedata { get; set; }
        public List<FlightDataChangeVacValues> lstFlightDataChangevalues { get; set; }
        public List<FlightDataChangeVacValues> lstFinalResult { get; set; }
        public bool IsNeedToClose { get; set; }
        VacationValueDifferenceInputDTO input;
        NSPanel overlayPanel;
        OverlayViewController overlay;
        public VacationDifferenceControllerController(NativeHandle handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public VacationDifferenceControllerController(NSCoder coder) : base(coder)
        {
        }

        public VacationDifferenceControllerController() : base("VacationDifferenceController")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Window.WillClose += delegate
            {
                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();
            };
            this.btnUpdate.Activated += BtnUpdate_Activated;
            
        }

        private void BtnUpdate_Activated(object sender, EventArgs e)
        {
            bool isConnectionAvailable = Reachability.CheckVPSAvailable();
            if (isConnectionAvailable)
            {
                overlayPanel = new NSPanel();
                overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                overlay = new OverlayViewController();
                overlay.OverlayText = "Updating Bid data..Please wait";
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);


                DownloadBidWindowController obj = new DownloadBidWindowController();
                bool isSuccess = obj.RedownloadBidData();

                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();

                string alerttext = string.Empty;
                if (isSuccess)
                    alerttext = "Successfully Updated Bid data. Please check .";
                else
                    alerttext = "There is an error while updating the bid data. Please check";
                InvokeOnMainThread(() =>
                {
                    NSApplication.SharedApplication.EndSheet(overlayPanel);
                    overlayPanel.OrderOut(this);

                    var alert = new NSAlert();
                    alert.MessageText = "WBidMax";
                    alert.InformativeText = alerttext;
                    alert.AddButton("OK");


                    alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                    {

                        alert.Window.Close();
                        NSApplication.SharedApplication.StopModal();
                        
                        if (CommonClass.SummaryController != null)
                        {
                            CommonClass.SummaryController.TableViewReload();
                        }
                        if (CommonClass.ModernController != null)
                        {
                            CommonClass.ModernController.ReloadContent();
                        }
                        if (CommonClass.BidLineController != null)
                        {
                            CommonClass.BidLineController.ReloadContent();
                        }
                    };
                    alert.RunModal();
                });
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
                    alert.MessageText = "Bid Data Update";
                    alert.InformativeText = alertmessage;
                    alert.AddButton("OK");
                    alert.RunModal();

                });
            }
        }

        public void GetVacationDifffrenceData()
        {
            bool isConnectionAvailable = Reachability.CheckVPSAvailable();

            if (isConnectionAvailable)
            {
                lstFinalResult = new List<FlightDataChangeVacValues>();
                input = new VacationValueDifferenceInputDTO();

                input.BidDetails = new UserBidDetails();
                input.BidDetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                input.BidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                input.BidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "M" ? 1 : 2;
                input.BidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
                input.BidDetails.Month = GlobalSettings.CurrentBidDetails.Month;


                input.IsDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
                input.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
                input.IsVAC = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
                input.FAEOMStartDate = GlobalSettings.FAEOMStartDate.Date.Day;
                input.FromApp = (int)WBid.WBidiPad.Core.Enum.FromApp.WbidmaxMACApp;
                input.lstVacation = new List<VacationInfo>();

                //input.lstVacation.Add(new VacationInfo { Type = "FV", VacDate = "05/29-06/04" });
                var vavacation = GlobalSettings.WBidStateCollection.Vacation;
                if (vavacation != null && vavacation.Count > 0)
                {
                    foreach (var item in vavacation)
                    {
                        var startdate = Convert.ToDateTime(item.StartDate);
                        var enddate = Convert.ToDateTime(item.EndDate);
                        var vacationstring = startdate.Month + "/" + startdate.Day + "-" + enddate.Month + "/" + enddate.Day;
                        input.lstVacation.Add(new VacationInfo { Type = "VA", VacDate = vacationstring });

                    }
                }
                var Fvvavacation = GlobalSettings.WBidStateCollection.FVVacation;
                if (Fvvavacation != null && Fvvavacation.Count > 0)
                {
                    foreach (var item in Fvvavacation)
                    {
                        var vacationstring = item.StartAbsenceDate.Month + "/" + item.StartAbsenceDate.Day + "-" + item.EndAbsenceDate.Month + "/" + item.EndAbsenceDate.Day;
                        input.lstVacation.Add(new VacationInfo { Type = item.AbsenceType, VacDate = vacationstring });
                    }
                }
                var jsonData = ServiceUtils.JsonSerializer(input);
                jsonData = jsonData.Replace("\"__type\":\"VacationInfo:#WBid.WBidiPad.Model\",", "");
                StreamReader dr = ServiceUtils.GetRestData("GetVacationDifferenceData", jsonData);
                lstVacationDifferencedata = WBidCollection.ConvertJSonStringToObject<List<VacationValueDifferenceOutputDTO>>(dr.ReadToEnd());
                if (lstVacationDifferencedata.Count > 0)
                {
                    lstFlightDataChangevalues = lstVacationDifferencedata.FirstOrDefault().lstFlightDataChangeVacValues;
                    
                    foreach (var item in lstFlightDataChangevalues)
                    {
                        var lineobj = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == item.LineNum);
                        if (lineobj != null)
                        {
                            item.OldTotalPay = lineobj.Tfp;
                            item.OldVPCu = Decimal.Round(lineobj.VacPayCuBp, 2);
                            item.OldVPNe = Decimal.Round(lineobj.VacPayNeBp, 2);
                            item.OldVPBo = Decimal.Round(lineobj.VacPayBothBp, 2);
                            if (item.OldTotalPay != item.NewTotalPay || item.NewVPBo != item.OldVPBo || item.NewVPCu != item.OldVPCu || item.NewVPNe != item.OldVPNe)
                            {
                                lstFinalResult.Add(item);
                            }
                        }
                    }
                    tblVacationDiff.Source = new VacationDifferenceTableSource(this);
                }
                else
                {
                    IsNeedToClose = true;
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        ShowMessageBox("WBidMax", "There are no differences in pay for your vacation with the new Flight Data.");
                    }
                    else
                    {
                       
                        //InvokeOnMainThread(() =>
                        //{
                        //    this.Window.Close();
                        //    this.Window.OrderOut(this);
                        //    NSApplication.SharedApplication.StopModal();
                        //});
                        ShowMessageBox("WBidMax", "There are no differences in pay with the new Flight Data. But if you have vacation, please turn ON vacation and check the vacation difference.");
                    }

                }
            }
            else
            {
                IsNeedToClose = true;
                string alertmessage = GlobalSettings.VPSDownAlert;
                if (Reachability.IsSouthWestWifiOr2wire())
                {
                    alertmessage = GlobalSettings.SouthWestConnectionAlert;
                }
                ShowMessageBox("WBidMax", alertmessage);
            }
        }

        private void ShowMessageBox(string title, string content)
        {
            InvokeOnMainThread(() =>
            {
            var alert = new NSAlert();
            alert.MessageText = title;
            alert.InformativeText = content;
            alert.AddButton("OK");
          
           
            alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
            {
                
                    alert.Window.Close();
                    this.Window.Close();
                    this.Window.OrderOut(this);
                    NSApplication.SharedApplication.StopModal();
                
                
            };
            alert.RunModal();
            });
        }
        public new VacationDifferenceController Window
        {
            get { return (VacationDifferenceController)base.Window; }
        }


        public partial class VacationDifferenceTableSource : NSTableViewSource
        {
            VacationDifferenceControllerController vacationdifferenceVC;

            public VacationDifferenceTableSource(VacationDifferenceControllerController show)
            {
                vacationdifferenceVC = show;
            }

            public override nint GetRowCount(NSTableView tableView)
            {
                return vacationdifferenceVC.lstFinalResult.Count;
            }

            public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
            {
                if (tableColumn.Identifier == "linenum")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].LineNum.ToString();

                }
                else if (tableColumn.Identifier == "oldtotpay")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].OldTotalPay.ToString();

                }
                else if (tableColumn.Identifier == "newtotpay")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].NewTotalPay.ToString();

                }
                else if (tableColumn.Identifier == "oldvacpaycu")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].OldVPCu .ToString();

                }
                else if (tableColumn.Identifier == "newvacpaycu")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].NewVPCu.ToString();

                }
                else if (tableColumn.Identifier == "oldvacpayne")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].OldVPNe.ToString();

                }
                else if (tableColumn.Identifier == "newvacpayne")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].NewVPNe.ToString();

                }
                else if (tableColumn.Identifier == "oldvacpaybo")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].OldVPBo.ToString();

                }
                else if (tableColumn.Identifier == "newvacpaybo")
                {
                    return (NSString)vacationdifferenceVC.lstFinalResult[(int)row].NewVPBo.ToString();

                }
                
                return new NSString();
            }

            public override void SelectionDidChange(NSNotification notification)
            {

               
            }
        }

    }
}
