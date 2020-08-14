using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace PropertyManager.Data
{
    public class WebScraperService
    {
        readonly IConfiguration Configuration;
        const string TradeMeAddressXpath = "/html/body/trade-me/div[1]/main/div/tm-property-listing/div/tm-property-listing-body/div/section[1]/tg-row/tg-col/h1";

        public WebScraperService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<Property> GetProperty(string propertyUrl)
        {
            var url = new Uri(propertyUrl);
            if (url.Host != Configuration["TradeMeHostUrl"]) {
                throw new ArgumentException("Only Trade Me listings are supported");
            }

            var web = new HtmlAgilityPack.HtmlWeb();
            var document = await web.LoadFromWebAsync(propertyUrl);
            var address = document.DocumentNode.SelectSingleNode(TradeMeAddressXpath).InnerText;

            return new Property {
                Address = address,
                Cv = 900000
            };
        }
    }
}