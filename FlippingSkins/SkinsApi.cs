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
        const int CURRNECY = 6;
        public static async Task<float> GetPriceAsync(string nameOfItem, int appid)
        {
            string url = $"https://steamcommunity.com/market/priceoverview/?"
                + $"appid={appid}&market_hash_name={Uri.EscapeDataString(nameOfItem)}&currency={CURRNECY}";

            Console.WriteLine(url);

            using (var client = new HttpClient())
            {
                try
                {
                    var json = await client.GetStringAsync(url);
                    Console.WriteLine(json);
                    JsonCSGOPrices? jsonCSGOPrices = JsonConvert.DeserializeObject<JsonCSGOPrices>(json);
                    Console.ReadKey();
                    if (jsonCSGOPrices != null)
                    {
                        float? price = jsonCSGOPrices.lowest_price;
                        if (price.HasValue)
                        {
                            return price.Value;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HttpRequestException: {httpEx.Message}");
                    Console.ReadKey();
                    return 0;
                }
            }
            return 0;
        }
    }
}
