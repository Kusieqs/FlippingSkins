using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace FlippingSkins.Utils
{
    public static class Config
    {
        public static ChromeOptions options = new ChromeOptions();
        public const int MAX_ITEM_NUMBER = 200;
        public const string LOGIN_TO_STEAM = "flipingSkins";
        public const string PASSWORD_TO_STEAM = "vR5QKwJ252H%kpu";
        public const string GMAIL = "flippingskins@gmail.com";
        public const string PASSWORD_TO_GMAIL = "FlippingSkins123";
        public const string GOOGLE_CLIENT_ID = "71031490539-phrkft1pgbmffljk7vsgr1mcke5o2hl9.apps.googleusercontent.com";
        public const string GOOGLE_CLIENT_SECRET = "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";


        /// <summary>
        /// Setting config to login into steam and gmail
        /// </summary>
        /// <returns>ConfigInformation object</returns>
        public static ConfigInformation SetConfig() => new ConfigInformation(LOGIN_TO_STEAM, PASSWORD_TO_STEAM, GMAIL, PASSWORD_TO_GMAIL);

        /// <summary>
        /// Special arguments to set chrome
        /// </summary>
        public static void StartConfig()
        {
            //options.AddArgument("--headless");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--remote-allow-origins=*");
            options.AddArgument("--ignore-certificate-errors");
        }

    }
}
