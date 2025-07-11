using System.Globalization;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace FlippingSkins.Api
{
    public static class SkinsApi
    {
        const int CURRNECY = 1;
        public static int count = 0;
        public static bool isItMaximumRequest = false;

        /// <summary>
        /// Getting price from api
        /// </summary>
        /// <param name="nameOfItem"></param>
        /// <param name="appid"></param>
        /// <returns>The lowest price to get item</returns>
        public static async Task<float> GetPriceAsync(string nameOfItem, int appid)
        {
            if (isItMaximumRequest) 
            {
                return 0;
            }

            string url = $"https://steamcommunity.com/market/priceoverview/?"
                + $"appid={appid}&market_hash_name={Uri.EscapeDataString(nameOfItem)}&currency={CURRNECY}";

            count++;
            if (count % 20 == 0)
            {
                await Task.Delay(60000);
            }


            using (var client = new HttpClient())
            {
                try
                {
                    var json = await client.GetStringAsync(url);
                    Console.WriteLine(json);
                    JsonCSGOPrices? jsonCSGOPrices = JsonConvert.DeserializeObject<JsonCSGOPrices>(json);
                    if (jsonCSGOPrices != null)
                    {
                        float price = 0;
                        if (jsonCSGOPrices.lowest_price != null)
                        {
                            string priceString = jsonCSGOPrices.lowest_price.ToString().Replace("$", "");
                            price = float.Parse(priceString, CultureInfo.InvariantCulture);
                        }
                        return price;
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HttpRequestException: {httpEx.Message}");
                    Console.ReadKey();
                    isItMaximumRequest = true;
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ReadKey();
                    return 0;
                }

            }
            return 0;
        }
    }
}
