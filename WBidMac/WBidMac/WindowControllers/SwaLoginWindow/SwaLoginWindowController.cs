    using System;

using Foundation;
using AppKit;
using ObjCRuntime;
using ADT.Common.Utility;
using System.Web;
using WebKit;
using ScreenCaptureKit;
using System.Text.RegularExpressions;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidMac.Mac.ViewControllers.CustomAlertView;
using WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo;
using MapKit;

namespace WBid.WBidMac.Mac.WindowControllers.SwaLoginWindow
{
	public partial class SwaLoginWindowController : NSWindowController
	{
		public SwaLoginWindowController (NativeHandle handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public SwaLoginWindowController (NSCoder coder) : base (coder)
		{
		}

		public SwaLoginWindowController () : base ("SwaLoginWindow")
		{
		}
        private WKWebView _webView;
        private string codeVerifier;
        NSPanel overlayPanel;
        public bool isInitialLogin;
        public bool closeHome;
        public bool isLoginTapped;
        public bool isBidDownload { get; set; } = false;
        OverlayViewController overlay;
        NSObject _notif;
        public override void AwakeFromNib ()
		{
            
            base.AwakeFromNib();
            this.Window.SetContentSize(new CGSize(480,480));
            isLoginTapped = false;
            LoadWebView();
            Window.Title = "SWA Login";
            Window.WillClose += (sender, e) =>
            {
                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();
                NSApplication.SharedApplication.EndSheet(this.Window);
                if (isInitialLogin && closeHome)
                {
                    CommonClass.HomeController.Window.Close();
                    CommonClass.HomeController.Window.OrderOut(this);
                }
               if(!isLoginTapped)
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName("PKCE_loginSuccess", new NSString("Failed"));
                }

            };
            

        }
       

        public new SwaLoginWindow Window {
			get { return (SwaLoginWindow) base.Window; }
		}
        private void LoadWebView()
        {
            var apiService = new ApiService();
            codeVerifier = apiService.GenerateCodeVerifier();
            string codeChallenge = apiService.GenerateCodeChallenge(codeVerifier);

            var uriBuilder = new UriBuilder(APIConstants.AuthEndpoint);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? "");
            queryParams["response_type"] = "code";
            queryParams["client_id"] = APIConstants.ClientID;
            queryParams["redirect_uri"] = APIConstants.RedirectUri;
            queryParams["scope"] = "openid email profile address phone";
            queryParams["code_challenge"] = codeChallenge;
            queryParams["code_challenge_method"] = "S256";
            queryParams["prompt"] = "login";
            uriBuilder.Query = queryParams.ToString();

            var authUri = uriBuilder.Uri;
            var request = new NSUrlRequest(new NSUrl(authUri.AbsoluteUri));
            if(isInitialLogin)
            {
                closeHome = true;
            }
            var config = new WKWebViewConfiguration();
            _webView = new WKWebView(Window.ContentView.Frame, config);
            Window.ContentView.AddSubview(_webView);
            _webView.NavigationDelegate = new WebViewNavigationDelegate(OnRedirectUrlReceived,this);
            _webView.LoadRequest(request);
        }

       

        public class WebViewNavigationDelegate : WKNavigationDelegate
        {
            private readonly Action<string> _onRedirect;
            private readonly SwaLoginWindowController _swaLoginWindow;

            public WebViewNavigationDelegate(Action<string> onRedirect,SwaLoginWindowController swaLoginWindow)
            {
                _onRedirect = onRedirect;
                _swaLoginWindow = swaLoginWindow;
            }

            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                var url = navigationAction.Request.Url.AbsoluteString;

                if (url.StartsWith(APIConstants.RedirectUri))
                {
                    _onRedirect?.Invoke(url);
                    decisionHandler(WKNavigationActionPolicy.Cancel); // prevent loading it
                }
                else
                {
                    decisionHandler(WKNavigationActionPolicy.Allow);
                }
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                //webView.EvaluateJavaScript("document.body.innerHTML", (result, error) =>
                //{
                //    if (error != null)
                //    {
                //        return;
                //    }

