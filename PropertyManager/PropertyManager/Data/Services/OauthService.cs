using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PropertyManager.Data {
    public class OauthService {
        readonly IConfiguration Configuration;
        readonly HttpClient HttpClient;

        public OauthService(IConfiguration configuration)
        {
            Configuration = configuration;
            HttpClient = new HttpClient();
            var basicAuth = $"{Configuration["TenancyServicesApiConsumerKey"]}:{Configuration["TenancyServicesApiConsumerSecret"]}";
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth.Base64Encode());
        }

        public async Task<string> GetAccessToken()
        {
            var values = new Dictionary<string, string> {
                { "grant_type", "client_credentials" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await HttpClient.PostAsync(Configuration["TenancyServicesAuthServerUrl"], content);
            var responseString = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(responseString);
            return accessTokenResponse.AccessToken;
        }
    }
}