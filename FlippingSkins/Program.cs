using FlippingSkins;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

internal class Program
{
    private static void Main(string[] args)
    {
        ConfigInformation configInformation = SettingConfig();

        StartInfo();

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
                        Scrap.ScrapPricesFromSteamMarket(driver);
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
    
    private static void StartInfo()
    {
        Console.WriteLine("You can only turn on the application once every 10 minutes. If you decide to use it earlier, it may crash.");
        Console.WriteLine("Click enter to continue");
        Console.ReadKey();
        Console.Clear();
    }
    private static ConfigInformation SettingConfig() => new ConfigInformation("flipingSkins", "vR5QKwJ252H%kpu", "flippingskins@gmail.com", "FlippingSkins123");

}