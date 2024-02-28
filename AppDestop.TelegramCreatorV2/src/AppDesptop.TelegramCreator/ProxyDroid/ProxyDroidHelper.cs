using AppDesptop.TelegramCreator.ProxyDroid.Interface;
using Serilog;
using System.Net;

namespace AppDesptop.TelegramCreator.ProxyDroid
{
    public class ProxyDroidHelper : IProxyDroidHelper
    {
        public async Task<bool> CheckPorxy(string host, string port, string? username, string? password, bool http)
        {
            try
            {
                string proxyAddress = host + ":" + port;
                // Tạo một WebRequest sử dụng proxy
                WebRequest request = WebRequest.Create("https://www.google.com/");
                request.Proxy = new WebProxy(proxyAddress);
                {
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        // Xác thực proxy nếu cần thiết
                        request.Proxy.Credentials = new NetworkCredential(username, password);
                    }
                    if (http == false)
                    {
                        // Sử dụng SOCKS5 proxy
                        ((WebProxy)request.Proxy).UseDefaultCredentials = false;
                        ((WebProxy)request.Proxy).BypassProxyOnLocal = false;
                    }
                    // Thực hiện một yêu cầu GET đơn giản để kiểm tra proxy
                    using (WebResponse response = request.GetResponse())
                    {
                        // Kiểm tra mã trạng thái HTTP để xác định tính hợp lệ của proxy
                        HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;
                        return (int)statusCode >= 200 && (int)statusCode < 300;
                    }
                }
            }
            catch (WebException)
            {
                // Xảy ra lỗi khi yêu cầu sử dụng proxy
                return false;
            }
        }

        public async Task<List<ProxyInfo>> ReadFileProxy(string filePath)
        {
            List<ProxyInfo> proxies = new List<ProxyInfo>();
            try
            {
                var proxyfile = File.ReadAllLines(filePath);
                Log.Error("AddFileproxy " + filePath + " \n" + proxyfile.Length);
                for (int i = 0; i < proxyfile.Length; i++)
                {
                    string[] proxy = proxyfile[i].Split(':');
                    if (!string.IsNullOrEmpty(proxy[0]) && !string.IsNullOrEmpty(proxy[1]))
                    {
                        ProxyInfo proxyinfo = new ProxyInfo();
                        proxyinfo.Host = proxy[0];
                        proxyinfo.Port = proxy[1];
                        if (proxy.Count() == 2)
                        {
                            proxyinfo.Username = null;
                            proxyinfo.Password = null;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(proxy[2]) && !string.IsNullOrEmpty(proxy[3]))
                            {
                                proxyinfo.Username = proxy[2];
                                proxyinfo.Password = proxy[3];
                            }
                        }
                        proxies.Add(proxyinfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return proxies;
        }

        public async Task WriteFileProxyDroidAAsync(string index, string deviceId, string host, string port, string? username, string? password, bool http)
        {
            try
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "ProxyDroid\\" + index);
                string fileXML = Path.Combine(Environment.CurrentDirectory, "ProxyDroid\\org.proxydroid_preferences.xml");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    string destinationFile = Path.Combine(filePath, Path.GetFileName(fileXML));
                    File.Copy(fileXML, destinationFile);
                }
                if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port))
                {
                    XmlHelper.SetElementValue(filePath + "\\org.proxydroid_preferences.xml", "host", host);
                    XmlHelper.SetElementValue(filePath + "\\org.proxydroid_preferences.xml", "port", port);
                    XmlHelper.SetBooleanElementValue(filePath + "\\org.proxydroid_preferences.xml", "isBypassApps", true);
                    XmlHelper.SetBooleanElementValue(filePath + "\\org.proxydroid_preferences.xml", "isRunning", true);
                    XmlHelper.SetBooleanElementValue(filePath + "\\org.proxydroid_preferences.xml", "isConnecting", true);
                }
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    XmlHelper.SetElementValue(filePath + "\\org.proxydroid_preferences.xml", "password", password);
                    XmlHelper.SetElementValue(filePath + "\\org.proxydroid_preferences.xml", "user", username);
                    XmlHelper.SetBooleanElementValue(filePath + "\\org.proxydroid_preferences.xml", "isAuth", true);
                }
                else if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                {
                    XmlHelper.SetBooleanElementValue(filePath + "\\org.proxydroid_preferences.xml", "isAuth", false);
                }
                if (http == true)
                {
                    XmlHelper.SetElementValue(filePath + "\\org.proxydroid_preferences.xml", "proxyType", "http");
                }
                else if (http == false)
                {
                    XmlHelper.SetElementValue(filePath + "\\org.proxydroid_preferences.xml", "proxyType", "socks5");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }

        }

        public class ProxyInfo
        {
            public string? Host { get; set; }
            public string? Port { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public bool IsHttp { get; set; } = true;
            public bool IsRunning { get; set; } = false;
        }
    }
}
