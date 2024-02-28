using AppDesptop.TelegramCreator.Models;
using static AppDesptop.TelegramCreator.ProxyDroid.ProxyDroidHelper;

namespace AppDesptop.TelegramCreator.ProxyDroid.Interface
{
    public interface IProxyDroidHelper
    {
        Task WriteFileProxyDroidAAsync(string index,string deviceId, string host, string port, string? username, string? password,bool http);
        Task<bool> CheckPorxy(string host, string port, string? username, string? password, bool http);
        Task<List<ProxyInfo>> ReadFileProxy(string filePath);
    }
}
