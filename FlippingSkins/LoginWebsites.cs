using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace FlippingSkins
{
    internal static class LoginWebsites
    {
        private static ChromeOptions options = new ChromeOptions();
        public static void CreatingWeb()
        {
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");

            IWebDriver driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.skinsmonkey.com");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToSkinsMonkey(driver, wait);
        }

        private static void LoginToSkinsMonkey(IWebDriver driver,WebDriverWait wait)
        {
            try
            {
                var loginButton = wait.Until(driver => driver.FindElement(By.XPath("//div[@class='base-button auth-button primary']")));
                Actions actions = new Actions(driver);
                actions.MoveToElement(loginButton).Click().Perform();

                Thread.Sleep(2000);
                LoginToSteam(driver, wait);
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
        }

        private static void LoginToSteam(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                var login = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                login.SendKeys("");

                var password = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='password'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                password.SendKeys("");

                var loginButton = wait.Until(driver => driver.FindElement(By.XPath("//button[@class='DjSvCZoKKfoNSmarsEcTS']")));
                Actions actions = new Actions(driver);
                actions.MoveToElement(loginButton).Click().Perform();

                string guard = GmailGuard();
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

        }

        private static string GmailGuard()
        {
            IWebDriver gmail = new ChromeDriver(options);
            gmail.Manage().Window.Maximize();
            gmail.Navigate().GoToUrl("https://workspace.google.com/intl/pl/gmail/");
            WebDriverWait wait = new WebDriverWait(gmail, TimeSpan.FromSeconds(5));

            var enterLogin = wait.Until(gmail => gmail.FindElement(By.XPath("//a[@class='button button--medium header__aside__button button--desktop button--tablet button--mobile']")));
            Actions clickButton = new Actions(gmail);
            clickButton.MoveToElement(enterLogin).Click().Perform();

            var email = wait.Until(gmail => gmail.FindElement(By.XPath("//input[@class='whsOnd zHQkBf']")));
            email.SendKeys("");

            var enterEmail = wait.Until(gmail => gmail.FindElement(By.XPath("//button[@class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 BqKGqe Jskylb TrZEUc lw1w4b']")));
            clickButton.MoveToElement(enterEmail).Click().Perform();

            Thread.Sleep(5000);
            var password = wait.Until(gmail => gmail.FindElement(By.XPath("//input[@class='whsOnd zHQkBf']")));
            password.SendKeys("");

            var enterPassword = wait.Until(gmail => gmail.FindElement(By.XPath("//button[@class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 BqKGqe Jskylb TrZEUc lw1w4b']")));
            clickButton.MoveToElement(enterPassword).Click().Perform();

            return null;
        }
    }
}
