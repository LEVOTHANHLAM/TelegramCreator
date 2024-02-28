using AdvancedSharpAdbClient;
using AppDesptop.TelegramCreator.Models;
using WTelegram;

namespace AppDesptop.TelegramCreator.Interfaces
{
    public interface ITelegram
    {
        Task<bool> CheckLayoutTelegram(string index, DeviceData data, AdbClient adbClient);
        Task<PhoneInfo> ImportPhone(string index, string service, string key, string contry, DeviceData data, AdbClient adbClient,Label label);
        Task<string> ImportPhoneCode(string index, string service, string key, string idPhone, DeviceData data, AdbClient adbClient, Label label);
        Task<bool> ImportFullname(string index, string lastname, string firtname, DeviceData data, AdbClient adbClient);
        Task<bool> ImportPassword(string index, string password, Client client, DeviceData data, AdbClient adbClient);
        Task<string> LoginApiTelegram(string index, string phone, Client client);
        Task<bool> AuthorApiTelegram(string index, string code, Client client);
        Task<string> GetCodeLDPlayerTelegram(string index, DeviceData data, AdbClient adbClien);
        Task<bool> ImportUsername(string index, string username, Client client);
        Task<bool> ImportAvatar(string index, string fileImage, Client client);
        Task<string> GetCodeLoginAPILDPlayerTelegram(DeviceData data, AdbClient adbClien);
    }
}
