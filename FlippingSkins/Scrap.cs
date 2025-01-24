using System;
using System.Collections.Generic;
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
        public static List<ScrapRust> scrap = new List<ScrapRust>();
        public static List<List<ScrapRust>> scrapPriceFromRust;
        public static int counter = 0;
        public static void ScrapPricesAndNamesFromSkinsMonkey(IWebDriver driver)
        {
            bool isToHighPrice = true;
            do
            {
                Thread.Sleep(2000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                var namesToScrap = wait.Until(driver => driver.FindElements(By.XPath("//span[@class='item-card__name']")));
                var pricesV1toScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-price item-card__price']")));
                var pricesV2ToSCrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-card__info']")));

                Console.Clear();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                for (int i = 0; i < namesToScrap.Count; i++)
                {
                    string name = (string)js.ExecuteScript("return arguments[0].textContent;", namesToScrap[i]);
                    string price = (string)js.ExecuteScript("return arguments[0].textContent;", pricesV1toScrap[i]);
                    price = price.Remove(0, 1).Trim();

                    ScrapRust scrapElement = new ScrapRust(name, float.Parse(price, CultureInfo.InvariantCulture));

                    if (!scrap.Any(x => x.Name == name))
                    {
                        scrap.Add(scrapElement);
                    }

                    if (float.Parse(price, CultureInfo.InvariantCulture) < 1.2)
                    {
                        isToHighPrice = false;
                        break;
                    }
                }

                var scrollbar = namesToScrap[24];
                Actions actions = new Actions(driver);
                actions.MoveToElement(scrollbar).Click().Build().Perform();

                for(int i = 0; i < 3; i ++)
                {
                    actions.SendKeys(Keys.PageDown).Build().Perform();
                    Thread.Sleep(250);
                }

            } while (isToHighPrice);
        }
        public static async Task ScrapPricesFromSteamMarket(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            Actions action = new Actions(driver);
            driver.Navigate().GoToUrl("https://rust.scmm.app/items");
            Thread.Sleep(6500);
            string originalWindow = driver.CurrentWindowHandle;

            foreach (var item in scrapPriceFromRust[counter++])
            {
                var writeItem = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='mud-input-slot mud-input-root mud-input-root-outlined']")));
                writeItem.SendKeys(Keys.Control + "a");
                writeItem.SendKeys(Keys.Delete);
                writeItem.SendKeys($"{item.Name}");

                int counterToSkipPage = 0;
                do
                {
                    Thread.Sleep(1000);
                    counterToSkipPage++;
                    var findElement = wait.Until(driver => driver.FindElements(By.XPath($"//span[text()=\"{item.Name}\"]")));

                    if (findElement.Count == 0)
                        continue;

                    if (counterToSkipPage == 4)
                    {
                        counterToSkipPage = 0;
                        var nextPage = wait.Until(driver => driver.FindElement(By.XPath("//button[@aria-label='Next page'][@class='mud-button-root mud-icon-button mud-ripple mud-ripple-icon']")));
                        action.MoveToElement(findElement[0]).Click().Perform();
                    }

                    action.MoveToElement(findElement[0]).Click().Perform();
                    break;

                } while (true);


                foreach (string windowHandle in driver.WindowHandles)
                {
                    if (windowHandle != originalWindow)
                    {
                        driver.SwitchTo().Window(windowHandle);
                        break;
                    }
                }
                Thread.Sleep(700);

                var priceElements = driver.FindElements(By.XPath("//h6[contains(@class, 'mud-typography mud-typography-h6 pa-2')]//span[contains(text(), '$')]"));
                if(priceElements.Count > 0)
                    item.PriceRustSteam = float.Parse(priceElements[0].Text.Remove(0, 1), CultureInfo.InvariantCulture);

                driver.Close();
                driver.SwitchTo().Window(originalWindow);
            }

            driver.Quit();
        }
    }
}
