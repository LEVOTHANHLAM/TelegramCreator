using AdvancedSharpAdbClient;
using AppDesptop.TelegramCreator.GetSmsCode;
using AppDesptop.TelegramCreator.Helper;
using AppDesptop.TelegramCreator.Interfaces;
using AppDesptop.TelegramCreator.Models;
using AppDestop.TelegramCreator.ChoThueSimCodeApi;
using AppDestop.TelegramCreator.FiveSimApi;
using AppDestop.TelegramCreator.Sms365Api;
using AppDestop.TelegramCreator.ViotpApi;
using LDPlayerAndADBController;
using LDPlayerAndADBController.ADBClient;
using LDPlayerAndADBController.AdbController;
using Serilog;
using System.Diagnostics;
using System.Xml.Linq;
using TL;
using WTelegram;

namespace AppDestop.TelegramCreator.ScriptsTelegram
{
    public class Telegram : ITelegram
    {
        private string packageTelegram = "org.telegram.messenger";
        public async Task<bool> CheckLayoutTelegram(string index, DeviceData data, AdbClient adbClient)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    adbClient.StartApp(data, packageTelegram);
                    await LDController.DelayAsync();
                    if (adbClient.IsAppRunning(data, packageTelegram))
                    {
                        return true;
                    }
                    i++;
                    adbClient.StopApp(data, packageTelegram);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return false;
        }

