using Newtonsoft.Json;

namespace PropertyManager.Data {
    public class AccessTokenResponse {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
