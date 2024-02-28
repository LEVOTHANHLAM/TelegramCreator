using LDPlayerAndADBController;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace AppDesptop.TelegramCreator.ScriptsTelegram
{
    class TelegramServices
    {
        private readonly string phoneNumber;

        public TelegramServices(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public string GetRandomHash(string? proxyUrl)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (!string.IsNullOrEmpty(proxyUrl))
                    {
                        var proxyParts = proxyUrl.Split(':');
                        var Address = proxyParts[0];
                        var Port = int.Parse(proxyParts[1]);
                        var Username = proxyParts.Length > 2 ? proxyParts[2] : null;
                        var Password = proxyParts.Length > 3 ? proxyParts[3] : null;
                        if (!string.IsNullOrEmpty(Address) && Port > 0)
                        {
                            var proxy = new WebProxy($"http://{Address}:{Port}");
                            if (!string.IsNullOrEmpty(Username))
                            {
                                proxy.Credentials = new NetworkCredential(Username, Password);
                            }
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            client.Proxy = proxy; // Gán proxy cho WebClient
                        }
                    }

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var data = new System.Collections.Specialized.NameValueCollection
            {
                { "phone", this.phoneNumber }
            };

                    // Chuyển dữ liệu từ NameValueCollection sang mảng byte khi gửi yêu cầu
                    var response = client.UploadValues("https://my.telegram.org/auth/send_password", "POST", data);

                    // Chuyển đổi mảng byte nhận được thành chuỗi để xử lý kết quả
                    var responseString = System.Text.Encoding.Default.GetString(response);
                    dynamic result = JsonConvert.DeserializeObject(responseString);
                    string randomHash = result.random_hash;
                    return randomHash;
                }
            }
            catch (Exception e)
            {
                // Xử lý ngoại lệ ở đây
                return null;
            }
        }


        public Tuple<string, string> GetApiIdAndHashId(string randomHash, string codeApp, string? proxyUrl)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (!string.IsNullOrEmpty(proxyUrl))
                    {
                        var proxyParts = proxyUrl.Split(':');
                        var Address = proxyParts[0];
                        var Port = int.Parse(proxyParts[1]);
                        var Username = proxyParts.Length > 2 ? proxyParts[2] : null;
                        var Password = proxyParts.Length > 3 ? proxyParts[3] : null;
                        if (!string.IsNullOrEmpty(Address) && Port > 0)
                        {
                            var proxy = new WebProxy($"http://{Address}:{Port}");
                            if (!string.IsNullOrEmpty(Username))
                            {
                                proxy.Credentials = new NetworkCredential(Username, Password);
                            }
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            client.Proxy = proxy; // Gán proxy cho WebClient
                        }
                    }

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var data = new System.Collections.Specialized.NameValueCollection
                    {
                        { "phone", this.phoneNumber },
                        { "random_hash", randomHash },
                        { "password", codeApp }
                    };
                    Random random = new Random();
                    var response = client.UploadValues("https://my.telegram.org/auth/login", data);
                    var cookie = client.ResponseHeaders["Set-Cookie"];

                    client.Headers[HttpRequestHeader.Cookie] = cookie;

                    var app = client.DownloadString("https://my.telegram.org/apps");

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(app);

                    var title = doc.DocumentNode.SelectSingleNode("//title").InnerText;

                    if (title == "Create new application")
                    {
                        var hashInput = doc.DocumentNode.SelectSingleNode("//input[@name='hash']");
                        string hash = hashInput.GetAttributeValue("value", "");

                        var appInfo = new System.Collections.Specialized.NameValueCollection
                        {
                            { "hash", hash },
                            { "app_title", $"Telegram Application {this.phoneNumber}" },
                            { "app_shortname", $"AppBotTelegram{Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", random.Next(5, 10))}" },
                            { "app_url", "" },
                            { "app_platform", "other" },
                            { "app_desc", $"AppBotTelegram{Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", random.Next(5, 10))}" }
                        };

                        var createAppResponse = client.UploadValues("https://my.telegram.org/apps/create", appInfo);
                        string createAppResult = System.Text.Encoding.Default.GetString(createAppResponse);

                        if (createAppResult == "ERROR")
                        {
                            return new Tuple<string, string>("", "");
                        }

                        var newApp = client.DownloadString("https://my.telegram.org/apps");
                        var newDoc = new HtmlAgilityPack.HtmlDocument();
                        newDoc.LoadHtml(newApp);

                        var gInputs = newDoc.DocumentNode.SelectNodes("//span[@class='form-control input-xlarge uneditable-input']");

                        string apiId = gInputs[0].InnerText;
                        string apiHash = gInputs[1].InnerText;

                        return new Tuple<string, string>(apiId, apiHash);
                    }
                    else if (title == "App configuration")
                    {
                        var gInputs = doc.DocumentNode.SelectNodes("//span[@class='form-control input-xlarge uneditable-input']");

                        string apiId = gInputs[0].InnerText;
                        string apiHash = gInputs[1].InnerText;

                        return new Tuple<string, string>(apiId, apiHash);
                    }
                    else
                    {
                        return new Tuple<string, string>("", "");
                    }
                }
            }
            catch (Exception e)
            {
                // Handle exception here
                return new Tuple<string, string>("", "");
            }
        }
    }
}
