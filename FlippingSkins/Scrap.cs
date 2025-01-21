using System;
using System.Collections.Generic;
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
        public static List<ScrapElement> scrap = new List<ScrapElement>();
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

                    ScrapElement scrapElement = new ScrapElement(name, float.Parse(price, CultureInfo.InvariantCulture));

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
        public static void ScrapPricesFromSteamMarket(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            driver.Navigate().GoToUrl("https://rust.scmm.app/items");
            Thread.Sleep(6500);
            foreach(var item in scrap)
            {
                var writeItem = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='mud-input-slot mud-input-root mud-input-root-outlined']")));
                writeItem.SendKeys("");
                writeItem.SendKeys($"{item.Name}");
                Thread.Sleep(2000);
            }
        }
    }
}
