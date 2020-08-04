using Newtonsoft.Json;
using PropertyManager.Data.Models;
using System.Collections.Generic;

namespace PropertyManager.Data {
    public class MarketRentResponse {
        [JsonProperty("items")]
        public List<MarketRent> MarketRents { get; set; }
    }
}
