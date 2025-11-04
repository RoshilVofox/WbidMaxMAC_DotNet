using System;

using Foundation;
using AppKit;
using iText.Forms.Form.Element;
using ObjCRuntime;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class BidderVerificationWindowController : NSWindowController
	{
		public BidderVerificationWindowController (NativeHandle handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public BidderVerificationWindowController (NSCoder coder) : base (coder)
		{
		}

		public BidderVerificationWindowController () : base ("BidderVerificationWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
            lblWarningText.StringValue = $"We have detected that you ({CommonClass.UserName.Substring(1)}) are attempting to submit a bid for employee {GlobalSettings.SubmitBid.Bidder} ({GlobalSettings.TemporaryEmployeeName}).";
            btnCheckbox.State = NSCellStateValue.Off;
            btnCheckbox.Title = $"I certify I have permission from {GlobalSettings.SubmitBid.Bidder} ({GlobalSettings.TemporaryEmployeeName}) to submit a bid for them.";
            btnSubmitButton.Enabled = false;
            btnCheckbox.AttributedTitle = new NSAttributedString(btnCheckbox.Title, foregroundColor: NSColor.SystemRed); ;
			btnSubmitButton.Activated += HandleSubmitButton;
			btnCancel.Activated+= (sender, e) => {
                GlobalSettings.IsBidderCertified = false;
                NSApplication.SharedApplication.EndSheet(this.Window);
                this.Window.Close();
            };
			btnCheckbox.Activated += HandleCheckBox;

        }

        private void HandleCheckBox(object sender, EventArgs e)
        {
           if(btnCheckbox.State==NSCellStateValue.On)
			{
                var attributedTitle = new NSAttributedString(
                    btnCheckbox.Title,
                    foregroundColor: NSColor.SystemGreen
                );
                btnCheckbox.AttributedTitle = attributedTitle;
                btnSubmitButton.Enabled = true;
            }
			else
			{
                var attributedTitle = new NSAttributedString(
                    btnCheckbox.Title,
                    foregroundColor: NSColor.SystemRed
                );
                btnCheckbox.AttributedTitle = attributedTitle;
                btnSubmitButton.Enabled = false;
            }
        }

        private void HandleSubmitButton(object sender, EventArgs e)
        {
            GlobalSettings.IsBidderCertified = true;
            //this.Window.OrderOut(this);
            NSApplication.SharedApplication.EndSheet(this.Window);
            this.Window.Close();
            NSNotificationCenter.DefaultCenter.PostNotificationName("BidderCertify", null);
        }

        public new BidderVerificationWindow Window {
			get { return (BidderVerificationWindow) base.Window; }
		}
	}
}
