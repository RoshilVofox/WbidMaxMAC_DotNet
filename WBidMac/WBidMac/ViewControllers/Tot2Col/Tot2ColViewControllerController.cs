using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidMac.Mac.ViewControllers.Tot2Col
{
	public partial class Tot2ColViewControllerController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public Tot2ColViewControllerController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public Tot2ColViewControllerController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public Tot2ColViewControllerController () : base ("Tot2ColViewController", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new Tot2ColViewController View {
			get {
				return (Tot2ColViewController) base.View;
			}
		}
        public NSPanel panel;
        public string FromScreen;
        public override void AwakeFromNib()
        {
            try
            {
                base.AwakeFromNib();
                List<ColumnDefinition> Numerator = WBidCollection.GetRatioFeatureColumn();
                Numerator.Insert(0, new ColumnDefinition { Id = 0, DisplayName = "Select" });

                btnColum1.RemoveAllItems();
                btnColumn2.RemoveAllItems();
                btnColum1.AddItems(Numerator.Select(x => x.DisplayName).ToArray());
                btnColumn2.AddItems(Numerator.Select(x => x.DisplayName).ToArray());

                btnColum1.Activated += BtnColum1_Activated;
                btnColumn2.Activated += BtnColumn2_Activated;
                btnOk.Activated += BtnOk_Activated;
                btnCancel.Activated += BtnCancel_Activated;
                

            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        private void BtnCancel_Activated(object sender, EventArgs e)
        {
            NSApplication.SharedApplication.EndSheet(panel);
            panel.OrderOut(this);
            SendNotification("CANCEL-Tot2Col");
        }

        private void BtnOk_Activated(object sender, EventArgs e)
        {
            if (btnColum1.TitleOfSelectedItem != "Select" && btnColumn2.TitleOfSelectedItem != "Select")
            {

                if (GlobalSettings.WBidINIContent.Tot2ColValues == null)
                    GlobalSettings.WBidINIContent.Tot2ColValues = new WBidiPad.Model.Tot2Col();
                CalculateTot2ColValuesForAllLines();
                if (panel != null)
                {
                    NSApplication.SharedApplication.EndSheet(panel);
                    panel.OrderOut(this);
                }

                SendNotification("OK-Tot2Col");
            }
            else
            {


                var alert = new NSAlert();
                alert.AlertStyle = NSAlertStyle.Informational;
                alert.Window.Title = "WBidMax";
                alert.MessageText = "Either select Column 1 or Column 2 values - or click Cancel.";
                alert.AddButton("Ok");
                alert.RunModal();
                return;

            }
        }

        private void BtnColumn2_Activated(object sender, EventArgs e)
        {
           
        }

        private void BtnColum1_Activated(object sender, EventArgs e)
        {
           
        }


        void SendNotification(string StateData)
        {
            string notificaitonName = "";
            if (FromScreen == "BidLine")
            {
                notificaitonName = "BidLineRatioNotification";
            }
            else if (FromScreen == "Modern")
            {
                notificaitonName = "ModernRatioNotification";
            }
            else if (FromScreen == "Sort")
            {
                notificaitonName = "RatioNotification";
            }
            else if (FromScreen == "Summary")
            {
                notificaitonName = "SummaryRatioNotification";
            }

            NSNotificationCenter.DefaultCenter.PostNotificationName(notificaitonName, new Foundation.NSString(StateData));

        }

        private void CalculateTot2ColValuesForAllLines()
        {
            var col1 = WBidCollection.GetRatioFeatureColumn().FirstOrDefault(x => x.DisplayName == btnColum1.TitleOfSelectedItem);
            var col2 = WBidCollection.GetRatioFeatureColumn().FirstOrDefault(x => x.DisplayName == btnColumn2.TitleOfSelectedItem);
            if (col1 != null && col2 != null)
            {
                GlobalSettings.WBidINIContent.Tot2ColValues.Column1 = col1.Id;
                GlobalSettings.WBidINIContent.Tot2ColValues.Column2 = col2.Id;
                foreach (var line in GlobalSettings.Lines)
                {
                    var column1 = line.GetType().GetProperty(col1.DataPropertyName).GetValue(line, null);
                    if (col1.DataPropertyName == "TafbInBp")
                        column1 = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
                    decimal column1Value = Convert.ToDecimal(column1);

                    var column2 = line.GetType().GetProperty(col2.DataPropertyName).GetValue(line, null);
                    if (col2.DataPropertyName == "TafbInBp")
                        column2 = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
                    decimal col2Value = Convert.ToDecimal(column2);

                    line.Tot2Col = Math.Round(decimal.Parse(String.Format("{0:0.00}", column1Value + col2Value)), 2, MidpointRounding.AwayFromZero);


                }
            }
        }
    }
}
