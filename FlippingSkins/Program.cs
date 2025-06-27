using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using FlippingSkins.Api;
using FlippingSkins.Login;
using FlippingSkins.Scraping;
using FlippingSkins.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

internal class Program
{
    public static IWebDriver? webDriver;
    public static ConfigInformation? configInformation;

    private static void Main(string[] args)
    {
        configInformation = Config.SetConfig();

        do
        {
            Console.WriteLine("######################################");
            Console.WriteLine("################ MENU ################");
            Console.WriteLine("######################################");

            Console.Write("\n\n1. Start scraping prices rust STEAM->SKINSMONKEY" +
                "\n2. Start scraping prices csgo SKINSMONKEY->STEAM" +
                "\n3. Information" +
                "\n4. Exit" +
                "\n\nNumber: ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine("\n\n");

            Config.StartConfig();
            try
            {
                switch (key.KeyChar)
                {
                    case '1':
                        RustBestDeals();
                        break;
                    case '2':
                        CSGOBestDeals();
                        break;
                    case '3':
                        Information();
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

            Console.ReadKey();
            Console.Clear();

        } while (true);
    }

    /// <summary>
    /// Searching best deals for RUST items
    /// </summary>
    private async static void RustBestDeals()
    {
        webDriver = LoginWebsites.CreatingWeb(configInformation, 1);
        Scrap.ScrapPricesAndNamesFromSkinsMonkey_Rust(webDriver);
        webDriver.Quit();

        for (int i = 0; i < Scrap.scrapRust.Count; i++)
        {
            Scrap.scrapRust[i].PriceRustSteam = await SkinsApi.GetPriceAsync(Scrap.scrapRust[i].Name, 252490);
            Scrap.scrapRust[i].SetProcent();
        }

        List<ScrapRust> bestDealsRust = Scrap.scrapRust.
            OrderByDescending(x => x.ProcentOfPrice).
            Take(100).
            ToList();

        bestDealsRust.RemoveAll(x => x.PriceRustSteam == 0 || x.PriceRustSkinsMonkey == 0);
        ShowingDeals("Best deals Steam -> SkinsMonkey:", bestDealsRust.Cast<ScrapElement>().ToList());
    }

    /// <summary>
    /// Searching best deals for CSGO items
    /// </summary>
    private async static void CSGOBestDeals()
    {
        float sortPrice = Scrap.SetPriceForCSGO();

        webDriver = LoginWebsites.CreatingWeb(configInformation, 2);
        Scrap.ScrapPricesAndNamesFromSkinsMonkey_CSGO(webDriver, sortPrice);
        webDriver.Quit();

        for (int i = 0; i < Scrap.scrapCSGO.Count; i++)
        {
            Scrap.scrapCSGO[i].PriceCSGOSkinsSteam = await SkinsApi.GetPriceAsync(Scrap.scrapCSGO[i].Name, 730);
            Scrap.scrapCSGO[i].SetProcent();
        }


        List<ScrapCSGO> bestDealsCsgo = Scrap.scrapCSGO.
            OrderByDescending(x => x.ProcentOfPrice).
            Take(100).
            ToList();

        bestDealsCsgo.RemoveAll(x => x.PriceCSGOSkinsMonkey == 0 || x.PriceCSGOSkinsSteam == 0);
        ShowingDeals("Best deals SkinsMonkey -> Steam:", bestDealsCsgo.Cast<ScrapElement>().ToList());
    }

    private static void Information()
    {
        Console.Clear();
        string description =
            "The main purpose of this application is to generate profit by flipping skins between two platforms: Steam and SkinsMonkey.\n\n" +
            "The first option focuses on flipping *Rust* skins from Steam to SkinsMonkey. It will display the most profitable deals you can make, taking into account all applicable taxes and fees.\n\n" +
            "The second option focuses on flipping *CS:GO* skins from SkinsMonkey to Steam. Similarly, it shows you the best opportunities for profit, with all costs already factored in.\n\n" +
            "Both options calculate net profit after fees, ensuring that every deal shown is 100% profitable.\n\n" +
            "Whether you're a seasoned trader or just getting started, this tool helps you make informed and profitable decisions with minimal effort.";

        Console.WriteLine(description);
        Console.ReadKey();
        Console.Clear();
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

}