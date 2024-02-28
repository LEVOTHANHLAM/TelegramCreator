using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace AppDesptop.TelegramCreator.ScriptsTelegram
{
    public class TelegramApplication
    {
        private string phoneNumber;
        private string appTitle;
        private string appShortname;
        private string appUrl;
        private string appPlatform;
        private string appDesc;
        private string randomHash;
        private string stelToken;
        private string useragent;

        public TelegramApplication(
            string phone_number,
            string app_title = "",
            string app_shortname = "",
            string app_url = "",
            string app_platform = "desktop",
            string app_desc = "",
            string random_hash = null,
            string stel_token = null,
            string user_agent = "")
        {
            phoneNumber = phone_number;
            appTitle = app_title;
            appShortname = app_shortname;
            appUrl = app_url;
            appPlatform = app_platform;
            appDesc = app_desc;
            randomHash = random_hash;
            stelToken = stel_token;
            useragent = user_agent;
        }

        public bool SendPassword()
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                client.Headers[HttpRequestHeader.Accept] = "application/json, text/javascript, */*; q=0.01";
                client.Headers[HttpRequestHeader.Referer] = "https://my.telegram.org/auth";
                client.Headers[HttpRequestHeader.UserAgent] = useragent;

                string postData = $"phone={phoneNumber}";
                byte[] response = client.UploadData("https://my.telegram.org/auth/send_password", "POST", Encoding.UTF8.GetBytes(postData));

                string result = Encoding.UTF8.GetString(response);
                dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                randomHash = jsonData["random_hash"];
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool AuthLogin(string cloudPassword)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                client.Headers[HttpRequestHeader.Accept] = "application/json, text/javascript, */*; q=0.01";
                client.Headers[HttpRequestHeader.Referer] = "https://my.telegram.org/auth";
                client.Headers[HttpRequestHeader.UserAgent] = useragent;

                string postData = $"phone={phoneNumber}&random_hash={randomHash}&password={cloudPassword}";
                byte[] response = client.UploadData("https://my.telegram.org/auth/login", "POST", Encoding.UTF8.GetBytes(postData));

                string result = Encoding.UTF8.GetString(response);
                stelToken = client.ResponseHeaders["Set-Cookie"];
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public (string, string) AuthApp()
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.Cookie] = $"stel_token={stelToken}";
                client.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                client.Headers[HttpRequestHeader.UserAgent] = useragent;
                client.Headers[HttpRequestHeader.Referer] = "https://my.telegram.org/org";

                byte[] responseBytes = client.DownloadData("https://my.telegram.org/apps");
                string response = Encoding.UTF8.GetString(responseBytes);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                HtmlNodeCollection apiNodes = doc.DocumentNode.SelectNodes("//span[@class='form-control input-xlarge uneditable-input']//text()");
                string api1 = apiNodes[0].InnerText;
                string api2 = apiNodes[1].InnerText;

                return (api1, api2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (null, null);
            }
        }
    }
}
