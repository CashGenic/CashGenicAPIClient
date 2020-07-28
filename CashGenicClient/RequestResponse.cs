using System;
using Newtonsoft.Json;


namespace CashGenicClient
{

    public class RequestResponse
    {
        [JsonProperty("responseCode")]
        public long ResponseCode { get; set; }

        [JsonProperty("responseData")]
        public string ResponseData { get; set; }
    }

}

