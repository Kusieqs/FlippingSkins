using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Interactions;

namespace FlippingSkins
{
    public static class Scrap
    {
        public static void SettingFiltr()
        {
            // Zanotowanie selenium
            var options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);


            try
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl("https://www.skinsmonkey.com");
                

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var loginButton = wait.Until(driver => driver.FindElement(By.XPath("//div[@class='base-button auth-button primary']")));

                
                Actions actions = new Actions(driver);
                actions.MoveToElement(loginButton).Click().Perform();
                Thread.Sleep(2000);

                
                var login = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                login.SendKeys("");

                var password = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='password'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                password.SendKeys("");

            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine("Nie udało się załadować strony logowania Steam lub nie znaleziono wymaganych elementów.");
                driver.Quit();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error " + ex);
                driver.Quit();
            }
            Console.ReadKey();
        }
    }
}
