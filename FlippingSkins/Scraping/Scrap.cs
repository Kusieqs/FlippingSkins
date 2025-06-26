using System.Diagnostics;
using System.Globalization;
using FlippingSkins.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace FlippingSkins.Scraping
{
    internal static class Scrap
    {

        public static List<ScrapRust> scrapRust = new List<ScrapRust>();
        public static List<List<ScrapRust>> scrapPriceFromRust;
        public static List<ScrapCSGO> scrapCSGO = new List<ScrapCSGO>();
        public static List<List<ScrapCSGO>> scrapPriceFromCSGO;
        public static int counter = 0;

        /// <summary>
        /// Scraping prices from RUST items
        /// </summary>
        /// <param name="driver"></param>
        public static void ScrapPricesAndNamesFromSkinsMonkey_Rust(IWebDriver driver)
        {
            Actions action = new Actions(driver);
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

                    if (scrapElement.PriceRustSkinsMonkey < 4)
                    {
                        isToHighPrice = false;
                        break;
                    }
                }

                var scrollbar = namesToScrap[19];
                action.MoveToElement(scrollbar).Click().Build().Perform();

                for (int i = 0; i < 3; i++)
                {
                    action.SendKeys(Keys.PageDown).Build().Perform();
                    Thread.Sleep(250);
                }

                RemoveElement(action, wait, driver);
            } while (isToHighPrice);

        }

        /// <summary>
        /// Scraping prices from CSGO items
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="priceSort"></param>
        public static void ScrapPricesAndNamesFromSkinsMonkey_CSGO(IWebDriver driver, float priceSort)
        {
            Actions action = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            var sorting = wait.Until(driver => driver.FindElements(By.XPath("//input[@class='form-input__core']")));
            ;
            SetSorting(action, sorting[2], priceSort);
            SetQuality(action, wait, driver);
            bool isToLowPrice = true;
            Thread.Sleep(5000);
            bool firstTime = true;

            do
            {
                Thread.Sleep(2000);
                var pricesV1toScrap = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='item-card__bottom']//div[@class='item-card__info']//div[@class='item-price item-card__price']")));
                var element = wait.Until(driver => driver.FindElements(By.CssSelector("img.item-image")));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                int loop = firstTime ? 4 : pricesV1toScrap.Count;
                firstTime = false;

                for (int j = 0; j < loop; j++)
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

                    if (scrapElement.PriceCSGOSkinsMonkey < 0.4 || scrapCSGO.Count == Config.MAX_ITEM_NUMBER)
                    {
                        isToLowPrice = false;
                        break;
                    }

                }

                int timer = 0;
                while (isToLowPrice)
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

                RemoveElement(action, wait, driver);


            } while (isToLowPrice);

        }

        /// <summary>
        /// Set sorting price
        /// </summary>
        /// <param name="action"></param>
        /// <param name="sorting"></param>
        /// <param name="tupleNumber"></param>
        private static void SetSorting(Actions action, IWebElement sorting, float tupleNumber)
        {
            action.Click(sorting).Build().Perform();
            sorting.SendKeys(tupleNumber.ToString());
        }

        /// <summary>
        /// Set quality checkboxes
        /// </summary>
        /// <param name="action"></param>
        /// <param name="wait"></param>
        /// <param name="driver"></param>
        private static void SetQuality(Actions action, WebDriverWait wait, IWebDriver driver)
        {
            var sorting = wait.Until(driver => driver.FindElement(By.XPath("//div[@class='trade-collapse trade-filter-exterior']")));
            action.Click(sorting).Build().Perform();
            Thread.Sleep(2000);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var checkbox = wait.Until(driver => driver.FindElements(By.XPath("//span[@class='trade-filter-option-generic__label']")));
            for (int i = 2; i < 7; i++)
            {
                js.ExecuteScript("arguments[0].click();", checkbox[i]);
            }

        }

        /// <summary>
        /// Set price to sort
        /// </summary>
        /// <returns>Maximum price</returns>
        public static float SetPriceForCSGO()
        {
            float highPrice = 0.0f;

            do
            {
                Console.Clear();
                Console.Write("Set the price: ");
                if (float.TryParse(Console.ReadLine().Replace('.', ','), out highPrice)
                    && highPrice <= 100f
                    && highPrice >= 0.41f)
                {
                    break;
                }

            } while (true);
            highPrice = (float)Math.Round(highPrice, 2);
            Console.Clear();

            return highPrice;
        }

        /// <summary>
        /// Removing item from box to buy
        /// </summary>
        /// <param name="action"></param>
        /// <param name="wait"></param>
        /// <param name="driver"></param>
        private static void RemoveElement(Actions action, WebDriverWait wait, IWebDriver driver)
        {
            var itemToRemoves = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='cart-static-item__remove']")));
            if (itemToRemoves.Count > 0)
            {
                action.Click(itemToRemoves[0]).Build().Perform();
            }
        }
    }
}
