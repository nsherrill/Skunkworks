using GP.Shared.Common;
using GP.Shared.Common.DataContracts;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Engines.RemoteDataEngine
{
    public interface IRemoteDataEng
    {
        GPChromeDriver GenerateDriver();
    }

    public class RemoteDataEng : IRemoteDataEng
    {
        private const string LOGINPAGE = @"https://www.fanduel.com/p/login#login";

        public GPChromeDriver GenerateDriver()
        {
            Console.WriteLine("Getting remote driver " + DateTime.Now.ToString());
            var dirDriver = ConfigHelper.ChromeDriverDLLDirectory;
            var driver = new GPChromeDriver(dirDriver);
            try
            {
                driver.Navigate().GoToUrl(LOGINPAGE);
                System.Threading.Thread.Sleep(5000);

                var emailTxtBox = driver.FindElementById("email");
                emailTxtBox.SendKeys(ConfigHelper.FanDuelUserName);
                var passwordTxtBox = driver.FindElementById("password");
                passwordTxtBox.SendKeys(ConfigHelper.FanDuelPassword);
                var loginButton = driver.FindElementByCssSelector(@"#ccf1 > div.column-5.center > div:nth-child(2) > div:nth-child(3) > input");
                loginButton.Click();
                driver.HasLoggedIn = true;
            }
            catch
            {
                driver = null;
            }

            return driver;
        }
    }
}