                //    string htmlContent = result?.ToString();
                //    if(htmlContent.Contains("We didn't recognize the username or password you entered"))
                //    {
                //        WBidLogEvent objlogs = new WBidLogEvent();
                //        objlogs.LogBadPasswordUsage("",false,"");
                //    }
                //});
                Task.Run(() =>
                {
                    try
                    {

                        webView.EvaluateJavaScript("document.querySelector('.ping-error')?.innerText", (result, error) =>
                                    {
                                        if (error != null) return;

                                        string errorMessage = result?.ToString();
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            webView.EvaluateJavaScript("document.getElementById('username').value", (res, err) =>
                                            {
                                                if (err == null)
                                                {
                                                    string username = res?.ToString();
                                                    if (!string.IsNullOrEmpty(username))
                                                    {
                                                        username=username.Replace("e", "").Replace("x", "");
                                                        WBidLogEvent wBidLog = new WBidLogEvent();
                                                        wBidLog.LogBadPasswordUsageNewAPI(username, _swaLoginWindow.isInitialLogin, _swaLoginWindow.isBidDownload, errorMessage);

                                                    }
                                                }
                                            });
                                        }
                                    });
                    }
                    catch (Exception ex)
                    {

                    }
                });
                
            }
        }


        private async void OnRedirectUrlReceived(string url)
        {
            var uri = new Uri(url);
            var code = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("code");
            isLoginTapped = true;
            if (!string.IsNullOrEmpty(code))
            {
                closeHome = false;

                var apiService = new ApiService();
                var res=await apiService.ExchangeCodeForToken(code, codeVerifier);
                if(res.isFAlogin)
                {
                    if (isInitialLogin)
                    {
                        //await apiService.ExchangeCodeForToken(code, codeVerifier, isBuddyCheck);
                        overlayPanel = new NSPanel();
                        overlayPanel.SetContentSize(new CGSize(400, 120));
                        overlay = new OverlayViewController();
                        overlay.OverlayText = "Authenticating..";
                        overlayPanel.ContentView = overlay.View;
                        NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);
                        AuthenticationCheck();
                        Window.Close();
                    }
                    else
                    {
                        Window.Close();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("PKCE_loginSuccess", new NSString("Success"));
                        //await apiService.ExchangeCodeForToken(code, codeVerifier, isBuddyCheck);
                    }
                    //Window.Close();
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        var alert = new NSAlert
                        {
                            AlertStyle = NSAlertStyle.Informational,
                            MessageText = "WbidMax",
                            InformativeText = "You are attempting to log in with Pilot credentials. Please use valid Flight Attendant credentials instead.",
                        };
                        alert.AddButton("OK");

                        var result = alert.RunSheetModal(Window);

                        if (result == 1000) // First button returns 1000
                        {
                            LoadWebView(); //reload Web View
                        }
                    });
                }

                
            }
            else
            {
                var alert = new NSAlert()
                {
                    AlertStyle = NSAlertStyle.Informational,
                    MessageText = "WbidMax",
                    InformativeText = "SWA Login Error,please try login again or try after some time"
                };
                alert.AddButton("OK");
                var result = alert.RunSheetModal(Window);
                if(result==1000)
                {
                    LoadWebView();
                }
            }
        }

        private void UserUpdationNotification(NSNotification ns)
        {
            if(_notif!=null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_notif);
                if(ns.Object.ToString()=="Success" || ns.Object.ToString()=="Cancel")
                {
                    closeHome = false;
                }
                else
                {
                    closeHome = true;
                }
                
            }
            
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


               
                    try
                    {
                        _notif=NSNotificationCenter.DefaultCenter.AddObserver((NSString)"UserUpdation", UserUpdationNotification);
                        string empNo = Regex.Replace(userName, "[^0-9]+", string.Empty);
                        string url = "GetEmployeeDetails/" + empNo + "/4";
                        StreamReader dr = ServiceUtils.GetRestData(url);
                        WBidDataDownloadAuthorizationService.Model.UserInformation responseData = ServiceUtils.ConvertJSonToObject<WBidDataDownloadAuthorizationService.Model.UserInformation>(dr.ReadToEnd());
                        checkUserDetails(responseData, empNo);

                    }
                    catch (Exception ex)
                    {
                        CommonClass.AppDelegate.ErrorLog(ex);
                        CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                    }


             
            }
        }
        //private void client_GetEmployeeDetailsAsyncCompleted(object sender, GetEmployeeDetailsCompletedEventArgs e)
        //{
        private void checkUserDetails(WBidDataDownloadAuthorizationService.Model.UserInformation userData, string empNum)
        {
            try
            {
                //if (e.Result != null && e.Result.EmpNum != 0 && e.Result.EmpNum.ToString() != string.Empty)
                if (userData != null && userData.EmpNum != 0 && userData.EmpNum.ToString() != string.Empty)
                {
                    var userinfo = userData;
                    WbidUser wbidUser = new WbidUser();
                    wbidUser.UserInformation = new UserInformation();
                    wbidUser.UserInformation.FirstName = userinfo.FirstName;
                    wbidUser.UserInformation.LastName = userinfo.LastName;
                    wbidUser.UserInformation.Email = userinfo.Email;

                    wbidUser.UserInformation.EmpNo = userinfo.EmpNum.ToString();
                    wbidUser.UserInformation.CellNumber = userinfo.CellPhone;
                    wbidUser.UserInformation.AcceptMail = userinfo.AcceptEmail;
                    // wbidUser.UserInformation.Position =userinfo.Position;
                    if (userinfo.Position == 4)
                    {
                        wbidUser.UserInformation.Position = "Pilot";
                    }
                    else if (userinfo.Position == 3)
                    {
                        wbidUser.UserInformation.Position = "FA";
                    }
                    if (GlobalSettings.WbidUserContent != null)
                    {
                        wbidUser.RecentFiles = GlobalSettings.WbidUserContent.RecentFiles;
                    }

                    wbidUser.isUserFromDB = true;
                    wbidUser.UserInformation.IsFree = userinfo.IsFree;
                    wbidUser.UserInformation.PaidUntilDate = userinfo.WBExpirationDate ?? DateTime.MinValue; //userinfo.WBExpirationDate ?? DateTime.MinValue;
                    wbidUser.UserInformation.IsMonthlySubscribed = userinfo.IsMonthlySubscribed;
                    wbidUser.UserInformation.IsYearlySubscribed = userinfo.IsYearlySubscribed;
                    wbidUser.UserInformation.SecondSubscriptionLine = userinfo.SecondSubscriptionLine;
                    wbidUser.UserInformation.TopSubscriptionLine = userinfo.TopSubscriptionLine;
                    wbidUser.UserInformation.ThirdSubscriptionLine = userinfo.ThirdSubscriptionLine;
                    wbidUser.UserAccountDateTimeField = userinfo.UserAccountDateTime;
                    if (GlobalSettings.isVFXServer)
                        wbidUser.IsVFXServer = true;
                    GlobalSettings.WbidUserContent = wbidUser;

                    WBidHelper.SaveUserFile(wbidUser, WBidHelper.WBidUserFilePath);

                    InvokeOnMainThread(() =>
                    {
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();


                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        overlayPanel.OrderOut(this);
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Warning;
                        alert.MessageText = "Great";
                        alert.InformativeText = "We found a previous account from WBidMax.\nWe have imported those settings. Please verify the settings and change as needed!!";

                        alert.RunModal();
                        NSApplication.SharedApplication.EndSheet(overlayPanel);


                        var userUpdation = new UserUpdateInfoController();
                        userUpdation.authenticatedUserId = empNum;
                        userUpdation.title = "User Update Info";
                        userUpdation.buttonName = "Update";
                        userUpdation.isRegister = false;
                        userUpdation.Window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = false;
                        CommonClass.HomeController.Window.AddChildWindow(userUpdation.Window, NSWindowOrderingMode.Above);
                        userUpdation.Window.MakeKeyAndOrderFront(this);
                        NSApplication.SharedApplication.RunModalForWindow(userUpdation.Window);

                    });

                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();
                        var userReg = new UserUpdateInfoController();
                        userReg.authenticatedUserId = empNum;
                        userReg.title = "User Registration";
                        userReg.buttonName = "Register";
                        userReg.isRegister = true;
                        userReg.isFromSwaLogin = true;
                        userReg.Window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = false;
                        CommonClass.HomeController.Window.AddChildWindow(userReg.Window, NSWindowOrderingMode.Above);
                        userReg.Window.MakeKeyAndOrderFront(this);
                        NSApplication.SharedApplication.RunModalForWindow(userReg.Window);

                    });
                }

            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }
    }
}
