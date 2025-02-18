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
            bool isToHighPrice = true;
            do
            {
                Thread.Sleep(2000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                var namesToScrap = wait.Until(driver => driver.FindElements(By.XPath("//span[@class='item-card__name']")));
                var pricesV1toScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-price item-card__price']")));

                Console.Clear();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                for (int i = 0; i < namesToScrap.Count; i++)
                {
                    string name = (string)js.ExecuteScript("return arguments[0].textContent;", namesToScrap[i]);
                    string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV1toScrap[i]);
                    price = price.Remove(0, 1).Trim();

                    ScrapRust scrapElement = new ScrapRust(name, float.Parse(price, CultureInfo.InvariantCulture));

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


                var scrollbar = namesToScrap[24];
                actions.MoveToElement(scrollbar).Click().Build().Perform();

                for(int i = 0; i < 3; i ++)
                {
                    actions.SendKeys(Keys.PageDown).Build().Perform();
                    Thread.Sleep(250);
                }

            } while (isToHighPrice);
            
        }
        public static void ScrapPricesAndNamesFromSkinsMonkey_CSGO(IWebDriver driver)
        {
            bool isToHighPrice = true;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            var sorting = wait.Until(driver => driver.FindElements(By.XPath("//input[@class='form-input__core']")));
            Actions action = new Actions(driver);
            action.Click(sorting[2]).Build().Perform();
            sorting[2].SendKeys("1.2");

            do
            {
                Thread.Sleep(2000);
                var pricesV1toScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-price item-card__price']")));
                var element = wait.Until(driver => driver.FindElements(By.CssSelector("img.item-image")));

                Console.Clear();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                for (int i = 0; i < pricesV1toScrap.Count; i++)
                {
                    string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV1toScrap[i]);
                    price = price.Remove(0, 1).Trim();
                    string altText = element[i].GetAttribute("alt"); 

                    string name = altText.Trim();

                    ScrapCSGO scrapElement = new ScrapCSGO(name, float.Parse(price, CultureInfo.InvariantCulture));

                    if (!scrapCSGO.Any(x => x.Name == name))
                    {
                        scrapCSGO.Add(scrapElement);
                    }

                    if (scrapElement.PriceCSGOSkinsMonkey < 1)
                    {
                        isToHighPrice = false;
                        break;
                    }

                }


                var scrollbar = pricesV1toScrap[29];
                action.MoveToElement(scrollbar).Click().Build().Perform();

                for (int i = 0; i < 4; i++)
                {
                    action.SendKeys(Keys.PageDown).Build().Perform();
                    Thread.Sleep(250);
                }

            } while(isToHighPrice);

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
                bool bug = false;
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
                        var findElement = wait.Until(driver => driver.FindElements(By.XPath($"//span[text()=\"{item.Name}\"]")));

                        if (findElement.Count > 0)
                        {
                            action.MoveToElement(findElement[0]).Click().Perform();
                            isCorrectWindow = true;
                            break;
                        }
                        else if (sw.Elapsed.Seconds > 45)
                        {
                            isCorrectWindow = true;
                            bug = true;
                            sw.Stop();
                            break;
                        }

                    } while (++x < 9);

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

                if (bug)
                    continue;

                foreach (string windowHandle in driver.WindowHandles)
                {
                    if (windowHandle != originalWindow)
                    {
                        driver.SwitchTo().Window(windowHandle);
                        break;
                    }
                }

                SearchingPrice(driver, item);
                driver.Close();
                driver.SwitchTo().Window(originalWindow);
            }
        }

        /// <summary>
        /// Inputing text into textbox
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="wait">WebDriverWait object</param>
        /// <param name="name">Name of item</param>
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
                        item.SetFeeOnSteam();
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
        private static void EnterTextIntoSearch(IWebDriver driver, WebDriverWait wait, string name, string xpath)
        {
            var writeItem = wait.Until(driver => driver.FindElement(By.XPath(xpath)));
            writeItem.SendKeys(Keys.Control + "a");
            writeItem.SendKeys(Keys.Delete);
            writeItem.SendKeys($"{name}");
        }

        /// <summary>
        /// Searching price in website
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="item">Name of item to scrap price</param>
        private static void SearchingPrice(IWebDriver driver, ScrapRust item)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do
            {
                var priceElements = driver.FindElements(By.XPath("//h6[contains(@class, 'mud-typography mud-typography-h6 pa-2')]//span[contains(text(), '$')]"));

                if (priceElements.Count > 0)
                {
                    item.PriceRustSteam = float.Parse(priceElements[0].Text.Remove(0, 1), CultureInfo.InvariantCulture);
                    break;
                }
                else if (sw.Elapsed.TotalSeconds > 45)
                {
                    break;
                }
                Thread.Sleep(500);
            } while (true);
            sw.Stop();
        }
    }
}
