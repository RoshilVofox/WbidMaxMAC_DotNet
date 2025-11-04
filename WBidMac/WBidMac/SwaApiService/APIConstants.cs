using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;

namespace ADT.Common.Utility
{

    public static class APIConstants
    {
  
        #region SWA API Constants

        //Base URL
        public static string BaseURL => GlobalSettings.SwaEnvironmentType switch
        {
            (int)SwaEnviromentType.QA => "https://service.east.0.crewbid.qa.swalife.com/itest",
            (int)SwaEnviromentType.DEV => "https://itest.service.east.0.crewbid.dev.swalife.com",
            (int)SwaEnviromentType.PROD => "https://service.crewbid.swalife.com/golden",
            _ => throw new NotSupportedException()
        };
        //Base URL



        //Token API
        public static string AuthEndpoint => GlobalSettings.SwaEnvironmentType switch
        {
            (int)SwaEnviromentType.QA => "https://sso.fed.qa.aws.swalife.com/as/authorization.oauth2",
            (int)SwaEnviromentType.DEV => "https://sso.fed.dev.aws.swalife.com/as/authorization.oauth2",
            (int)SwaEnviromentType.PROD => "https://sso.fed.prod.aws.swalife.com/as/authorization.oauth2", 
            _ => throw new NotSupportedException()
        };

        public static string OAuthTokenRequestAPI => GlobalSettings.SwaEnvironmentType switch
        {
            (int)SwaEnviromentType.QA => "https://sso.fed.qa.aws.swalife.com/as/token.oauth2",
            (int)SwaEnviromentType.DEV => "https://sso.fed.dev.aws.swalife.com/as/token.oauth2",
            (int)SwaEnviromentType.PROD => "https://sso.fed.prod.aws.swalife.com/as/token.oauth2",
            _ => throw new NotSupportedException()
        };

        public static string TokenAud => GlobalSettings.SwaEnvironmentType switch
        {
            (int)SwaEnviromentType.QA => "aud://sso.fed.qa.aws.swacorp.com/cbna",
            (int)SwaEnviromentType.DEV => "aud://sso.fed.dev.aws.swacorp.com/cbna",
            (int)SwaEnviromentType.PROD => "aud://sso.fed.prod.aws.swacorp.com/cbna",
            _ => throw new NotSupportedException()
        };

        public static string ClientID => "p502838";
        public static string TokenAudience1 => "p502552";
        public static string TokenAudience2 => "p502553";



        //public static string RedirectUri => "https://crewbidapp.com/callback";
        public static string RedirectUri => "com.crewbid-wbidmax://callback";

        ////Redirect URL
        //public static string RedirectUri => GlobalSettings.SwaEnvironmentType switch
        //{
        //    (int)SwaEnviromentType.QA => "https://wbidmaxapp.com/callback",
        //    (int)SwaEnviromentType.DEV => "https://wbidmaxapp.com/callback",
        //    (int)SwaEnviromentType.PROD => "https://wbidmaxapp.com/home"
        //};


        //public static string BaseApiPath => GlobalSettings.SwaEnvironmentType switch
        //{
        //    (int)SwaEnviromentType.QA => "itest",
        //    (int)SwaEnviromentType.DEV => "itest",
        //    (int)SwaEnviromentType.PROD => "golden",
        //    _ => throw new NotSupportedException()
        //};

        //Service API
        public static string BidAwardRetrievalForADomicileAPI => $"{BaseURL}/if-line-base-auction/bid-round";
        public static string CoverLetterRetrievalForADomicileAPI => $"{BaseURL}/if-line-base-auction/bid-round";
        public static string BidSubmitAPI => $"{BaseURL}/if-line-base-auction/bid-round";
        public static string BuddyListAPI => $"{BaseURL}/if-line-base-auction/buddies";
        public static string LineretrivalAPI => $"{BaseURL}/lines-pairings/lines/bid-round";
        public static string PairingretrivalAPI => $"{BaseURL}/lines-pairings/pairings/bid-round";
        public static string SeniriotyRetrievalForADomicileAPI => $"{BaseURL}/if-line-base-auction/bid-round";
        //Service API                                                                       

        #endregion


    }
}

