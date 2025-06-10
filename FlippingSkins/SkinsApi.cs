using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlippingSkins
{
    public static class SkinsApi
    {
        const int appid = 730;
        const int currency = 6;
        public static async Task<float> GetPriceAsync(string nameOfItem)
        {
            string url = $"https://steamcommunity.com/market/priceoverview/?"
                + $"appid={appid}&market_hash_name={Uri.EscapeDataString(nameOfItem)}&currency={currency}";

            using (var client = new HttpClient())
            {
                try
                {
                    var json = await client.GetStringAsync(url);
                    JsonCSGOPrices? jsonCSGOPrices = JsonConvert.DeserializeObject<JsonCSGOPrices>(json);
                    if (jsonCSGOPrices != null)
                    {
                        float? price = jsonCSGOPrices.lowest_price;
                        if (price.HasValue)
                        {
                            return price.Value;
                        }
                    }
                }
                catch
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
