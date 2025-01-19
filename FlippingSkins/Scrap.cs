using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FlippingSkins
{
    internal static class Scrap
    {
        public static void ScrapPricesAndNames(IWebDriver driver)
        {
            List<ScrapElement> scrap = new List<ScrapElement>();    
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
                    Console.WriteLine($"name:{name} p1 {price}");
                    Console.ReadKey();

                    Console.WriteLine($"{price}");
                    Console.ReadKey();

                    ScrapElement scrapElement = new ScrapElement(name, float.Parse(price, CultureInfo.InvariantCulture));
                    scrap.Add(scrapElement);
                }
                
            } while (false);
        }
    }
}
