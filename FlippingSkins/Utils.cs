using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace FlippingSkins
{
    public static class Utils
    {
        public static ChromeOptions options = new ChromeOptions();

        public const double COUNTWEB = 10.0;

        /// <summary>
        /// Setting config to login into steam and gmail
        /// </summary>
        /// <returns>ConfigInformation object</returns>
        public static ConfigInformation SettingConfig() => new ConfigInformation("flipingSkins", "vR5QKwJ252H%kpu", "flippingskins@gmail.com", "FlippingSkins123");

        /// <summary>
        /// Exception message
        /// </summary>
        /// <param name="ex"></param>
        public static void ExceptionMessage(Exception ex)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR!!!");
            Console.ResetColor();
            Console.WriteLine(ex.ToString());
            Console.WriteLine("Click enter to continue");
            Console.ReadKey();
        }

        /// <summary>
        /// Special arguments to set chrome
        /// </summary>
        public static void StartConfig()
        {
            //options.AddArgument("--headless");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--remote-allow-origins=*");
            options.AddArgument("--ignore-certificate-errors");
        }

        /// <summary>
        /// Setting price of items from CSGO (to many items to scrap)
        /// </summary>
        /// <returns></returns>
        public static Tuple<float, float> SetPriceForCSGO()
        {
            float lowPrice = 0.0f;
            float highPrice = 0.0f;

            do
            {
                Console.Clear();
                Console.Write("Set the lowest price: ");
                if (float.TryParse(Console.ReadLine().Replace('.', ','), out lowPrice)
                    && lowPrice <= 101f
                    && lowPrice >= 0.4f)
                {
                    break;
                }

            } while (true);

            do
            {
                Console.Clear();
                Console.Write("Set the highest price: ");
                if (float.TryParse(Console.ReadLine().Replace('.', ','), out highPrice)
                    && highPrice <= 100f
                    && highPrice >= 0.41f
                    && highPrice >= lowPrice)
                {
                    break;
                }

            } while (true);

            lowPrice = (float)Math.Round(lowPrice, 2);
            highPrice = (float)Math.Round(highPrice, 2);

            Console.Clear();

            return new Tuple<float, float>(lowPrice, highPrice);
        }

        /// <summary>
        /// List of tuples to define prices 
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static List<Tuple<float, float>> SetListOfTuples(Tuple<float, float> tuple)
        {
            List<Tuple<float, float>> list = new List<Tuple<float, float>>();
            float startPoint = tuple.Item1;
            float endPoint = tuple.Item2;
            float checkPrice = tuple.Item1;
            do
            {
                if (checkPrice + 0.5f >= endPoint)
                {
                    list.Add(Tuple.Create(checkPrice, endPoint));
                    break;
                }
                else
                {
                    float startCheck = checkPrice;
                    checkPrice += 0.5f;
                    list.Add(Tuple.Create(startCheck, checkPrice));
                }

            } while (true);

            return list;
        }

    }
}
