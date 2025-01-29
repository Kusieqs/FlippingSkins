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

            Console.Write("\n\n1. Start scraping prices rust STEAM->SKINSMONKEY\n2. Start scraping prices rust SKINSMONKEY->STEAM\n3. Information\n4. Exit\n\nNumber: ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine("\n\n");

            switch (key.KeyChar)
            {
                case '1':
                    Stopwatch stopwatch = new Stopwatch();
                    try
                    {
                        IWebDriver driver = LoginWebsites.CreatingWeb(configInformation);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey(driver);
                        driver.Quit();
                        List<Task> tasks = new List<Task>();
                        List<List<ScrapRust>> collections = new List<List<ScrapRust>>();
                        int sizeOfCollections = (int)Math.Ceiling(Scrap.scrap.Count / 5.0);

                        for (int i = 0; i < 5; i++)
                        {
                            var collection = Scrap.scrap.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collections.Add(collection);
                        }

                        Scrap.scrapPriceFromRust = collections;
                        stopwatch.Start();
                        for (int i = 0; i < collections.Count; i++)
                        {
                            Thread.Sleep(5000);
                            tasks.Add(Task.Run(async () =>
                            {
                                IWebDriver driver = new ChromeDriver(options);
                                driver.Manage().Window.Maximize();
                                await Scrap.ScrapPricesFromSteamMarket(driver);
                                driver.Quit();
                            }));
                        }
                        await Task.WhenAll(tasks);
                        stopwatch.Stop();
                        Scrap.counter = 0;
                    }
                    catch(Exception ex)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR!!!");
                        Console.ResetColor();
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine("Click enter to continue");
                        stopwatch.Stop();
                        Console.ReadKey();
                    }


                    foreach (var item in Scrap.scrap)
                    {
                        item.SetFeeOnSkinsMonkey();
                    }

                    List<ScrapRust> bestDeals = Scrap.scrap.OrderBy(x => x.Difference).Take(50).ToList();
                    Console.Clear();
                    Console.WriteLine("Best deals Steam -> SkinsMoneky:");
                    foreach (var item in bestDeals)
                    {
                        Console.WriteLine($"Name: {item.Name}\nProcent: {item.Difference}\nBuy order Steam: {item.PriceRustSteam}\nSell SkinsMoneky {item.PriceRustSkinsWithFee}\n\n");
                    }
                    Console.WriteLine(stopwatch.Elapsed.Minutes);
                    Console.ReadKey();
                    break;
                case '2':
                    //cs
                    break;
                case '3':
                    // info
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
        var proxy = new Proxy
        {
            HttpProxy = "83.12.149.202:8080",  
            SslProxy = "83.12.149.202:808",
        };


        //options.AddArgument("--headless");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--remote-allow-origins=*");
        options.AddArgument("--ignore-certificate-errors");
        //options.Proxy = proxy;
    }

    /// <summary>
    /// Setting config to login into steam and gmail
    /// </summary>
    /// <returns>ConfigInformation object</returns>
    private static ConfigInformation SettingConfig() => new ConfigInformation("flipingSkins", "vR5QKwJ252H%kpu", "flippingskins@gmail.com", "FlippingSkins123");

}