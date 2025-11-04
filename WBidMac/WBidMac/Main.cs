using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;
namespace WBid.WBidMac.Mac
{
	class MainClass
	{
		static void Main (string[] args)
		{
			try
			{

				NSApplication.Init();
				NSApplication.Main(args);
			}
			catch (Exception ex)
			{
				var s = ex.Message.ToString();
			}

           


        }
	}
}

