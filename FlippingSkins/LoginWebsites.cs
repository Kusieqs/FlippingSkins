using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Runtime.CompilerServices;

namespace FlippingSkins
{
    internal static class LoginWebsites
    {
        private static ChromeOptions options = new ChromeOptions();
        private static ConfigInformation? configInformation;
        public static IWebDriver CreatingWeb(ConfigInformation config)
        {
            configInformation = config;
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");

            IWebDriver driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.skinsmonkey.com");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToSkinsMonkey(driver, wait);

            return driver;
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

                Thread.Sleep(4000);
                var sorting = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='form-select__body']")));
                actions.MoveToElement(sorting[3]).Click().Perform();

                Thread.Sleep(500);
                var clickRust = wait.Until(driver => driver.FindElement(By.XPath("//img[@alt='Rust']")));
                actions.MoveToElement(clickRust).Click().Perform();
                Thread.Sleep(1500);

            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine("Nie udało się załadować strony logowania Steam lub nie znaleziono wymaganych elementów.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex);
            }
        }

        private static void LoginToSteam(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                var login = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                login.SendKeys(configInformation.loginToSteam);

                var password = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='password'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                password.SendKeys(configInformation.passwordToSteam);

                var loginButton = wait.Until(driver => driver.FindElement(By.XPath("//button[@class='DjSvCZoKKfoNSmarsEcTS']")));
                Actions actions = new Actions(driver);
                actions.MoveToElement(loginButton).Click().Perform();

                string guard = GmailGuard();
                configInformation.keyGuard = guard;

                for(int i = 0; i < 5; i++)
                {
                    var charInput = wait.Until(driver => driver.FindElement(By.CssSelector("input._3xcXqLVteTNHmk-gh9W65d[value='']")));
                    charInput.SendKeys(guard[i].ToString());
                }

                Thread.Sleep(2000);
                var loginIntoSkinsMonkey = wait.Until(driver => driver.FindElement(By.XPath("//input[@class='btn_green_white_innerfade']")));
                actions.MoveToElement(loginIntoSkinsMonkey).Click().Perform();
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine("Nie udało się załadować strony logowania Steam lub nie znaleziono wymaganych elementów.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex);
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
            email.SendKeys(configInformation.loginToGmail);

            var enterEmail = wait.Until(gmail => gmail.FindElement(By.XPath("//button[@class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 BqKGqe Jskylb TrZEUc lw1w4b']")));
            clickButton.MoveToElement(enterEmail).Click().Perform();

            Thread.Sleep(4000);
            var password = wait.Until(gmail => gmail.FindElement(By.XPath("//input[@class='whsOnd zHQkBf']")));
            password.SendKeys(configInformation.passwordToGmail);

            var enterPassword = wait.Until(gmail => gmail.FindElement(By.XPath("//button[@class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 BqKGqe Jskylb TrZEUc lw1w4b']")));
            clickButton.MoveToElement(enterPassword).Click().Perform();

            Thread.Sleep(4500);
            var enterMessage = wait.Until(gmail => gmail.FindElement(By.XPath("//tr[@class='zA zE']")));
            clickButton.MoveToElement(enterMessage).Click().Perform();

            Thread.Sleep(1000);
            var elements = gmail.FindElements(By.XPath("//img[@class='ajT']"));
            if(elements.Count > 0)
            {
                clickButton.MoveToElement(elements[0]).Click().Perform();
            }

            Thread.Sleep(500);
            var guard = wait.Until(gmail => gmail.FindElement(By.CssSelector("td[style*='font-size:48px'][style*='font-family:Arial']")));
            string keyGuard = guard.Text;
            gmail.Quit();
            
            return keyGuard;
        }
    }
}
