using Newtonsoft.Json;

namespace PropertyManager.Data.Models {
    public class MarketRent {
        [JsonProperty("dwell")]
        public string DwellingType { get; set; }

        [JsonProperty("mean")]
        public string Mean { get; set; }
    }
}
