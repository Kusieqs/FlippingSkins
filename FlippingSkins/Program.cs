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
            Console.WriteLine("\n\n");

            try
            {
                switch (key.KeyChar)
                {
                    case '1':
                        webDriver = LoginWebsites.CreatingWeb(configInformation, 1);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_Rust(webDriver);
                        webDriver.Quit();

                        for(int i = 0; i < Scrap.scrapRust.Count; i ++)
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
                        break;

                    case '2':
                        float sortPrice = Utils.SetPriceForCSGO();

                        webDriver = LoginWebsites.CreatingWeb(configInformation, 2);
                        Scrap.ScrapPricesAndNamesFromSkinsMonkey_CSGO(webDriver, sortPrice);
                        webDriver.Quit();

                        for(int i = 0; i < Scrap.scrapCSGO.Count; i++)
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