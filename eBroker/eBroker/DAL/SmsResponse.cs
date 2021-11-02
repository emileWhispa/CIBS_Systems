using Newtonsoft.Json;

namespace eBroker.DAL
{
    public class SmsResponse
    {
        [JsonProperty("response")]
        public string response;
        [JsonProperty("statusCode")]
        public int statusCode;
    }
}