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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Gmail.v1.Data;
using System.Text.RegularExpressions;
using FlippingSkins.Utils;

namespace FlippingSkins.Login
{
    internal static class LoginWebsites
    {
        private static ConfigInformation? configInformation;

        /// <summary>
        /// Opening Skinsmonkey page
        /// </summary>
        /// <param name="config"></param>
        /// <param name="mode"></param>
        /// <returns>IWebDriver with configuration</returns>
        public static IWebDriver CreatingWeb(ConfigInformation config, int mode)
        {
            configInformation = config;

            IWebDriver driver = new ChromeDriver(Config.options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.skinsmonkey.com");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToSkinsMonkey(driver, wait, mode);

            return driver;
        }

        /// <summary>
        /// Login into skinsmonkey page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="wait"></param>
        /// <param name="mode"></param>
        private static void LoginToSkinsMonkey(IWebDriver driver, WebDriverWait wait, int mode)
        {
            var loginButton = wait.Until(driver => driver.FindElement(By.XPath("//div[@class='base-button auth-button primary']")));
            Actions actions = new Actions(driver);
            actions.MoveToElement(loginButton).Click().Perform();

            System.Threading.Thread.Sleep(2000);
            LoginToSteam(driver, wait);

            System.Threading.Thread.Sleep(2000);
            var sorting = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='form-select__body']")));
            actions.MoveToElement(sorting[3]).Click().Perform();

            System.Threading.Thread.Sleep(500);


            if (mode == 1)
            {
                var element = wait.Until(driver => driver.FindElement(By.XPath("//img[@alt='Rust']")));
                actions.MoveToElement(element).Click().Perform();
            }
            else
            {
                var elements = wait.Until(driver => driver.FindElements(By.XPath("//img[@alt='CS2']")));
                actions.MoveToElement(elements[1]).Click().Perform();
            }
            System.Threading.Thread.Sleep(1500);
        }

        /// <summary>
        /// Login into steam page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="wait"></param>
        private static async void LoginToSteam(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                var login = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='text'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                login.SendKeys(configInformation.loginToSteam);

                var password = wait.Until(driver => driver.FindElement(By.XPath("//input[@type='password'][@class='_2GBWeup5cttgbTw8FM3tfx']")));
                password.SendKeys(configInformation.passwordToSteam);

                var loginButton = wait.Until(driver => driver.FindElement(By.XPath("//button[@class='DjSvCZoKKfoNSmarsEcTS']")));
                Actions actions = new Actions(driver);
                actions.MoveToElement(loginButton).Click().Perform();

                System.Threading.Thread.Sleep(3000);
                GmailGuard();
                for (int i = 0; i < 5; i++)
                {
                    var charInput = wait.Until(driver => driver.FindElement(By.CssSelector("input._3xcXqLVteTNHmk-gh9W65d[value='']")));
                    charInput.SendKeys(configInformation.keyGuard[i].ToString());
                }

                System.Threading.Thread.Sleep(3000);
                var loginIntoSkinsMonkey = wait.Until(driver => driver.FindElement(By.XPath("//input[@class='btn_green_white_innerfade']")));
                actions.MoveToElement(loginIntoSkinsMonkey).Click().Perform();
            }
            catch (WebDriverTimeoutException) { }
            catch (Exception ex) { }

        }

        /// <summary>
        /// Downwriting Steam Guard
        /// </summary>
        private static async void GmailGuard()
        {
            var secrets = new ClientSecrets
            {
                ClientId = Config.GOOGLE_CLIENT_ID,
                ClientSecret = Config.GOOGLE_CLIENT_SECRET,
            };

            var scopes = new[] { GmailService.Scope.GmailReadonly };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore("token_store", true)
                );

            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "FlippingSkins"
            });

            var request = service.Users.Messages.List("me");
            request.MaxResults = 1;

            request.LabelIds = "INBOX";
            request.Q = "is:unread";
            request.IncludeSpamTrash = false;
            var response = await request.ExecuteAsync();

            if (response.Messages != null && response.Messages.Count > 0)
            {
                var msg = response.Messages.First();
                var fullMessage = await service.Users.Messages.Get("me", msg.Id).ExecuteAsync();
                var headers = fullMessage.Payload.Headers;
                var fromHeader = headers.FirstOrDefault(h => h.Name == "From");
                string messageBody = GetPlainTextFromMessage(fullMessage.Payload);
                configInformation.keyGuard = GetCode(messageBody);
            }
            else
            {
                Console.WriteLine("Brak wiadomości.");
            }
            return;
        }

        /// <summary>
        /// Getting string from mail
        /// </summary>
        /// <param name="part"></param>
        /// <returns>Mail body with steam guard</returns>
        public static string GetPlainTextFromMessage(MessagePart part)
        {
            if (part == null)
                return "";

            if (part.MimeType == "text/plain" && part.Body?.Data != null)
            {
                string base64 = part.Body.Data.Replace("-", "+").Replace("_", "/");
                byte[] data = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(data);
            }

            if (part.Parts != null)
            {
                foreach (var subPart in part.Parts)
                {
                    var text = GetPlainTextFromMessage(subPart);
                    if (!string.IsNullOrEmpty(text))
                        return text;
                }
            }

            return "";
        }

        /// <summary>
        /// Getting code from body mail
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Steam Guard</returns>
        private static string GetCode(string message)
        {
            Match match = Regex.Match(message, @"([0-9A-Z]{5})");
            string kodLogowania = "";

            if (match.Success)
            {
                kodLogowania = match.Groups[1].Value;
            }
            return kodLogowania;
        }
    }
}
