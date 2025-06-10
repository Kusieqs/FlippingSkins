using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using FlippingSkins;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ConfigInformation configInformation = Utils.SettingConfig();
        Utils.StartConfig();

        do
        {
            Console.WriteLine("######################################");
            Console.WriteLine("################ MENU ################");
            Console.WriteLine("######################################");

            Console.Write("\n\n1. Start scraping prices rust STEAM->SKINSMONKEY\n2. Start scraping prices csgo SKINSMONKEY->STEAM\n3. Information\n4. Exit\n\nNumber: ");
            ConsoleKeyInfo key = Console.ReadKey();
            IWebDriver webDriver;
            List<Task> tasks = new List<Task>();
            int sizeOfCollections = 0;
            Console.WriteLine("\n\n");

            try
            {
                switch (key.KeyChar)
                {
                    case '1':
                        webDriver = LoginWebsites.CreatingWeb(configInformation, 1);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_Rust(webDriver);
                        webDriver.Quit();

                        List<List<ScrapRust>> collectionsRust = new List<List<ScrapRust>>();
                        sizeOfCollections = (int)Math.Ceiling(Scrap.scrapRust.Count / Utils.COUNTWEB);
                        
                        for (int i = 0; i < Utils.COUNTWEB; i++)
                        {
                            var collection = Scrap.scrapRust.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collectionsRust.Add(collection);
                        }

                        Scrap.scrapPriceFromRust = collectionsRust;
                        await AsyncWebCreator(tasks, collectionsRust.Count, 1);
                        Scrap.counter = 0;

                        List<ScrapRust> bestDealsRust = Scrap.scrapRust.
                            OrderByDescending(x => x.ProcentOfPrice).
                            Take(100).
                            ToList();

                        bestDealsRust.RemoveAll(x => x.PriceRustSteam == 0 || x.PriceRustSkinsMonkey == 0);
                        ShowingDeals("Best deals Steam -> SkinsMonkey:", bestDealsRust.Cast<ScrapElement>().ToList());
                        break;

                    case '2':
                        Tuple<float, float> tuple = Utils.SetPriceForCSGO();
                        List<Tuple<float, float>> listOfPrices = Utils.SetListOfTuples(tuple);
                        webDriver = LoginWebsites.CreatingWeb(configInformation, 2);


                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_CSGO(webDriver, listOfPrices);
                        webDriver.Quit();



                        List<ScrapCSGO> bestDealsCsgo = Scrap.scrapCSGO.
                            OrderByDescending(x => x.ProcentOfPrice).
                            Take(100).
                            ToList();

                        bestDealsCsgo.RemoveAll(x => x.PriceCSGOSkinsMonkey == 0 || x.PriceCSGOSkinsSteam == 0);
                        ShowingDeals("Best deals SkinsMonkey -> Steam:", bestDealsCsgo.Cast<ScrapElement>().ToList());
                        break;
                    case '3':
                        break;
                    case '4':
                        Environment.Exit(0);
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.ExceptionMessage(ex);
            }

            tasks.Clear();
            Console.ReadKey();
            Console.Clear();

        } while (true);
    }

    /// <summary>
    /// Async method to run websites
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="loops"></param>
    /// <param name="mode"></param>
    private async static Task AsyncWebCreator(List<Task> tasks, int loops, int mode)
    {
        for (int i = 0; i < loops; i++)
        {
            Thread.Sleep(2500);
            tasks.Add(Task.Run(async () =>
            {
                IWebDriver driver = new ChromeDriver(Utils.options);
                driver.Manage().Window.Maximize();
                if (mode == 1)
                {
                    await Scrap.ScrapPricesFromSteamMarketRust(driver);
                }
                else
                {
                    await Scrap.ScrapPricesFromSteamMarketCSGO(driver);
                }
                driver.Quit();
            }));
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Showing best deals
    /// </summary>
    /// <param name="themeOfItems"></param>
    /// <param name="bestDeals"></param>
    private static void ShowingDeals(string themeOfItems, List<ScrapElement> bestDeals)
    {
        Console.Clear();
        Console.WriteLine("\n\n");
        Console.WriteLine(themeOfItems);
        foreach (var item in bestDeals)
        {
            item.Description();
        }
        Console.ReadKey();
    }

}