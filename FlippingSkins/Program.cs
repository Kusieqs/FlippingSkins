using System.Diagnostics;
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
            Console.WriteLine("\n\n");

            switch (key.KeyChar)
            {
                case '1':
                    try
                    {
                        IWebDriver driver = LoginWebsites.CreatingWeb(configInformation,1);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_Rust(driver);
                        driver.Quit();
                        List<Task> tasks = new List<Task>();
                        List<List<ScrapRust>> collections = new List<List<ScrapRust>>();
                        int sizeOfCollections = (int)Math.Ceiling(Scrap.scrapRust.Count / 5.0);

                        for (int i = 0; i < 5; i++)
                        {
                            var collection = Scrap.scrapRust.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collections.Add(collection);
                        }

                        Scrap.scrapPriceFromRust = collections;
                        for (int i = 0; i < collections.Count; i++)
                        {
                            Thread.Sleep(2500);
                            tasks.Add(Task.Run(async () =>
                            {
                                IWebDriver driver = new ChromeDriver(options);
                                driver.Manage().Window.Maximize();
                                await Scrap.ScrapPricesFromSteamMarketRust(driver);
                                driver.Quit();
                            }));
                        }
                        await Task.WhenAll(tasks);
                        Scrap.counter = 0;
                    }
                    catch(Exception ex)
                    {
                        ExceptionMessage(ex);
                    }

                    foreach (var item in Scrap.scrapRust)
                    {
                        item.SetProcent();
                    }
                    List<ScrapRust> bestDeals = Scrap.scrapRust.OrderByDescending(x => x.ProcentOfPrice).Take(100).ToList();

                    Console.Clear();
                    Console.WriteLine("Best deals Steam -> SkinsMoneky:");
                    foreach (var item in bestDeals)
                    {
                        item.Description();
                    }
                    Console.ReadKey();
                    break;

                case '2':
                    try
                    {
                        IWebDriver driver = LoginWebsites.CreatingWeb(configInformation,0);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_CSGO(driver);
                        foreach (var item in Scrap.scrapCSGO)
                        {
                            Console.WriteLine(item.Name);
                        }
                        Console.WriteLine(Scrap.scrapCSGO.Count);
                        driver.Quit();
                        List<Task> tasks = new List<Task>();
                        List<List<ScrapCSGO>> collections = new List<List<ScrapCSGO>>();
                        int sizeOfCollections = (int)Math.Ceiling(Scrap.scrapCSGO.Count / 5.0);

                        for (int i = 0; i < 5; i++)
                        {
                            var collection = Scrap.scrapCSGO.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collections.Add(collection);
                        }
                        Scrap.scrapPriceFromCSGO = collections;


                        for (int i = 0; i < collections.Count; i++)
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
                    }
                    catch(Exception ex)
                    {
                        ExceptionMessage(ex);
                    }

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

}