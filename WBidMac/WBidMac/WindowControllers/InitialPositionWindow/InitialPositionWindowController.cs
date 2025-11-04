using ObjCRuntime;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

namespace WBidMac.WindowControllers.InitialPositionWindow;

public partial class InitialPositionWindowController : NSWindowController
{
    public string selectedRole = "";
    public InitialPositionWindowController(NativeHandle handle) : base(handle)
    {
    }
    [Export("initWithCoder:")]
    public InitialPositionWindowController(NSCoder coder) : base(coder)
    {
    }

    public InitialPositionWindowController() : base("InitialPositionWindowController")
    {
    }
    private string _lastEnter = string.Empty;
    public override void AwakeFromNib()
    {
        base.AwakeFromNib();
        btnSwaAPI.Hidden = true;
        btnSwaAPI.State = NSCellStateValue.Off;
        btnSwaAPI.Activated += (object sender, EventArgs e) => {
            GlobalSettings.IsSWAApiTest = (btnSwaAPI.State == NSCellStateValue.On);
        };

    }


    partial void btnFATapped(NSObject sender)
    {
        selectedRole = "FA";
        this.Window.Close();
        this.Window.OrderOut(this);
        NSApplication.SharedApplication.StopModal();
        NSNotificationCenter.DefaultCenter.PostNotificationName("RoleTapped", (NSString)selectedRole);
    }

    partial void btnPilotTapped(NSObject sender)
    {
        selectedRole = "Pilot";
        this.Window.Close();
        this.Window.OrderOut(this);
        NSApplication.SharedApplication.StopModal();
        NSNotificationCenter.DefaultCenter.PostNotificationName("RoleTapped", (NSString)selectedRole);
    }

    

    public override void KeyDown(NSEvent theEvent)
    {
        base.KeyDown(theEvent);
        if ((theEvent.ModifierFlags.HasFlag(NSEventModifierMask.ControlKeyMask)))
        {

            //NSKey.
            _lastEnter += theEvent.CharactersIgnoringModifiers.ToUpper();


            if (_lastEnter == "TE" || _lastEnter == "ET")
            {
                btnSwaAPI.Hidden = !btnSwaAPI.Hidden;
            }
            else if (_lastEnter != "F" && _lastEnter != "B" && _lastEnter != "T" && _lastEnter != "E")
            {
                _lastEnter = string.Empty;
            }

            //var a=10;

        }
        else
        {
            _lastEnter = string.Empty;
        }
    }

}

