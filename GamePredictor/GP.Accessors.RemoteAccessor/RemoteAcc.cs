using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GP.Accessors.RemoteAccessor
{
    public interface IRemoteAcc
    {
        string GetPageContent(string url);

        //string Login(string loginURL, string postURL, string destinationURL, RemoteAcc.PostDataParser postDataParser);
        //string Login(string loginURL, string postURL, string destinationURL, RemoteAcc.PostDataParser postDataParser);
    }

    public class RemoteAcc : IRemoteAcc
    {
        public delegate string PostDataParser(string htmlToParse);

        //public string Login(string loginURL, string postURL, string destinationURL, RemoteAcc.PostDataParser postDataParser)
        //{
        //    using (var driver = new ChromeDriver(@"C:\workspaces\GamePredictor\DLLs"))
        //    {
        //        try
        //        {
        //            driver.Navigate().GoToUrl(loginURL);
        //            var emailTxtBox = driver.FindElementById("email");
        //            emailTxtBox.SendKeys("ungusc73@yahoo.com");
        //            var passwordTxtBox = driver.FindElementById("password");
        //            passwordTxtBox.SendKeys("Mackelv!3n");
        //            var loginButton = driver.FindElementByXPath("//*[@id=\"ccf0\"]/div[2]/div[2]/div[3]/input");
        //            loginButton.Click();

        //            var closeModalButton = driver.FindElementByXPath("//*[@id=\"body\"]/div/div[2]/a");
        //            if(closeModalButton != null)
        //                closeModalButton.Click();

        //            var allElements = driver.FindElementsByXPath("//*[@id=\"body\"]/section/div[2]/div[2]/div[5]/div/div[2]/div[2]/table/tbody/tr");

        //            var trParents = driver.FindElementsByClassName("lobbyitem standard");
        //            //driver.Manage().
        //        }
        //        catch (Exception e)
        //        {
        //            driver.Close();
        //            driver.Quit();
        //        }
        //    }
            ////var selenium = new DefaultSelenium("localhost", 80, "*firefox", loginURL);
            //var command = new Selenium.HttpCommandProcessor("localhost", @"c:\program files\internet explorer\iexplore.exe", "www.google.com");
            //var selenium = new DefaultSelenium(command);//"localhost", 80, "*firefox", "www.google.com");
            //try
            //{
            //    selenium.Start();
            //    selenium.WaitForPageToLoad("10000");

            //    selenium.Type("email", "ungusc73@yahoo.com");
            //    selenium.Type("password", "Mackelv!3n");
            //    selenium.Submit("ccf0");

            //    selenium.WaitForPageToLoad("10000");
            //    var text = selenium.GetBodyText();
            //    //selenium.GetTable("css=table#simpleTable")

            //}
            //catch
            //{
            //    selenium.Stop();
            //}
        //    return null;
        //}

        public string GetPageContent(string url)
        {
            try
            {
                WebRequest getReq = WebRequest.Create(url);
                using (Stream objStream = getReq.GetResponse().GetResponseStream())
                {
                    using (StreamReader objReader = new StreamReader(objStream))
                    {
                        var result = objReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        ////string url = "https://twitter.com/login";
        ////string username = "someUserName";
        ////string password = "somePassword";
        ////string commit = "Sign+In"; //this matches the data from Tamper Data

        //public string Login(string loginURL, string postURL, string destinationURL, RemoteAcc.PostDataParser postDataParser)
        //{
        //    string results = null;

        //    string postData = string.Empty;

        //    var loginRequest = WebRequest.Create(loginURL) as HttpWebRequest;
        //    using (var loginResponse = loginRequest.GetResponse())
        //    {
        //        using (var respStrm = loginResponse.GetResponseStream())
        //        {
        //            using (var sr = new StreamReader(respStrm))
        //            {
        //                var loginPage = sr.ReadToEnd();
        //                postData = postDataParser(loginPage);
        //            }
        //        }
        //    }

        //    HttpWebRequest postRequest = WebRequest.Create(postURL) as HttpWebRequest;
        //    postRequest.KeepAlive = true;
        //    postRequest.Method = "POST";
        //    postRequest.Host = "www.fanduel.com";
        //    postRequest.Accept = "text/html";
        //    postRequest.Referer = "https://www.fanduel.com/p/login";
        //    postRequest.ContentType = "multipart/form-data";
        //    //string postData = "FormNameForUserId=" + strUserId + "&FormNameForPassword=" + strPassword;
        //    byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(postData);
        //    postRequest.ContentLength = dataBytes.Length;
        //    using (Stream postStream = postRequest.GetRequestStream())
        //    {
        //        postStream.Write(dataBytes, 0, dataBytes.Length);
        //    }
        //    HttpWebResponse postCredsResponse = postRequest.GetResponse() as HttpWebResponse;

        //    // Probably want to inspect the http.Headers here first

        //    var destinationRequest = WebRequest.Create(destinationURL) as HttpWebRequest;
        //    destinationRequest.KeepAlive = true;
        //    destinationRequest.Method = "GET";
        //    destinationRequest.Host = "www.fanduel.com";
        //    destinationRequest.Accept = "text/html";
        //    destinationRequest.Referer = "https://www.fanduel.com/p/login";
        //    //destinationRequest.ContentType = "multipart/form-data";
        //    destinationRequest.CookieContainer = new CookieContainer();
        //    destinationRequest.CookieContainer.Add(postCredsResponse.Cookies);
        //    HttpWebResponse destinationResponse = destinationRequest.GetResponse() as HttpWebResponse;
        //    using (var stream = destinationResponse.GetResponseStream())
        //    {
        //        using (StreamReader sr = new StreamReader(stream))
        //        {
        //            results = sr.ReadToEnd();
        //        }
        //    }


        //    return results;


        //    //WebBrowser b = new WebBrowser();
        //    //b.Tag = new string[] { destinationURL, postData };
        //    //b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted);
        //    //b.Navigate(loginURL);
        //}

        //private void b_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    WebBrowser b = sender as WebBrowser;
        //    string[] tagData = b.Tag as string[];
        //    string destURL = tagData[0];
        //    string postData = tagData[1];
        //    string response = b.DocumentText;

        //    // looks in the page source to find the authenticity token.
        //    // could also use regular exp<b></b>ressions here.
        //    int index = response.IndexOf("authenticity_token");
        //    int startIndex = index + 41;
        //    string authenticityToken = response.Substring(startIndex, 40);

        //    // unregisters the first event handler
        //    // adds a second event handler
        //    b.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted);
        //    b.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted2);

        //    // format our data that we are going to post to the server
        //    // this will include our post parameters.  They do not need to be in a specific
        //    //	order, as long as they are concatenated together using an ampersand ( & )
        //    //string postData = string.Format("authenticity_token={2}&session[username_or_email]={0}&session[password]={1}&commit={3}", username, password, authenticityToken, commit);

        //    ASCIIEncoding enc = new ASCIIEncoding();

        //    //  we are encoding the postData to a byte array
        //    //b.Navigate("https://twitter.com/sessions", "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
        //    b.Navigate(destURL, "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
        //}

        //private void b_DocumentCompleted2(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    WebBrowser b = sender as WebBrowser;
        //    string response = b.DocumentText;

        //    if (response.Contains("Sign out"))
        //    {
        //        MessageBox.Show("Login Successful");
        //    }
        //}
    }
}
