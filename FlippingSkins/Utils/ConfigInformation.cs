using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippingSkins.Utils
{
    public class ConfigInformation
    {
        public string loginToSteam { get; private set; }
        public string passwordToSteam { get; private set; }
        public string loginToGmail { get; private set; }
        public string passwordToGmail { get; private set; }
        public string keyGuard { get; set; }
        public ConfigInformation(string loginToSteam, string passwordToSteam, string loginToGmail, string passwordToGmail)
        {
            this.loginToSteam = loginToSteam;
            this.passwordToSteam = passwordToSteam;
            this.loginToGmail = loginToGmail;
            this.passwordToGmail = passwordToGmail;
        }
        public ConfigInformation() { }
    }
}
