using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PropertyManager.Data
{
    public class WebScraperService
    {
        readonly IConfiguration Configuration;
        readonly IWebDriver WebDriver;
        const string TradeMeAddressXpath = "/html/body/trade-me/div[1]/main/div/tm-property-listing/div/tm-property-listing-body/div/section[1]/tg-row/tg-col/h1";
        const string HomesSearchInputSelector = "autocomplete-search";
        const string HomesSearchSubmitSelector = "#masthead > div > homes-search > div > div.addressSearch > homes-button > button";
        const string HomesPropertySelector = "/html/body/homes-root/homes-map-page/div/homes-drawer/div[2]/homes-property-map/homes-map/div[1]/div[1]/div[4]/div[1]/homes-current-map-marker-simple";
        const string HomesPropertyValueSelector = "//*[@id=\"mat-tab-content-0-0\"]/div/div/div[1]/homes-hestimate-tab/homes-price-tag-simple/div/span[2]";

        public WebScraperService(IConfiguration configuration, IWebDriver webDriver)
        {
            Configuration = configuration;
            WebDriver = webDriver;
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
            var homesValue = GetHomesValue(address);

            return new Property {
                Address = address,
                Cv = 900000,
                HomesValue = homesValue
            };
        }

        decimal GetHomesValue(string address)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            using (IWebDriver driver = new ChromeDriver(chromeOptions)) {
                try {
                    driver.Navigate().GoToUrl(Configuration["HomesBaseUrl"]);
                    Thread.Sleep(1000);
                    var searchBoxAccessor = By.Id(HomesSearchInputSelector);
                    var waitForSearchInputToLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    var searchInput = waitForSearchInputToLoad.Until(driver => {
                        var elementToBeDisplayed = driver.FindElement(searchBoxAccessor);
                        if (elementToBeDisplayed.Displayed) {
                            return elementToBeDisplayed;
                        }
                        return null;
                    });
                    Thread.Sleep(1000);
                    searchInput.SendKeys(address);
                    Thread.Sleep(1000);
                    driver.FindElement(By.CssSelector(HomesSearchSubmitSelector)).Click();
                    Thread.Sleep(1000);

                    var propertyLinkAccessor = By.XPath(HomesPropertySelector);
                    var waitForPropertyMapToLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    var propertyLink = waitForPropertyMapToLoad.Until(driver => {
                        try {
                            var elementToBeDisplayed = driver.FindElement(propertyLinkAccessor);
                            if (elementToBeDisplayed.Displayed) {
                                return elementToBeDisplayed;
                            }
                        } catch (NoSuchElementException) {
                            return null;
                        }
                        return null;
                    });
                    Thread.Sleep(1000);

                    propertyLink.Click();
                    Thread.Sleep(1000);


                    var propertyValueAccessor = By.XPath(HomesPropertyValueSelector);
                    var waitForPropertyLinkToLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    var propertyValue = waitForPropertyLinkToLoad.Until(driver => {
                        try {
                            var elementToBeDisplayed = driver.FindElement(propertyValueAccessor);
                            if (elementToBeDisplayed.Displayed) {
                                return elementToBeDisplayed;
                            }
                        } catch (NoSuchElementException) {
                            return null;
                        }
                        return null;
                    });
                    var value = ConvertToHomesDollarValue(propertyValue.Text);
                    return value;
                } finally {
                    driver.Quit();
                }
            }
        }

        static decimal ConvertToHomesDollarValue(string value)
        {
            var suffix = value.Last();
            switch (suffix) {
                case 'K':
                    var thousands = value[0..^1];
                    return decimal.Parse(thousands) * 1000;
                case 'M':
                    var millions = value[0..^1];
                    return decimal.Parse(millions) * 1000000;
                default:
                    throw new ArgumentException("Only conversions for thousands (K) and millions (M) are supported");
            }
        }
    }
}