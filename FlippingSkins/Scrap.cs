using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
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
                var pricesV2ToScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-card__info']")));

                Console.Clear();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                for (int i = 0; i < namesToScrap.Count; i++)
                {
                    string name = (string)js.ExecuteScript("return arguments[0].textContent;", namesToScrap[i]);
                    string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV1toScrap[i]);
                    string littlePrice = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV2ToScrap[i]);
                    price = price.Remove(0, 1).Trim() + littlePrice;

                    ScrapRust scrapElement = new ScrapRust(name, float.Parse(price, CultureInfo.InvariantCulture));

                    if (!scrapRust.Any(x => x.Name == name))
                    {
                        scrapRust.Add(scrapElement);
                    }

                    if (scrapElement.PriceRustSkinsMonkey < 1.8)
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
            int higher = 999;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            var sorting = wait.Until(driver => driver.FindElements(By.XPath("//input[@class='form-input__core']")));
            Actions action = new Actions(driver);
            action.Click(sorting[2]).Build().Perform();
            Thread.Sleep(500);
            sorting[2].SendKeys("999");
            Thread.Sleep(1000);

            do
            {

                Thread.Sleep(2000);
                var namesToScrap = wait.Until(driver => driver.FindElements(By.XPath("//span[@class='item-card__name']")));
                var pricesV1toScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-price item-card__price']")));
                var pricesV2ToScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-card__info']")));

                Console.Clear();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                for (int i = 0; i < namesToScrap.Count; i++)
                {
                    string name = (string)js.ExecuteScript("return arguments[0].textContent;", namesToScrap[i]);
                    string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV1toScrap[i]);
                    string littlePrice = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV2ToScrap[i]);
                    price = price.Remove(0, 1).Trim() + littlePrice;

                    ScrapCSGO scrapElement = new ScrapCSGO(name, float.Parse(price, CultureInfo.InvariantCulture));

                    if (!scrapRust.Any(x => x.Name == name))
                    {
                        scrapCSGO.Add(scrapElement);
                    }

                    if (scrapElement.PriceCSGOSkinsMonkey < 1)
                    {
                        isToHighPrice = false;
                        break;
                    }

                    var scrollbar = namesToScrap[24];
                    action.MoveToElement(scrollbar).Click().Build().Perform();

                }

                for (int i = 0; i < 3; i++)
                {
                    action.SendKeys(Keys.PageDown).Build().Perform();
                    Thread.Sleep(250);
                }

            } while(isToHighPrice);

        }
        public static async Task ScrapPricesFromSteamMarket(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            Actions action = new Actions(driver);
            driver.Navigate().GoToUrl("https://rust.scmm.app/items");
            await Task.Delay(6500);
            string originalWindow = driver.CurrentWindowHandle;


            foreach (var item in scrapPriceFromRust[counter++])
            {

                int counterOfReadingPrice = 0;
                EnterTextIntoSearch(driver, wait, item.Name);
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
                            EnterTextIntoSearch(driver,wait, item.Name);
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
        private static void EnterTextIntoSearch(IWebDriver driver, WebDriverWait wait, string name)
        {
            var writeItem = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='mud-input-slot mud-input-root mud-input-root-outlined']")));
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
