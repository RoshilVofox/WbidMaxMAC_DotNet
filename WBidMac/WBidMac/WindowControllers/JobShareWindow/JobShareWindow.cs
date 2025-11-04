
using ObjCRuntime;

namespace WBid.WBidMac.Mac.WindowControllers.JobShareWindow
{
	public partial class JobShareWindow : NSWindow
	{
		public JobShareWindow (NativeHandle handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public JobShareWindow(NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