        public async Task<PhoneInfo> ImportPhone(string index, string service, string key, string? contry, DeviceData data, AdbClient adbClient, Label label)
        {
            PhoneInfo phoneInfo = new PhoneInfo();
            if (ADBClientController.ClickElement(data, adbClient, "text='Start Messaging'", 30))
            {
                ADBClientController.ClickElement(data, adbClient, "text='Continue'", 20);
                ADBClientController.ClickElement(data, adbClient, "text='ALLOW'", 20);
                await LDController.DelayAsync(2, 4);
                bool running = true;
                while (running)
                {
                    LabaleHelper.WriteLabale(label, $"Service: {service}");

                    ADBClientController.ClearTextElement(data, adbClient, "content-desc='Phone number'", 15, 30);
                    ADBClientController.ClearTextElement(data, adbClient, "content-desc='Country code'", 15, 30);
                    switch (service)
                    {
                        case "5sim.net":
                            {
                                var httpResult = await FiveSimHttpHelper.BuyPhoneNumber(contry, key);
                                if (httpResult.StatusCode == 200)
                                {
                                    var getBuyPhone = (FiveResult)httpResult.Data;
                                    phoneInfo.PhoneNumber = getBuyPhone.phone;
                                    phoneInfo.IdPhoneNumber = getBuyPhone.id.ToString();
                                    running = false;
                                    break;
                                }
                                else
                                {
                                    var message = httpResult.Data;
                                    if (message != null)
                                    {
                                        RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {message}", 1);
                                        LabaleHelper.WriteLabale(label, $"message: {message}", 1);
                                    }
                                    else
                                    {

                                        RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    No Phone", 1);
                                        LabaleHelper.WriteLabale(label, "No Phone", 1);
                                    }
                                    continue;
                                }

                            }
                        case "chothuesimcode.com":
                            {
                                var getBuyPhone = await ChothueSimCodeHttpHelper.BuyPhoneNumber(key);
                                if (getBuyPhone.ResponseCode == 0)
                                {

                                    phoneInfo.PhoneNumber = "+84" + getBuyPhone.Result.Number;
                                    phoneInfo.IdPhoneNumber = getBuyPhone.Result.Id;
                                    running = false;
                                    break;
                                }
                                else
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {getBuyPhone.Msg}", 1);
                                    LabaleHelper.WriteLabale(label, getBuyPhone.Msg, 1);
                                    continue;
                                }

                            }
                        case "viotp.com":
                            {
                                var getBuyPhone = await ViotpHttpHelper.BuyPhoneNumber(key);
                                if (getBuyPhone.success == true)
                                {

                                    phoneInfo.PhoneNumber = "84" + getBuyPhone.data.phone_number;
                                    phoneInfo.IdPhoneNumber = getBuyPhone.data.request_id.ToString();
                                    running = false;
                                    break;
                                }
                                else
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {getBuyPhone.message}", 1);
                                    LabaleHelper.WriteLabale(label, getBuyPhone.message, 1);
                                    continue;
                                }

                            }
                        case "365sms.org":
                            {
                                var getBuyPhone = await Sms365HttpHelper.BuyPhoneNumber(key, contry);
                                string[] result = getBuyPhone.Split(':');
                                if (result[0] == "ACCESS_NUMBER")
                                {
                                    phoneInfo.PhoneNumber = result[2];
                                    phoneInfo.IdPhoneNumber = result[1];
                                    running = false;
                                    break;
                                }
                                else
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {getBuyPhone}", 1);
                                    LabaleHelper.WriteLabale(label, getBuyPhone, 1);
                                    continue;
                                }

                            }
                        case "getsmscode.io":
                            {
                                var getBuyPhone = await GetSmsCodeHttpHelper.BuyPhoneNumber(key, contry);
                                if (getBuyPhone == null)
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}  error_code:  {getBuyPhone.errors.error_code}       message:  Error", 1);
                                    LabaleHelper.WriteLabale(label, "Error", 1);
                                    continue;
                                }
                                else if (getBuyPhone.Status)
                                {
                                    phoneInfo.PhoneNumber = getBuyPhone.data.Phone_number;
                                    phoneInfo.IdPhoneNumber = getBuyPhone.data.ActivationId;
                                    running = false;
                                    break;
                                }
                                else
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}  error_code:  {getBuyPhone.errors.error_code}       message:  {getBuyPhone.errors.error_mess}", 1);
                                    LabaleHelper.WriteLabale(label, getBuyPhone.errors.error_code, 1);
                                    continue;
                                }

                            }
                        default:
                            {
                                continue;
                            }
                    }
                }
                await LDController.DelayAsync();
                ADBHelper.InputText(data.Serial, phoneInfo.PhoneNumber);
                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}   Import Phone Number:  {phoneInfo.PhoneNumber}", 0);
                LabaleHelper.WriteLabale(label, "Import Phone Number", 0);
                if (ADBClientController.ClickElement(data, adbClient, "content-desc='Done'", 30))
                {
                    ADBClientController.ClickElement(data, adbClient, "text='Continue'", 15);
                    ADBClientController.ClickElement(data, adbClient, "text='ALLOW'", 15);
                    List<string> listElement = new List<string> { "banned", "Enter code", "Check your Telegram messages" };
                    string findElement = string.Empty;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    bool result = true;
                    while (result)
                    {
                        foreach (string element in listElement)
                        {
                            try
                            {
                                if (ADBClientController.FindElementbyDump(ADBClientController.GetDump(data, adbClient), data, adbClient, element))
                                {
                                    findElement = element;
                                    result = false;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, ex.Message);
                            }
                        }
                        if (sw.ElapsedMilliseconds > 60000)
                        {
                            sw.Stop(); break;
                        }
                    }
                    switch (findElement)
                    {
                        case "banned":
                            {
                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}   This phone number: {phoneInfo.PhoneNumber} is banned.", 1);
                                LabaleHelper.WriteLabale(label, $" This phone number: {phoneInfo.PhoneNumber} is banned.", 1);
                                return null;
                            }
                        case "Check your Telegram messages":
                            {
                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}   Is Using {phoneInfo.PhoneNumber} ", 1);
                                LabaleHelper.WriteLabale(label, $"Is Using {phoneInfo.PhoneNumber}", 1);
                                return null;
                            }
                        case "Enter code":
                            {
                                return phoneInfo;
                            }
                        default:
                            {
                                return phoneInfo;
                            }
                    }
                }
            }
            return null;
        }

        public async Task<string> ImportPhoneCode(string index, string service, string key, string idPhone, DeviceData data, AdbClient adbClient, Label label)
        {
            string code;
            int i = 0;
            while (i < 15)
            {
                i++;
                Log.Information($"Start {i} ", $"Start {i} ");
                LabaleHelper.WriteLabale(label, $" Get Code {service}");
                await LDController.DelayAsync(10, 20);
                switch (service)
                {
                    case "5sim.net":
                        {
                            await LDController.DelayAsync(10, 20);
                            var httpResult = await FiveSimHttpHelper.GetOtp(key, idPhone);
                            if (httpResult.StatusCode == 200)
                            {
                                var getOtp = (FiveResult)httpResult.Data;
                                code = getOtp.sms[0].code;
                                return code;
                            }
                            else
                            {
                                var message = httpResult.Data;
                                if (message != null)
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {message}", 1);
                                    LabaleHelper.WriteLabale(label, message.ToString(), 1);
                                }
                                else
                                {
                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    No Code", 1);
                                    LabaleHelper.WriteLabale(label, "No Code", 1);

                                }
                                await LDController.DelayAsync(5, 10);
                                continue;
                            }
                        }
                    case "chothuesimcode.com":
                        {
                            var getOtp = await ChothueSimCodeHttpHelper.GetOtp(key, idPhone);
                            if (getOtp.ResponseCode == 0)
                            {
                                code = getOtp.Result.Code;
                                return code;
                            }
                            else
                            {
                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {getOtp.Msg}", 1);
                                LabaleHelper.WriteLabale(label, getOtp.Msg, 1);
                                await LDController.DelayAsync(5, 10);
                                continue;
                            }
                        }
                    case "viotp.com":
                        {
                            var getOtp = await ViotpHttpHelper.GetOtp(key, idPhone);
                            if (getOtp.success == true)
                            {
                                if (getOtp.data?.Status == 1)
                                {
                                    code = getOtp.data.Code;
                                    return code;
                                }
                                await LDController.DelayAsync(5, 10);
                                continue;
                            }
                            else
                            {
                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {getOtp.message}", 1);
                                LabaleHelper.WriteLabale(label, getOtp.message, 1);
                                await LDController.DelayAsync(5, 10);
                                continue;
                            }
                        }
                    case "365sms.org":
                        {
                            await LDController.DelayAsync(10, 20);
                            var getOtp = await Sms365HttpHelper.GetOtp(key, idPhone);
                            string[] result = getOtp.Split(':');
                            if (result[0] == "STATUS_OK")
                            {

                                code = result[1];
                                return code;
                            }
                            else
                            {
                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}    {getOtp}", 1);
                                LabaleHelper.WriteLabale(label, getOtp, 1);
                                await LDController.DelayAsync(5, 10);
                                continue;
                            }
                        }
                    case "getsmscode.io":
                        {
                            await LDController.DelayAsync(20, 40);
                            var getOtp = await GetSmsCodeHttpHelper.GetOtp(key, idPhone);
                            Log.Information($"End{i}        {getOtp}");
                            if (getOtp != null && getOtp.Status)
                            {
                                code = getOtp.sms_code;
                                return code;
                            }
                            else
                            {
                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {index}  {getOtp.error}", 1);
                                LabaleHelper.WriteLabale(label, getOtp.error, 1);
                                await LDController.DelayAsync(5, 10);
                                continue;
                            }
                        }
                    default:
                        {
                            continue;
                        }
                }
            }
            return null;
        }

        public async Task<bool> ImportFullname(string index, string lastname, string firtname, DeviceData data, AdbClient adbClient)
        {
            var profileInfo = ADBClientController.ElementIsExist(data, adbClient, "text='Profile info'", 20);
            if (profileInfo == true)
            {
                Thread.Sleep(200);
                ////input firstname
                ADBHelper.InputTextWithADBKeyboard(data.Serial, firtname);
                Thread.Sleep(200);
                //input Lastname
                ADBHelper.TapByPercent(data.Serial, 66.3, 38.4);
                Thread.Sleep(200);
                ADBHelper.InputTextWithADBKeyboard(data.Serial, lastname);
                Thread.Sleep(200);
                ADBClientController.ClickElement(data, adbClient, "content-desc='Done'", 20);
                ADBClientController.ClickElement(data, adbClient, "text='Continue'", 20);
                ADBClientController.ClickElement(data, adbClient, "text='ALLOW'", 20);
                await LDController.DelayAsync();
                ADBClientController.ClickElement(data, adbClient, "text='ALLOW'", 20);
                return true;
            }
            return false;
        }

        public async Task<bool> ImportPassword(string index, string password, Client client, DeviceData data, AdbClient adbClient)
        {
            try
            {
                string old_password = "";
                string new_password = password;
                var accountPwd = await client.Account_GetPassword();
                var passwordNew = accountPwd.current_algo == null ? null : await WTelegram.Client.InputCheckPassword(accountPwd, old_password);
                accountPwd.current_algo = null; // makes InputCheckPassword generate a new password
                var new_password_hash = new_password == null ? null : await WTelegram.Client.InputCheckPassword(accountPwd, new_password);
                return await client.Account_UpdatePasswordSettings(passwordNew, new Account_PasswordInputSettings
                {
                    flags = Account_PasswordInputSettings.Flags.has_new_algo,
                    new_password_hash = new_password_hash?.A,
                    new_algo = accountPwd.new_algo,
                    hint = "new password hint",
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public async Task<string> LoginApiTelegram(string index, string phone, Client client)
        {
            try
            {
                var checkLgoinPhone = await client.Login(phone);
                return checkLgoinPhone;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }

        public async Task<bool> AuthorApiTelegram(string index, string code, Client client)
        {
            try
            {
                var checkLoginCode = await client.Login(code);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }

        }

        public async Task<string> GetCodeLDPlayerTelegram(string index, DeviceData data, AdbClient adbClien)
        {

            string Logincode = "Login code:";
            bool running = true;
            string lastMatch = null;
            for (int i = 0; i < 20; i++)
            {
                var telegraMess = ADBClientController.FindElements(data, adbClien, "class='android.view.ViewGroup'", 30);
                if (telegraMess.Count > 0)
                {
                    foreach (var item in telegraMess)
                    {
                        item.Click();
                        await LDController.DelayAsync(10);
                        var dump = adbClien.DumpScreen(data);
                        if (dump != null)
                        {
                            string xmlString = dump.OuterXml;
                            if (xmlString.Contains(Logincode) == true)
                            {
                                string[] substrings = xmlString.Split(new[] { Logincode }, StringSplitOptions.None);
                                foreach (string substring in substrings)
                                {
                                    string trimmed = substring.Trim();

                                    if (!string.IsNullOrEmpty(trimmed))
                                    {
                                        lastMatch = trimmed;
                                    }
                                }
                                if (lastMatch != null)
                                {
                                    string[] resultCode = lastMatch.Split('.');
                                    string codeLogin = resultCode[0];
                                    int outputInt;
                                    bool isInt = Int32.TryParse(codeLogin, out outputInt);
                                    if (isInt && codeLogin.Length == 5)
                                    {
                                        return codeLogin;
                                    }
                                }
                            }
                            else
                            {
                                ADBClientController.FindElementIsExistOrClickByClass(data, adbClien, "Go back", "android.widget.ImageView", 30);
                                await LDController.DelayAsync();
                            }
                        }
                        else
                        {
                            ADBClientController.FindElementIsExistOrClickByClass(data, adbClien, "Go back", "android.widget.ImageView", 30);
                            await LDController.DelayAsync();
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> ImportUsername(string index, string username, Client client)
        {
            try
            {
                await client.Account_UpdateUsername(username);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public async Task<bool> ImportAvatar(string index, string fileImage, Client client)
        {
            try
            {
                var file = await client.UploadFileAsync(fileImage);
                await client.Photos_UploadProfilePhoto(file);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }

        }
        public async Task<string> GetCodeLoginAPILDPlayerTelegram(DeviceData data, AdbClient adbClien)
        {

            string Logincode = "This is your login code:";
            bool running = true;
            string lastMatch = null;
            for (int i = 0; i < 10; i++)
            {
                var telegraMess = ADBClientController.FindElements(data, adbClien, "class='android.view.ViewGroup'", 30);
                if (telegraMess.Count > 0)
                {
                    foreach (var item in telegraMess)
                    {
                        try
                        {
                            item.Click();
                            await LDController.DelayAsync(10);
                            var dump = adbClien.DumpScreen(data);
                            if (dump != null)
                            {
                                string xmlString = dump.OuterXml;
                                XDocument doc = XDocument.Parse(xmlString);

                                // Tìm node chứa chuỗi cần trích xuất
                                XElement node = doc.Descendants("node")
                                                   .Last(n => n.Attribute("text")?.Value.Contains("my.telegram.org. This is your login code:") ?? false);
                                if (node != null)
                                {
                                    // Get the text content of the node
                                    string targetText = node.Attribute("text")?.Value;

                                    // Extract the login code after the prefix
                                    int startIndex = targetText.IndexOf(Logincode);
                                    if (startIndex != -1)
                                    {
                                        // Extract the login code part after the prefix
                                        string codeWithPrefix = targetText.Substring(startIndex + Logincode.Length).Trim();

                                        // Find the end of the code (where it might end with a space, newline, or other characters)
                                        int endIndex = codeWithPrefix.IndexOfAny(new[] { ' ', '\n' });

                                        // Extract the code based on the endIndex position
                                        string code = endIndex != -1 ? codeWithPrefix.Substring(0, endIndex) : codeWithPrefix;

                                        if (!string.IsNullOrEmpty(code))
                                        {
                                            ADBClientController.FindElementIsExistOrClickByClass(data, adbClien, "Go back", "android.widget.ImageView", 30);
                                            return code;
                                        }
                                    }
                                }
                                else
                                {
                                    ADBClientController.FindElementIsExistOrClickByClass(data, adbClien, "Go back", "android.widget.ImageView", 30);
                                    await LDController.DelayAsync();
                                }
                            }
                            else
                            {
                                ADBClientController.FindElementIsExistOrClickByClass(data, adbClien, "Go back", "android.widget.ImageView", 30);
                                await LDController.DelayAsync();
                            }
                        }
                        catch (Exception)
                        {

                        }
                       
                    }
                }
            }
            return null;
        }
    }
}
