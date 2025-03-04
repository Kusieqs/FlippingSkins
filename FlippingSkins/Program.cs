using System.Diagnostics;
using System.Threading.Tasks;
using FlippingSkins;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

internal class Program
{
    public static ChromeOptions options = new ChromeOptions();
    private static async Task Main(string[] args)
    {
        ConfigInformation configInformation = SettingConfig();

        StartConfig();

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
                        sizeOfCollections = (int)Math.Ceiling(Scrap.scrapRust.Count / 10.0);
                        
                        for (int i = 0; i < 10; i++)
                        {
                            var collection = Scrap.scrapRust.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collectionsRust.Add(collection);
                        }

                        Scrap.scrapPriceFromRust = collectionsRust;
                        await AsyncWebCreator(tasks, collectionsRust.Count, 1);
                        Scrap.counter = 0;

                        foreach (var item in Scrap.scrapRust)
                        {
                            item.SetProcent();
                        }

                        List<ScrapRust> bestDeals = Scrap.scrapRust.OrderByDescending(x => x.ProcentOfPrice).Take(100).ToList();
                        ShowingDeals("Best deals Steam -> SkinsMoneky:", bestDeals.Cast<ScrapElement>().ToList());
                        break;

                    case '2':
                        webDriver = LoginWebsites.CreatingWeb(configInformation, 0);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_CSGO(webDriver);
                        webDriver.Quit();

                        List<List<ScrapCSGO>> collectionsCSGO = new List<List<ScrapCSGO>>();
                        sizeOfCollections = (int)Math.Ceiling(Scrap.scrapCSGO.Count / 10.0);

                        for (int i = 0; i < 10; i++)
                        {
                            var collection = Scrap.scrapCSGO.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collectionsCSGO.Add(collection);
                        }
                        Scrap.scrapPriceFromCSGO = collectionsCSGO;


                        for (int i = 0; i < collectionsCSGO.Count; i++)
                        {
                            Thread.Sleep(2500);
                            tasks.Add(Task.Run(async () =>
                            {
                                IWebDriver driver = new ChromeDriver(options);
                                driver.Manage().Window.Maximize();
                                await Scrap.ScrapPricesFromSteamMarketCSGO(driver);
                                driver.Quit();
                            }));
                        }
                        await Task.WhenAll(tasks);
                        Scrap.counter = 0;

                        /// POPRAWKA
                        List<ScrapCSGO> bestDealsCsgo = Scrap.scrapCSGO.OrderByDescending(x => x.Difference).Take(100).ToList();
                        Console.WriteLine("Best deals SkinsMonkey -> Steam:");
                        foreach (var item in bestDealsCsgo)
                        {
                            item.Description();
                        }
                        Console.ReadKey();
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
                ExceptionMessage(ex);
            }

            tasks.Clear();
            Console.ReadKey();
            Console.Clear();

        } while (true);
    }

    /// <summary>
    /// Special arguments to set chrome
    /// </summary>
    private static void StartConfig()
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
    /// Setting config to login into steam and gmail
    /// </summary>
    /// <returns>ConfigInformation object</returns>
    private static ConfigInformation SettingConfig() => new ConfigInformation("flipingSkins", "vR5QKwJ252H%kpu", "flippingskins@gmail.com", "FlippingSkins123");

    /// <summary>
    /// Exception message
    /// </summary>
    /// <param name="ex"></param>
    private static void ExceptionMessage(Exception ex)
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
                IWebDriver driver = new ChromeDriver(options);
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