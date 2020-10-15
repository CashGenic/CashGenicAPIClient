using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CashGenicClient
{

    public class CashGenicAPIService
    {


        private static RestClient GetClient()
        {

            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;

            string webUrl = "https://" + localSettings.Values["cgURL"].ToString() + ":" + localSettings.Values["portNumber"].ToString();


            ServicePointManager.ServerCertificateValidationCallback +=
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };


            var client = new RestClient();
            client.Timeout = 2000; //ms
            client.BaseUrl = new Uri(webUrl);

            return client;

        }




        public static async Task<ApiToken> GetToken(string username, string password)
        {

            var client = GetClient();
            var request = new RestRequest("token",Method.POST);           
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", username);
            request.AddParameter("password", password);
            IRestResponse response = await client.ExecuteAsync(request);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ApiToken>(response.Content);
            }
            else
            {
                return null;
            }

        }



        public static async Task<SystemDetail> GetSystem(string token)
        {

            var client = GetClient();
            var request = new RestRequest("system", Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {

                try
                {

                    SystemDetail systemDetail = JsonConvert.DeserializeObject<SystemDetail>(response.Content);
                    return systemDetail;  // SystemDetail.FromJson(response.Content);
                }catch(Exception ex)
                {
                    return null;
                    
                }
            }
            else
            {
                return null;
            }



        }




        public static async Task<SystemStatus> GetStatus(string token)
        {

            var client = GetClient();
            var request = new RestRequest("status", Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                SystemStatus systemStatus = JsonConvert.DeserializeObject<SystemStatus>(response.Content);
                return systemStatus;
            }
            else
            {
                return null;
            }

        }




        public static async Task<SystemResponse> StartSession(string token, int value)
        {
            var client = GetClient();
            var request = new RestRequest("session", Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { request = "PayAmount", value = value });

            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RequestResponse requestResponse = JsonConvert.DeserializeObject<RequestResponse>(response.Content);
                return (SystemResponse)requestResponse.ResponseCode;
            }
            else
            {
                return SystemResponse.ConnectionError;
            }

        }



        public static async Task<SystemResponse> StartRefundSession(string token, int value)
        {
            var client = GetClient();
            var request = new RestRequest("session", Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { request = "RefundAmount", value = value });

            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RequestResponse requestResponse = JsonConvert.DeserializeObject<RequestResponse>(response.Content);
                return (SystemResponse)requestResponse.ResponseCode;
            }
            else
            {
                return SystemResponse.ConnectionError;
            }

        }



        public static async Task<SystemResponse> CancelSession(string token)
        {


            var client = GetClient();
            var request = new RestRequest("session", Method.POST);
            // request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { request = "CancelPayment"});

            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RequestResponse requestResponse = JsonConvert.DeserializeObject<RequestResponse>(response.Content);
                return (SystemResponse)requestResponse.ResponseCode;
            }
            else
            {
                return SystemResponse.ConnectionError;
            }


        }




        public static async Task<SystemResponse> EndSession(string token)
        {


            var client = GetClient();
            var request = new RestRequest("session", Method.POST);
            // request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { request = "CloseSession" });

            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RequestResponse requestResponse = JsonConvert.DeserializeObject<RequestResponse>(response.Content);
                return (SystemResponse)requestResponse.ResponseCode;
            }
            else
            {
                return SystemResponse.ConnectionError;
            }


        }







    }
}
