using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PropertyManager.Data {
    public class TenancyService {
        readonly IConfiguration Configuration;
        readonly OauthService OauthService;
        readonly HttpClient HttpClient;
        const int MonthsOfHistoricDataToInclude = 6;
        const string DwellingType = "House";

        public TenancyService(IConfiguration configuration, OauthService oauthService)
        {
            Configuration = configuration;
            OauthService = oauthService;
            HttpClient = new HttpClient();
        }

        public async Task<string> GetMarketRent(string suburb, int numberOfBedrooms)
        {
            var accessToken = await OauthService.GetAccessToken();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var twoMonthsAgo = DateTime.Today.AddMonths(-2);
            var period = $"{twoMonthsAgo.Year}-{twoMonthsAgo.Month.PadToTwoDigits()}";
            var queryParameters = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryParameters.Add("period-ending", period);
            queryParameters.Add("num-months", MonthsOfHistoricDataToInclude.ToString());
            queryParameters.Add("area-definition", "IMR2005");
            queryParameters.Add("statistics", "mean");
            queryParameters.Add("dwelling_type", DwellingType);
            queryParameters.Add("num-bedrooms", numberOfBedrooms.ToString());
            queryParameters.Add("area-codes", FindBestAreaCodeForSuburb(suburb));
            var queryString = $"{Configuration["TenancyServicesBaseUrl"]}/statistics?{queryParameters}";
            var response = await HttpClient.GetAsync(queryString);
            var responseString = await response.Content.ReadAsStringAsync();
            var marketRentResponse = JsonConvert.DeserializeObject<MarketRentResponse>(responseString);
            return marketRentResponse.MarketRents.First().Mean;
        }

        string FindBestAreaCodeForSuburb(string suburb)
        {
            return "0604"; // make dynamic
        }
    }
}