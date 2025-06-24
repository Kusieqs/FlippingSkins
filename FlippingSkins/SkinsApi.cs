using System.Globalization;
using Newtonsoft.Json;

namespace FlippingSkins
{
    public static class SkinsApi
    {
        const int CURRNECY = 1;
        public static int count = 0;
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
                    if (jsonCSGOPrices != null)
                    {
                        float? price = null;
                        if (jsonCSGOPrices.lowest_price != null)
                        {
                            string priceString = jsonCSGOPrices.lowest_price.ToString().Replace("$", "");
                            price = float.Parse(priceString, CultureInfo.InvariantCulture);
                        }

                        if (price.HasValue)
                        {
                            count++;
                            if (count % 20 == 0)
                            {
                                await Task.Delay(60000);
                            }
                            return price.Value;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HttpRequestException: {httpEx.Message}");
                    Console.WriteLine(count);
                    Console.ReadKey();
                    return 0;
                }
            }
            return 0;
        }
    }
}
