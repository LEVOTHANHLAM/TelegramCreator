using Azure;
using HtmlAgilityPack;
using LDPlayerAndADBController;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net;
using System.Security.Policy;

namespace AppDesptop.TelegramCreator.ScriptsTelegram
{
    public class TelegramAPI
    {
        public async Task<string> LoginPhone(string phone, string usserAgents, string? proxyUrl)
        {
            try
            {
                var httpClientHandler = new HttpClientHandler();
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
                        httpClientHandler.Proxy = proxy;
                    }
                }
                // Tạo đối tượng HttpClient
                using var client = new HttpClient(httpClientHandler);
                // string usserAgents = FileHelper.GetUserAgents();
                // Thêm Headers vào HttpClient
                client.DefaultRequestHeaders.Add("User-Agent", usserAgents);

                // Tạo form data
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(phone), "phone");

                // Gửi yêu cầu POST
                var response = await client.PostAsync("https://my.telegram.org/auth/send_password", formData);

                // Đọc và hiển thị dữ liệu trả về nếu yêu cầu thành công
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseBody);

                    // Kiểm tra xem có thuộc tính "random_hash" hay không
                    if (jsonResponse["random_hash"] != null)
                    {
                        string randomHashValue = jsonResponse["random_hash"].ToString();
                        return randomHashValue;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"Lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(TelegramAPI)}, params; {nameof(LoginPhone)}, Error; {ex.Message}, Exception; {ex}");
            }
            return null;
        }
        public async Task<string> GetCookie(string phone, string usserAgents, string hash, string code, string? proxyUrl)
        {
            try
            {
                var httpClientHandler = new HttpClientHandler();
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
                        httpClientHandler.Proxy = proxy;
                    }
                }
                // Tạo đối tượng HttpClient
                using var client = new HttpClient(httpClientHandler);
                // string usserAgents = FileHelper.GetUserAgents();
                // Thêm Headers vào HttpClient
                client.DefaultRequestHeaders.Add("User-Agent", usserAgents);

                // Tạo form data
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(phone), "phone");
                formData.Add(new StringContent(hash), "random_hash");
                formData.Add(new StringContent(code), "password");
                // Gửi yêu cầu POST
                var response = await client.PostAsync("https://my.telegram.org/auth/login", formData);

                // Đọc và hiển thị dữ liệu trả về nếu yêu cầu thành công
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (responseBody.Contains("true"))
                    {
                        var cookies = httpClientHandler.CookieContainer.GetCookies(new Uri("https://my.telegram.org/apps"));
                        // Trả về cookie dưới dạng chuỗi
                        string cookieString = string.Join(";", cookies.Cast<Cookie>().Select(c => $"{c.Name}={c.Value}"));
                        return cookieString;
                    }
                }
                else
                {
                    Console.WriteLine($"Lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(TelegramAPI)}, params; {nameof(LoginPhone)}, Error; {ex.Message}, Exception; {ex}");
            }
            return null;
        }
        public async Task<string> GetHash(string cookie, string usserAgents, string? proxyUrl)
        {
            try
            {
                var httpClientHandler = new HttpClientHandler();
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
                        httpClientHandler.Proxy = proxy;
                    }
                }
                // Tạo đối tượng HttpClient
                using var client = new HttpClient(httpClientHandler);
                // string usserAgents = FileHelper.GetUserAgents();
                // Thêm Headers vào HttpClient
                client.DefaultRequestHeaders.Add("User-Agent", usserAgents);
                // Gửi yêu cầu POST
                if (!string.IsNullOrEmpty(cookie))
                {
                    client.DefaultRequestHeaders.Add("cookie", cookie);
                    //client.DefaultRequestHeaders.Add("User-Agent", usserAgents);
                }
                try
                {
                    var response = await client.GetAsync("https://my.telegram.org/apps");
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(responseBody);
                    ///
                    var title = doc.DocumentNode.SelectSingleNode("//title").InnerText;

                    if (title == "Create new application")
                    {
                        var hashInput = doc.DocumentNode.SelectSingleNode("//input[@name='hash']");
                        var hash = hashInput.GetAttributeValue("value", "");
                        if (!string.IsNullOrEmpty(hash))
                        {
                            return hash;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(TelegramAPI)}, params; {nameof(LoginPhone)}, Error; {ex.Message}, Exception; {ex}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(TelegramAPI)}, params; {nameof(LoginPhone)}, Error; {ex.Message}, Exception; {ex}");
            }
            return null;

        }
        public async Task<string> GetApiIdAndHash(string cookie, string hash, string? proxyUrl, string usserAgents)
        {
            int i = 3;
            while (i > 0)
            {
                i--;
                try
                {

                    // Tạo đối tượng HttpClientHandler với proxy nếu có
                    var httpClientHandler = new HttpClientHandler();
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
                            httpClientHandler.Proxy = proxy;
                        }
                    }
                    using var client = new HttpClient(httpClientHandler);
                    Random random = new Random();
                    string app_title = Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz", random.Next(3, 5)) + Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", random.Next(5, 10));
                    string app_shortname = Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz", random.Next(3, 5)) + Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", random.Next(5, 10));
                    string app_url = "www.tegram.org";
                    string app_platform = "desktop";
                    string app_desc = Helpers.GenerateRandomString("0123456789", random.Next(10, 20));
                    var formData = new MultipartFormDataContent();
                    formData.Add(new StringContent(hash), "hash");
                    formData.Add(new StringContent(app_title), "app_title");
                    formData.Add(new StringContent(app_shortname), "app_shortname");
                    formData.Add(new StringContent(app_url), "app_url");
                    formData.Add(new StringContent(app_platform), "app_platform");
                    formData.Add(new StringContent(app_desc), "app_desc");
                    // Gửi yêu cầu POST
                    var request2 = new HttpRequestMessage(HttpMethod.Post, "https://my.telegram.org/apps/create");
                    request2.Headers.Add("cookie", cookie);
                    request2.Content = formData;
                    var response2 = await client.SendAsync(request2);
                    response2.EnsureSuccessStatusCode();
                    var body = await response2.Content.ReadAsStringAsync();
                    // Đọc và hiển thị dữ liệu trả về nếu yêu cầu thành công
                    if (response2.IsSuccessStatusCode)
                    {
                        try
                        {
                            var response = await client.GetAsync("https://my.telegram.org/apps");
                            response.EnsureSuccessStatusCode();
                            var responseBody = await response.Content.ReadAsStringAsync();
                            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(responseBody);
                            // Extracting the hash value
                            var spans = doc.DocumentNode.Descendants("span")
               .Where(span => span.GetAttributeValue("class", "") == "form-control input-xlarge uneditable-input")
               .Select(span => span.InnerText)
               .ToList();
                            // Lấy giá trị api_id và api_hash
                            string apiId = spans.FirstOrDefault();
                            string apiHash = spans.Skip(1).FirstOrDefault();
                            if (!string.IsNullOrEmpty(apiId) && !string.IsNullOrEmpty(apiHash))
                            {
                                return $"{apiId}|{apiHash}";
                            }
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(GetApiIdAndHash)}, Error; {ex.Message}, Exception; {ex}");
                }
            }

            return null;
        }
    }
}
