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
            try
            {
                Console.WriteLine("######################################");
                Console.WriteLine("################ MENU ################");
                Console.WriteLine("######################################");

                Console.Write("\n\n1. Start scraping prices\n2. Informations\n3. Exit\n\nNumber: ");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine("\n\n");

                switch(key.KeyChar)
                {
                    case '1':
                        IWebDriver driver = LoginWebsites.CreatingWeb(configInformation);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey(driver);
                        driver.Quit();

                        foreach (var item in Scrap.scrap)
                        {
                            Console.WriteLine($"{item.Name}\n");
                        }
                        Console.ReadKey();


                        List<Task> tasks = new List<Task>();
                        List<List<ScrapRust>> collections = new List<List<ScrapRust>>();
                        int sizeOfCollections = (int)Math.Ceiling(Scrap.scrap.Count / 10.0);

                        for(int i = 0; i < 10; i++)
                        {
                            var collection = Scrap.scrap.Skip(i * sizeOfCollections).Take(sizeOfCollections).ToList();
                            collections.Add(collection);
                        }

                        Scrap.scrapPriceFromRust = collections;

                        for (int i = 0; i < collections.Count; i++) 
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                IWebDriver driver = new ChromeDriver(options);
                                driver.Manage().Window.Maximize();
                                await Scrap.ScrapPricesFromSteamMarket(driver);
                                driver.Quit();
                            }));
                        }
                        await Task.WhenAll(tasks);

                        Scrap.counter = 0;

                        foreach (var item in Scrap.scrap)
                        {
                            Console.WriteLine($"{item.Name}\n{item.PriceRustSkinsMonkey}\t{item.PriceRustSteam}\n");
                        }
                        break;
                    case '2':
                        // info
                        break;
                    case '3':
                        Environment.Exit(0);
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR!!!");
                Console.ResetColor();
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Click enter to continue");
                Console.ReadKey();
            }
        }while (true);
    }
    
    /// <summary>
    /// Special arguments to set chrome
    /// </summary>
    private static void StartConfig()
    {
        //options.AddArgument("--headless");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddArgument("--disable-gpu");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--remote-allow-origins=*");
    }

    /// <summary>
    /// Setting config to login into steam and gmail
    /// </summary>
    /// <returns>ConfigInformation object</returns>
    private static ConfigInformation SettingConfig() => new ConfigInformation("flipingSkins", "vR5QKwJ252H%kpu", "flippingskins@gmail.com", "FlippingSkins123");

}