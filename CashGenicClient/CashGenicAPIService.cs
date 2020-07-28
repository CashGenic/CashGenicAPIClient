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

        private const string webUrl = "https://192.168.1.109:44333/";




        private static RestClient GetClient()
        {


            ServicePointManager.ServerCertificateValidationCallback +=
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };


            var client = new RestClient();
            client.Timeout = -1;
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
                SystemDetail systemDetail = JsonConvert.DeserializeObject<SystemDetail>(response.Content);
                return SystemDetail.FromJson(response.Content);
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
           // request.RequestFormat = DataFormat.Json;
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







    }
}
