
using iOSPasswordStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.SwaApiModels;
using WBid.WBidMac.Mac;
using WBidMac.SwaApiModels;
using WebKit;

namespace ADT.Common.Utility
{
    public class ApiService
    {
        private readonly RestClient _client;
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://sso.fed.dev.aws.swalife.com/")
        };
        public ApiService()
        {

        }
        public ApiService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
        }


        

        public async Task<RestResponse> GetDataAsync(Dictionary<string, string> headers, Dictionary<string, object> queryParams)
        {
            var request = new RestRequest("", Method.Get);

          

            // Add query parameters dynamically
            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    request.AddParameter(param.Key, param.Value?.ToString(), ParameterType.QueryString);
                }
            }

            // Add headers dynamically
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            return await _client.ExecuteGetAsync(request);
        }

        public async Task<RestResponse> PostDataAsync(Dictionary<string, string> headers, string body)
        {
            var request = new RestRequest("", Method.Post);

            // Add headers dynamically
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            // Add body if not null
            if (body != null)
            {
                request.AddStringBody(body, DataFormat.Json);
            }

            return await _client.ExecutePostAsync(request);
        }

        public  async Task<List<TItem>> GetPaginatedDataAsync<TResponse, TItem>(Dictionary<string, string> headers,Dictionary<string, object> queryParams,Func<TResponse, List<TItem>> extractItems)
        {
            int page = 0;
            int pageSize = 500;
            List<TItem> allData = new List<TItem>();

            
            RestResponse response = await FetchApiResponseAsync(_client, page, pageSize, headers, queryParams); // Initial Fetch

            if (!response.IsSuccessful || response.Content == null)
            {
                throw new Exception($"API Error : {response.StatusCode} - {response.Content}");
            }

            var paginatedResponse = JsonConvert.DeserializeObject<TResponse>(response.Content);

            if (paginatedResponse != null)
            {
                allData.AddRange(extractItems(paginatedResponse));
            }

            if (paginatedResponse is not IPaginatedResponse paginated || !paginated.HasNextPage)
            {
                return allData;
            }

            var pageObj = paginated.Page;
      
            int totalPages = 1;
            if (pageObj != null)
            {
                var pageJObject = JObject.FromObject(pageObj);
                totalPages = pageJObject["totalPages"]?.Value<int>() ?? 1;
            }


            var tasks = new List<Task<RestResponse>>();
            for (int p = 1; p < totalPages; p++) // Fetching remaining pages.Page 0 already fetched.
            {
                int pageIndex = p; 
                tasks.Add(FetchApiResponseAsync(_client, pageIndex, pageSize, headers, queryParams));
            }

            var responses = await Task.WhenAll(tasks);

            foreach (var resp in responses)
            {
                if (resp.IsSuccessful && resp.Content != null)
                {
                    var pageResponse = JsonConvert.DeserializeObject<TResponse>(resp.Content);
                    if (pageResponse != null)
                    {
                        allData.AddRange(extractItems(pageResponse));
                    }
                }
                else
                {
                    throw new Exception($"API Error : {resp.StatusCode} - {resp.Content}");
                }
            }

            return allData;
        }
        private async static Task<RestResponse> FetchApiResponseAsync(RestClient client, int page, int pageSize, Dictionary<string, string> headers,Dictionary<string, object> queryParams)
        {
            var request = new RestRequest("", Method.Get);

            // Add pagination parameters
            request.AddParameter("page", page);
            request.AddParameter("size", pageSize);

            // Add query parameters dynamically
            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    request.AddParameter(param.Key, param.Value?.ToString(), ParameterType.QueryString);
                }
            }

            // Add headers dynamically
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            return await client.ExecuteGetAsync(request);
        }

       
        public string GenerateCodeVerifier()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var random = new Random();
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
            {
                nonce[i] = chars[random.Next(chars.Length)];
            }

            return new string(nonce);
        }

        public string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }
        public async Task<(string accessToken,bool isFAlogin)> ExchangeCodeForToken(string code,string coderVerifier)
        {
            var tokenUrl = APIConstants.OAuthTokenRequestAPI;
            var clientId = APIConstants.ClientID;
            var redirectUri = APIConstants.RedirectUri;
            var httpClient = new System.Net.Http.HttpClient();

            var postData = new List<KeyValuePair<string, string>>
{
    new KeyValuePair<string, string>("grant_type", "authorization_code"),
    new KeyValuePair<string, string>("code", code),
    new KeyValuePair<string, string>("redirect_uri", redirectUri),
    new KeyValuePair<string, string>("client_id", clientId),
    new KeyValuePair<string, string>("code_verifier", coderVerifier), // Include the code verifier
    new KeyValuePair<string, string>("aud", $"{APIConstants.TokenAud}/{APIConstants.TokenAudience1}"),
     new KeyValuePair<string, string>("aud", $"{APIConstants.TokenAud}/{APIConstants.TokenAudience2}")
};


            var content = new System.Net.Http.FormUrlEncodedContent(postData);
            var response = await httpClient.PostAsync(tokenUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(responseString);
            string accessToken = jsonObject["access_token"].ToString();
            int expiresInSeconds = (int)jsonObject["expires_in"];
            GlobalSettings.SwaAccessToken = accessToken;
            GlobalSettings.SwaTokenExpiry = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);

            var handler = new JwtSecurityTokenHandler();
            bool isFAlogin = false;
            if (handler.CanReadToken(accessToken))
            {
                var token = handler.ReadJwtToken(accessToken);
                CommonClass.UserName = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                isFAlogin = token.Claims.Where(x => x.Type == "groups").Any(x=>x.Value.Contains("Attendant"));
                var tokenModel = new SwaJWTModel()
                {
                    tokenExpiry = GlobalSettings.SwaTokenExpiry,
                    Token = accessToken

                };

                string tokenData = JsonConvert.SerializeObject(tokenModel);

                KeychainHelpers.SetBearerToken(tokenData, "WBid.Oauth.token", Security.SecAccessible.AfterFirstUnlockThisDeviceOnly, false);
                
                

            }
            return (accessToken,isFAlogin);

        }

    }
    public interface IPaginatedResponse
    {
        bool HasNextPage { get; }
        object Page { get; }
    }
    // Strongly-Typed Model for Token Response
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
   

}
