using System.Threading.Tasks;

namespace PropertyManager.Data
{
    public class WebScraperService
    {
        const string TradeMeAddressXpath = "/html/body/trade-me/div[1]/main/div/tm-property-listing/div/tm-property-listing-body/div/section[1]/tg-row/tg-col/h1";

        public async Task<Property> GetPropertyAsync(string propertyUrl)
        {
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