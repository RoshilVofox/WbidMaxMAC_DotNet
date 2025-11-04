using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADT.Common.Utility
{

    public static class APIConstants
    {
        #region SWA Constants
        public const string OAuthAuthenticationAPI = "";
        public const string OAuthTokenRequestAPI = "https://sso.fed.dev.aws.swalife.com/as/token.oauth2";
        public const string  LineretrivalAPI = "https://service.crewbid.dev.swalife.com/itest/lines-pairings/lines/bid-round";
        public const string PairingretrivalAPI = "https://service.crewbid.dev.swalife.com/itest/lines-pairings/pairings/bid-round";
        public const string SeniriotyRetrievalForADomicileAPI = "https://service.crewbid.dev.swalife.com/itest/if-line-base-auction/bid-round";
        public const string CoverLetterRetrievalForADomicileAPI = "https://service.crewbid.dev.swalife.com/itest/if-line-base-auction/bid-round";
        public const string AuthEndpoint = "https://sso.fed.dev.aws.swalife.com/as/authorization.oauth2";
        public const string RedirectUri = "com.crewbid-wbidmax://callback";

        #endregion
    }
}
