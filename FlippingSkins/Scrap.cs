using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace FlippingSkins
{
    internal static class Scrap
    {

        public static List<ScrapRust> scrapRust = new List<ScrapRust>();
        public static List<List<ScrapRust>> scrapPriceFromRust;
        public static List<ScrapCSGO> scrapCSGO = new List<ScrapCSGO>();
        public static List<List<ScrapCSGO>> scrapPriceFromCSGO;
        public static int counter = 0;

        public static void ScrapPricesAndNamesFromSkinsMonkey_Rust(IWebDriver driver)
        {
            Actions actions = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            bool isToHighPrice = true;

            do
            {
                Thread.Sleep(2000);
                var namesToScrap = wait.Until(driver => driver.FindElements(By.XPath("//span[@class='item-card__name']")));
                var pricesToScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-price item-card__price']")));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                Console.Clear();

                for (int i = 0; i < namesToScrap.Count; i++)
                {
                    string name = (string)js.ExecuteScript("return arguments[0].textContent;", namesToScrap[i]);
                    string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesToScrap[i]);
                    ScrapRust scrapElement = new ScrapRust(name, float.Parse(price.Remove(0, 1).Trim(), CultureInfo.InvariantCulture));

                    if (!scrapRust.Any(x => x.Name == name))
                    {
                        scrapRust.Add(scrapElement);
                    }

                    if (scrapElement.PriceRustSkinsMonkey < 1.5)
                    {
                        isToHighPrice = false;
                        break;
                    }
                }

                var scrollbar = namesToScrap[19];
                actions.MoveToElement(scrollbar).Click().Build().Perform();

                for(int i = 0; i < 3; i ++)
                {
                    actions.SendKeys(Keys.PageDown).Build().Perform();
                    Thread.Sleep(250);
                }
            } while (isToHighPrice);
            
        }
        public static void ScrapPricesAndNamesFromSkinsMonkey_CSGO(IWebDriver driver, List<Tuple<float,float>> tuples)
        {
            bool isToHighPrice = true;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            var sorting = wait.Until(driver => driver.FindElements(By.XPath("//input[@class='form-input__core']")));
            Actions action = new Actions(driver);

            for (int i = 0; tuples.Count > i; i++)
            {
                action.Click(sorting[1]).Build().Perform();
                sorting[1].SendKeys(Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.Backspace + Keys.ArrowRight + Keys.Backspace);
                sorting[1].SendKeys(tuples[i].Item1.ToString());

                action.Click(sorting[2]).Build().Perform();
                sorting[2].SendKeys(tuples[i].Item2.ToString() + Keys.Enter);

                Thread.Sleep(2000);

                do
                {
                    Thread.Sleep(2000);
                    var pricesV1toScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-price item-card__price']")));
                    var element = wait.Until(driver => driver.FindElements(By.CssSelector("img.item-image")));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                    for (int j = 0; j < pricesV1toScrap.Count; j++)
                    {
                        string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV1toScrap[j]);
                        price = price.Remove(0, 1).Trim();
                        string altText = element[j].GetAttribute("alt");

                        string name = altText.Trim();

                        ScrapCSGO scrapElement = new ScrapCSGO(name, float.Parse(price, CultureInfo.InvariantCulture));

                        if (!scrapCSGO.Any(x => x.Name == name))
                        {
                            scrapCSGO.Add(scrapElement);
                        }

                        if (scrapElement.PriceCSGOSkinsMonkey < 0.4 || scrapElement.PriceCSGOSkinsMonkey < tuples[i].Item1)
                        {
                            isToHighPrice = false;
                            break;
                        }

                    }

                    int timer = 0;
                    while(isToHighPrice)
                    {
                        try
                        {
                            var scrollbar = pricesV1toScrap[19];
                            action.MoveToElement(scrollbar).Click().Build().Perform();
                            break;
                        }
                        catch
                        {
                            timer++;
                            Thread.Sleep(1000);
                            if (timer == 15)
                            {
                                return;
                            }
                        }
                    };


                    for (int j = 0; j < 4; j++)
                    {
                        action.SendKeys(Keys.PageDown).Build().Perform();
                        Thread.Sleep(250);
                    }


                } while (isToHighPrice);
            }

        }
        public static async Task ScrapPricesFromSteamMarketRust(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            Actions action = new Actions(driver);
            driver.Navigate().GoToUrl("https://rust.scmm.app/items");
            await Task.Delay(6500);
            string originalWindow = driver.CurrentWindowHandle;

            foreach (var item in scrapPriceFromRust[counter++])
            {
                int counterOfReadingPrice = 0;
                EnterTextIntoSearch(driver, wait, item.Name, "//input[@type='text'][@class='mud-input-slot mud-input-root mud-input-root-outlined']");
                Stopwatch sw = new Stopwatch();
                sw.Start();

                bool isCorrectWindow = false;
                do
                {
                    counterOfReadingPrice++;
                    int x = 0;
                    do
                    {
                        Thread.Sleep(1000);
                        counterOfReadingPrice++;
                        var findElement = wait.Until(driver => driver.FindElements(By.XPath($"//p[span[text()=\"{item.Name}\"]]/following-sibling::p/span")));


                        if (findElement.Count > 0)
                        {
                            isCorrectWindow = true;
                            item.PriceRustSteam = float.Parse(findElement[0].Text.Remove(0, 1), CultureInfo.InvariantCulture);
                            item.SetProcent();
                            break;
                        }
                        else if (sw.Elapsed.Seconds > 20)
                        {
                            isCorrectWindow = true;
                            break;
                        }

                    } while (++x < 9);
                    sw.Stop();

                    if (isCorrectWindow)
                        break;
                    else
                    {
                        var nextPage = wait.Until(driver => driver.FindElements(By.XPath("//button[@aria-label='Next page'][@class='mud-button-root mud-icon-button mud-ripple mud-ripple-icon']")));
                        var textGlitch = wait.Until(driver => driver.FindElements(By.XPath($"//p[text()=\"Nothing found, try broadening your search\"]")));

                        if (nextPage.Count > 0)
                            action.MoveToElement(nextPage[0]).Click().Perform();
                        else if (textGlitch.Count > 0)
                            EnterTextIntoSearch(driver,wait, item.Name, "//input[@type='text'][@class='mud-input-slot mud-input-root mud-input-root-outlined']");
                    }
                } while (true);
            }
        }
        public static async Task ScrapPricesFromSteamMarketCSGO(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            Actions action = new Actions(driver);
            driver.Navigate().GoToUrl("https://csgostocks.de/");
            await Task.Delay(3500);

            var consider = wait.Until(driver => driver.FindElements(By.XPath("//p[@class='fc-button-label']")));
            action.MoveToElement(consider[0]).Click().Perform();
            await Task.Delay(1000);


            string originalWindow = driver.CurrentWindowHandle;

            foreach (var item in scrapPriceFromCSGO[counter++])
            {
                Thread.Sleep(1000);
                var searchClick = wait.Until(driver => driver.FindElement(By.XPath("//input[@placeholder='Search for items...']")));
                action.MoveToElement(searchClick).Click().Perform();
                Thread.Sleep(500);
                EnterTextIntoSearch(driver, wait, item.Name, "//input[@placeholder='Search for items...']");
                Thread.Sleep(500);
                var elements = wait.Until(driver => driver.FindElements(By.XPath("//button[@role='option']//a//span")));
                foreach (var element in elements)
                {
                    if(element.Text == item.Name)
                    {
                        action.MoveToElement(element).Click().Perform();
                        break;
                    }
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                string priceOfItem = "";
                do
                {
                    Thread.Sleep(200);
                    var trElements = wait.Until(driver => driver.FindElements(By.XPath("//td/span")));
                    priceOfItem = trElements.First().Text;

                    
                    if (priceOfItem != "N/A")
                    {
                        sw.Stop();
                        item.PriceCSGOSkinsSteam = float.Parse(priceOfItem.Remove(0, 1), CultureInfo.InvariantCulture);
                        item.SetProcent();
                        break;
                    }
                    else if(sw.Elapsed.TotalSeconds > 5)
                    {
                        sw.Stop();
                        break;
                    }

                    /// poprawka
                } while (true);
            }

        }


        /// <summary>
        /// Inputing text into textbox
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="wait">WebDriverWait object</param>
        /// <param name="name">Name of item</param>
        private static void EnterTextIntoSearch(IWebDriver driver, WebDriverWait wait, string name, string xpath)
        {
            var writeItem = wait.Until(driver => driver.FindElement(By.XPath(xpath)));
            writeItem.SendKeys(Keys.Control + "a");
            writeItem.SendKeys(Keys.Delete);
            writeItem.SendKeys($"{name}");
        }
    }
}
