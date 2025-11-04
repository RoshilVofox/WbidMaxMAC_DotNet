using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ADT.Common.Models;
using System.Net.NetworkInformation;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using WBidMac.SwaApiModels;
using Newtonsoft.Json.Serialization;
using iOSPasswordStorage;
using WBid.WBidiPad.iOS.SwaApiModels;
using System.Text.Json;
using WBid.WBidiPad.Core;

namespace ADT.Common.Utility
{
    public static class ServiceHelper
    {
        

        private static ApiService _apiService;



       /// <summary>
       /// Get trip Data using SWA API
       /// </summary>
       /// <param name="packageId"></param>
       /// <param name="test"></param>
       /// <returns></returns>
        public async static Task<List<SWATrip.LinesPairing>> GetTripDataFromSWA(string packageId, string test)
        {
            string token = GlobalSettings.SwaAccessToken;


            string baseUrl = $"{APIConstants.PairingretrivalAPI}/{packageId}";
            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {
                { "authorization", "Bearer " + token },
                { "x-swa-user-department", "IF" },
                { "Accept", "*/*" },
                { "cache-control", "no-cache" }
            };

            return await _apiService.GetPaginatedDataAsync< SWATrip.Root, SWATrip.LinesPairing>(headers, null, response => response._embedded.LinesPairings);
        }
        /// <summary>
        /// Get Line Data using SWA API
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public async static Task<List<SWALine.LinesLine>> GetLineDataFromSWA(string packageId, string test)
        {

            string token = GlobalSettings.SwaAccessToken;

            string baseUrl = $"{APIConstants.LineretrivalAPI}/{packageId}";
            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {
                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };

          
            return await _apiService.GetPaginatedDataAsync< SWALine.Root, SWALine.LinesLine>(headers, null, response => response._embedded.LinesLines);
        }
        /// <summary>
        /// Get Seniority data using SWA API
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static async Task<List<SWASeniority.IFLineBaseAuctionSeniority>> GetSeniorityDataFromSWA(string packageId)
        {
            
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.SeniriotyRetrievalForADomicileAPI}/{packageId}/seniority";
           
            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {
                
                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };

          
            return await _apiService.GetPaginatedDataAsync< SWASeniority.Root, SWASeniority.IFLineBaseAuctionSeniority>(headers, null, response => response?._embedded?.IFLineBaseAuctionSeniorities??new List<SWASeniority.IFLineBaseAuctionSeniority>());
        }
        /// <summary>
        /// Get Cover Letter Data using SWA API
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<byte[]> GetCoverLetterDataFromSWAAsync(string packageId)
        {
           
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.SeniriotyRetrievalForADomicileAPI}/{packageId}/cover-letter";
            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {

                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };
           var response= await _apiService.GetDataAsync(headers,null);
            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to download PDF: {response.StatusCode} - {response.Content}");
            }

            // Ensure content is a PDF
            if (response.ContentType != "application/pdf")
            {
                throw new Exception("Invalid content type, expected application/pdf.");
            }

            // Save the response raw bytes to a file
            if (response.RawBytes != null)
            {
                return response.RawBytes;
                
            }
            else
            {
                throw new Exception("Failed to retrieve PDF content.");
            }
        }

        public static async Task<SwaBuddyList> GetBuddyListFromSWAAsync(string employeeID)
        {
            
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.BuddyListAPI}";
            _apiService = new ApiService(baseUrl);
            
            var headers = new Dictionary<string, string>
            {
                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };
            var parameter = new Dictionary<string, object>
            {
                { "employeeId",employeeID}
            };
            var response = await _apiService.GetDataAsync(headers, parameter);
            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to retrieve Buddy List for {employeeID}");
            }

            return JsonConvert.DeserializeObject<SwaBuddyList>(response.Content);
        }

        public static async Task<List<SwaBidAward.IFLineBaseAuctionAward>> GetBidAwardAsync(string packageID)
        {
            
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.BidAwardRetrievalForADomicileAPI}/{packageID}/line-awards";

            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {
                
                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };


            return await _apiService.GetPaginatedDataAsync<SwaBidAward.Root, SwaBidAward.IFLineBaseAuctionAward>(headers, null, response => response?._embedded?.IFLineBaseAuctionAwards?? new List<SwaBidAward.IFLineBaseAuctionAward>());
        }

        public static async Task<List<SwaJobShareBidAward.IFLineBaseAuctionJobShareAward>> GetJobShareBidAwardAsync(string packageID)
        {
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.BidAwardRetrievalForADomicileAPI}/{packageID}/jobshare-awards";

            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {

                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };


            return await _apiService.GetPaginatedDataAsync<SwaJobShareBidAward.Root, SwaJobShareBidAward.IFLineBaseAuctionJobShareAward>(headers, null, response => response?._embedded?.IFLineBaseAuctionJobShareAwards??new List<SwaJobShareBidAward.IFLineBaseAuctionJobShareAward>());
        }

        public static async Task<List<SwaMrtBidAward.IFLineBaseAuctionMrtAward>> GetMRTBidAwardAsync(string packageID)
        {
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.BidAwardRetrievalForADomicileAPI}/{packageID}/mrt-awards";

            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {

                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };


            return await _apiService.GetPaginatedDataAsync<SwaMrtBidAward.Root, SwaMrtBidAward.IFLineBaseAuctionMrtAward>(headers, null, response => response?._embedded?.IFLineBaseAuctionMrtAwards ?? new List<SwaMrtBidAward.IFLineBaseAuctionMrtAward>()); ;
        }


        public static async Task<List<SwaReserveAward.IFLineBaseAuctionReserveAward>> GetReserveAwardAsync(string packageID)
        {
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.BidAwardRetrievalForADomicileAPI}/{packageID}/reserve-awards";

            _apiService = new ApiService(baseUrl);
            var headers = new Dictionary<string, string>
            {

                 { "authorization", "Bearer " + token },
                 { "x-swa-user-department", "IF" },
                 { "Accept", "*/*" },
                 { "cache-control", "no-cache" }
            };


            return await _apiService.GetPaginatedDataAsync<SwaReserveAward.Root, SwaReserveAward.IFLineBaseAuctionReserveAward>(headers, null, response => response?._embedded?.IFLineBaseAuctionReserveAwards ?? new List<SwaReserveAward.IFLineBaseAuctionReserveAward>()); ;
        }

        public static async Task<List<SwaBidResponse>>SubmitBid(SwaApiSubmitBid submitBid,string packageId)
        {
            
            string token = GlobalSettings.SwaAccessToken;
            string baseUrl = $"{APIConstants.BidSubmitAPI}/{packageId}/bids";
            _apiService = new ApiService(baseUrl);
       
            string jsonString = JsonConvert.SerializeObject(submitBid, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore // Ignores null properties
            });

            var headers = new Dictionary<string, string>
            {
                {"authorization","Bearer "+token},
                {"x-swa-user-department","IF"},
                {"Accept","*/*" },
                {"cache-control","no-cache" }
            };
            var response = await _apiService.PostDataAsync(headers, jsonString);
            if(response.IsSuccessful)
            {
                var bidResult= JsonConvert.DeserializeObject<WBidMac.SwaApiModels.RootObject>(response.Content).Embedded.IFLineBaseAuctionBids.ToList();
                if(bidResult.Count!=0)
                {
                    return bidResult;
                }
                else
                {
                    throw new Exception($"Bid submitted successfully but was not able to retrieve the submit result");
                }
                
            }
            else
            {
                string errorMessage = $"Failed to submit bid : ";

                try
                {
                    using var doc = JsonDocument.Parse(response.Content);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("message", out var msg))
                        errorMessage += $"{msg.GetString()}";
                }
                catch
                {

                    //errorMessage += $" - Raw response: {response.Content}";
                }
                throw new Exception(errorMessage);
            }
           
            
        }

        //public static SwaJWTModel GetStoredToken()
        //{
        //    string tokenData = string.Empty;
        //    tokenData = KeychainHelpers.GetBearerToken("WBid.Oauth.token", false);
        //    try
        //    {
        //        var tokenObject = JsonConvert.DeserializeObject<SwaJWTModel>(tokenData);
        //        string accessToken = tokenObject.Token;
        //        long expiryTimestamp = long.Parse(tokenObject.tokenExpiry);

        //        DateTime expiryTime = DateTimeOffset.FromUnixTimeSeconds(expiryTimestamp).UtcDateTime;
        //        bool isExpired = DateTime.UtcNow >= expiryTime;
        //        //isExpired = true;
        //        //if(isExpired)
        //        //{
        //        //    await ShowPKCELoginWindow();
        //        //}
        //        return (accessToken, expiryTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error retrieving token: {ex.Message}");
        //        return (null, null);
        //    }
        //}

       

    }


}

