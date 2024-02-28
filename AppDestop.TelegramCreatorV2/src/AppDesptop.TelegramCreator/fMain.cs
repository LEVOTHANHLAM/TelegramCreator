using AdvancedSharpAdbClient;
using AppDesptop.TelegramCreator.Database.Entities;
using AppDesptop.TelegramCreator.Database.Repositories;
using AppDesptop.TelegramCreator.Froms;
using AppDesptop.TelegramCreator.Helper;
using AppDesptop.TelegramCreator.Interfaces;
using AppDesptop.TelegramCreator.Models;
using AppDesptop.TelegramCreator.MuiltiTask;
using AppDesptop.TelegramCreator.ProxyDroid;
using AppDesptop.TelegramCreator.ProxyDroid.Interface;
using AppDesptop.TelegramCreator.ScriptsTelegram;
using AppDestop.TelegramCreator.ChoThueSimCodeApi;
using ICSharpCode.SharpZipLib.Zip;
using LDPlayerAndADBController;
using LDPlayerAndADBController.ADBClient;
using LDPlayerAndADBController.AdbController;
using Microsoft.VisualBasic.ApplicationServices;
using Org.BouncyCastle.Utilities.Zlib;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.Services.Description;
using System.Windows.Shapes;
using TL;
using WTelegram;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ComboBoxItem = AppDesptop.TelegramCreator.Helper.ComboBoxItem;
using Path = System.IO.Path;


namespace AppDesptop.TelegramCreator
{
    public partial class fMain : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private readonly IAccountRepository _accountRepository;
        private readonly ITelegram _telegram;
        private List<CountryCode> listCountryCode = new List<CountryCode>();
        private List<Helper.ComboBoxItem> listItem = new List<ComboBoxItem>();

        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        private readonly BaseViewModel _baseViewModel;
        private int columnCount = 0;
        private int rowCount = 0;
        private int originalFormWidth;
        private List<string> _listProxy = new List<string>();
        private List<string> _listFirtname = new List<string>();
        private List<string> _listLastname = new List<string>();
        private List<string> _listUsername = new List<string>();
        private List<string> _listAvatar = new List<string>();
        private object _lockObject;
        private List<ProfileModel> profileModels;
        private JsonHelper jsonHelper;
        public fMain(IAccountRepository accountRepository, ITelegram telegram)
        {
            InitializeComponent();
            string path = Path.Combine(Environment.CurrentDirectory, "settings\\configGeneral.json");
            jsonHelper = new JsonHelper(path, isJsonString: false);
            loadFileJsonService();
            _accountRepository = accountRepository;
            InitializeSavedValues();
            originalFormWidth = this.Width;
            _baseViewModel = new BaseViewModel();
            btnDisplay.Text = "5";
            readFile();
            profileModels = new List<ProfileModel>();
            _lockObject = new object();
            _telegram = telegram;
            RichTextBoxHelper._RichTextBox = rtbLogs;
        }
        private void readFile()
        {
            if (File.Exists(GlobalModels.PathProxy))
            {
                var lines = File.ReadAllLines(GlobalModels.PathProxy);
                _listProxy.Clear();
                foreach (var line in lines)
                {
                    _listProxy.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathFirstName))
            {
                var lines = File.ReadAllLines(GlobalModels.PathFirstName);
                _listFirtname.Clear();
                foreach (var line in lines)
                {
                    _listFirtname.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathLastName))
            {
                var lines = File.ReadAllLines(GlobalModels.PathLastName);
                _listLastname.Clear();
                foreach (var line in lines)
                {
                    _listLastname.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathUserName))
            {
                var lines = File.ReadAllLines(GlobalModels.PathUserName);
                _listUsername.Clear();
                foreach (var line in lines)
                {
                    _listUsername.Add(line);
                }
            }
        }
        private void InitializeSavedValues()
        {
            numberAccount.Value = jsonHelper.GetIntType("numberAccount");
            numberThread.Value = jsonHelper.GetIntType("numberThread");
            rdoNoProxy.Checked = jsonHelper.GetBooleanValue("rdoNoProxy");
            rdoProxy.Checked = jsonHelper.GetBooleanValue("rdoProxy");
            radioCustomizePass.Checked = jsonHelper.GetBooleanValue("radioCustomizePass");
            txtPass.Text = jsonHelper.GetValuesFromInputString("txtPass");
            radioRandomPass.Checked = jsonHelper.GetBooleanValue("radioRandomPass");
            NumberPass.Value = jsonHelper.GetIntType("NumberPass");
            radioRadomFullNameUs.Checked = jsonHelper.GetBooleanValue("radioRadomFullNameUs");
            radioCustomizeFullName.Checked = jsonHelper.GetBooleanValue("radioCustomizeFullName");
            radioRadomFullNameVN.Checked = jsonHelper.GetBooleanValue("radioRadomFullNameVN");
            radioRandomUserName.Checked = jsonHelper.GetBooleanValue("radioRandomUserName");
            radioCustomizeUserName.Checked = jsonHelper.GetBooleanValue("radioCustomizeUserName");
            cbUpdateAvatar.Checked = jsonHelper.GetBooleanValue("cbUpdateAvatar");
            txtFolderAvatar.Text = jsonHelper.GetValuesFromInputString("txtFolderAvatar");
            CBoxOtpService.Text = jsonHelper.GetValuesFromInputString("CBoxOtpService");
            txtApikey.Text = jsonHelper.GetValuesFromInputString("txtApikey");
            CBServiceCode.Text = jsonHelper.GetValuesFromInputString("CBServiceCode");
        }
        private void saveProperties()
        {
            SettingsTool.GetSettings("configGeneral").AddValue("radioCustomizeUserName", radioCustomizeUserName.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioRandomUserName", radioRandomUserName.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("numberAccount", numberAccount.Value);
            SettingsTool.GetSettings("configGeneral").AddValue("numberThread", numberThread.Value);
            SettingsTool.GetSettings("configGeneral").AddValue("rdoNoProxy", rdoNoProxy.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("rdoProxy", rdoProxy.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioCustomizePass", radioCustomizePass.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("txtPass", txtPass.Text.Trim());
            SettingsTool.GetSettings("configGeneral").AddValue("radioRandomPass", radioRandomPass.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("NumberPass", NumberPass.Value);
            SettingsTool.GetSettings("configGeneral").AddValue("radioRadomFullNameUs", radioRadomFullNameUs.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioCustomizeFullName", radioCustomizeFullName.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioRadomFullNameVN", radioRadomFullNameVN.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("cbUpdateAvatar", cbUpdateAvatar.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("txtFolderAvatar", txtFolderAvatar.Text.Trim());
            SettingsTool.GetSettings("configGeneral").AddValue("CBoxOtpService", CBoxOtpService.Text.Trim());
            SettingsTool.GetSettings("configGeneral").AddValue("txtApikey", txtApikey.Text.Trim());
            SettingsTool.GetSettings("configGeneral").AddValue("CBServiceCode", CBServiceCode.Text.Trim());
            SettingsTool.UpdateSetting("configGeneral");
        }
        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                LDController.EditFileConfigLDPlayer();
                btnStart.Enabled = false;
                if (!AdbServer.Instance.GetStatus().IsRunning)
                {
                    AdbServer server = new AdbServer();
                    StartServerResult result = server.StartServer($"{LDController.PathFolderLDPlayer}\\adb.exe", false);
                    if (result != StartServerResult.Started)
                    {
                        MessageCommon.ShowMessageBox("Can't start adb server", 2);
                        return;
                    }
                }
                tableLayoutPanel.Dock = DockStyle.Fill;
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tableLayoutPanel.AutoScroll = true;
                var ldplayers = LDController.GetDevices2();
                if (LDController.GetDevices2().Count == 0 || ldplayers.Count < (int)numberThread.Value)
                {
                    MessageCommon.ShowMessageBox("Please check LDPlayer!", 2);
                    btnStart.Enabled = true;
                    return;
                }
                saveProperties();
                GlobalModels.Service = CBoxOtpService.Text.Replace("(recommend)", "").Trim();
                Random random = new Random();
                if (rdoNoProxy.Checked)
                {
                    _listProxy.Clear();
                }
                if (radioRandomUserName.Checked)
                {
                    var listUser = ProfileHelper.ReadFile(GlobalModels.PathDataUserName);
                    if (listUser.Count > 0)
                    {
                        _listUsername.Clear();
                        foreach (var item in listUser)
                        {
                            var username = item + LDPlayerAndADBController.Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", random.Next(5, 15));
                            _listUsername.Add(username);
                        }
                    }
                }
                if (radioRadomFullNameVN.Checked)
                {
                    var firtsnames = ProfileHelper.ReadFile(GlobalModels.PathDataFirstNameVN);
                    var lastnames = ProfileHelper.ReadFile(GlobalModels.PathDataLastNameVN);
                    if (firtsnames.Count > 0 && lastnames.Count > 0)
                    {
                        _listFirtname.Clear();
                        _listLastname.Clear();
                        foreach (var item in firtsnames)
                        {
                            _listFirtname.Add(item);
                        }
                        foreach (var item in lastnames)
                        {
                            _listLastname.Add(item);
                        }
                    }

                }
                else if (radioRadomFullNameUs.Checked == true)
                {
                    var firtsnames = ProfileHelper.ReadFile(GlobalModels.PathDataFirstNameUS);
                    var lastnames = ProfileHelper.ReadFile(GlobalModels.PathDataLastNameUs);
                    if (firtsnames.Count > 0 && lastnames.Count > 0)
                    {
                        _listFirtname.Clear();
                        _listLastname.Clear();
                        foreach (var item in firtsnames)
                        {
                            _listFirtname.Add(item);
                        }
                        foreach (var item in lastnames)
                        {
                            _listLastname.Add(item);
                        }
                    }
                }
                //-----------------------------------------------//
                List<string> list = new List<string>();
                Queue<string> queueProxy = new Queue<string>(); ;
                if (rdoProxy.Checked)
                {
                    list.AddRange(_listProxy);
                    while (numberAccount.Value > list.Count)
                    {
                        list.AddRange(_listProxy);
                    }
                    queueProxy = new Queue<string>(list);
                }
                Queue<string> queueUsername = new Queue<string>();
                if (radioCustomizeUserName.Checked)
                {
                    queueUsername = new Queue<string>(_listUsername);
                }
                Queue<string> queueAvatar = new Queue<string>();
                if (cbUpdateAvatar.Checked && Directory.Exists(txtFolderAvatar.Text.Trim()))
                {
                    _listAvatar.Clear();
                    var files = FileHelper.ReadImageFiles(txtFolderAvatar.Text.Trim());
                    if (files.Count == 0)
                    {
                        return;
                    }
                    _listAvatar.AddRange(files);
                    while (numberAccount.Value > _listAvatar.Count)
                    {
                        _listAvatar.AddRange(files);
                    }
                    queueAvatar = new Queue<string>(_listAvatar);
                }
                profileModels.Clear();
                for (int i = 0; i < numberAccount.Value; i++)
                {
                    ProfileModel profile = new ProfileModel();
                    profile.FileSession = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Session");
                    profile.Firstname = _listFirtname[random.Next(0, _listFirtname.Count)];
                    profile.Lastname = _listLastname[random.Next(0, _listLastname.Count)];
                    if (radioCustomizeUserName.Checked)
                    {
                        profile.Username = queueUsername.Dequeue().Replace("'", "''");
                    }
                    else
                    {
                        profile.Username = _listUsername[random.Next(0, _listUsername.Count)];
                    }
                    if (radioCustomizePass.Checked)
                    {
                        profile.Password = txtPass.Text.Trim();
                    }
                    else
                    {
                        profile.Password = LDPlayerAndADBController.Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", (int)NumberPass.Value);
                    }
                    if (_listProxy.Count > 0 && rdoProxy.Checked)
                    {
                        string text = queueProxy.Dequeue().Replace("'", "''");
                        profile.Proxy = text;
                    }
                    if (_listAvatar.Count > 0 && cbUpdateAvatar.Checked)
                    {
                        string text = queueAvatar.Dequeue().Replace("'", "''");
                        profile.FileImage = text;
                    }
                    profile.IsUsing = false;
                    profileModels.Add(profile);
                }
                GlobalModels.Contry = jsonHelper.GetValuesFromInputString("Contry");

                GlobalModels.Devices.Clear();
                foreach (var item in ldplayers)
                {
                    if (!LDController.IsDevice_Running("index", item.index.ToString()))
                    {
                        DeviceInfo device = new DeviceInfo();
                        device.IndexLDPlayer = item.index.ToString();
                        device.AdbClient = new AdbClient();
                        device.Data = new DeviceData();
                        device.View = new ViewInfo();
                        device.View.Embeddedpanel = new Panel();
                        device.View.StatusLabel = new Label();
                        device.View.LdplayerHandle = new IntPtr();
                        device.View.Panel = new Panel();
                        device.View.PanelButton = new Panel();
                        device.View.BtnClose = new Button();
                        device.sourceToken = new CancellationTokenSource();
                        device.pauseToken = new PauseTokenSource();
                        device.DataGridView = new DataGridView();
                        device.DataGridView = dtgvAccount;
                        device.RowDataGridView = new DataGridViewRow();
                        GlobalModels.Devices.Add(device);
                    }
                    if (GlobalModels.Devices.Count >= (int)numberThread.Value)
                    {
                        break;
                    }
                }
                List<Task> tasks = new List<Task>();
                cancellationTokenSource = new CancellationTokenSource();
                foreach (var item in GlobalModels.Devices)
                {
                    try
                    {
                        lock (_lockObject)
                        {
                            File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\leidian{item.IndexLDPlayer}\\data.vmdk");
                            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "LDplayer\\data.zip"), $"{LDController.PathFolderLDPlayer}\\vms\\leidian{item.IndexLDPlayer}");
                            File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\config\\leidian{item.IndexLDPlayer}.config");
                            string newPath = Path.Combine($"{LDController.PathFolderLDPlayer}\\vms\\config", $"leidian{item.IndexLDPlayer}.config");
                            File.Copy(Path.Combine(Environment.CurrentDirectory, "LDplayer\\leidian.txt"), newPath);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    await LDController.DelayAsync(5);
                    int i = 6;
                    while (i > 0)
                    {
                        LDController.Open("index", item.IndexLDPlayer);
                        await LDController.DelayAsync(5);
                        if (LDController.IsDevice_Running("index", item.IndexLDPlayer))
                        {
                            if (columnCount >= int.Parse(btnDisplay.Text.Trim()))
                            {
                                rowCount++;
                                columnCount = 0;
                                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }
                            item.View.originalParentHandle = IntPtr.Zero;
                            addViewControl(item, tableLayoutPanel);
                            columnCount++;
                            await LDController.DelayAsync(5, 10);
                            tasks.Add(Task.Run(async () =>
                            {
                                await StartDevice(item);
                            }, cancellationTokenSource.Token));
                            break;
                        }
                        i--;
                    }
                }
                btnStart.Enabled = true;
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
            }
        }
        private void writeLog(DeviceInfo device, string message, string? idAccount, Label? label, int type = 0, bool isWriteRichTextBox = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(idAccount))
                {
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, idAccount, "cIdAccount", "cStatus", message, type);
                }
                if (isWriteRichTextBox)
                {
                    RichTextBoxHelper.WriteLogRichTextBox($"LDPLayer: {device.IndexLDPlayer}  {message}", type);
                }
                if (!string.IsNullOrEmpty(device.IndexLDPlayer))
                {
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tStatus", message, type);
                }
                if (label != null)
                {
                    LabaleHelper.WriteLabale(label, message, type);
                }

            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private async Task ChangeProxy(string index, string deviceId, string ip, string port, string? username, string? password)
        {
            try
            {
                string cmd = $"shell am broadcast -a com.vat.vpn.CONNECT_PROXY -n com.vat.vpn/.ui.ProxyReceiver --es address {ip} --es port {port}";
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    cmd = $"shell am broadcast -a com.vat.vpn.CONNECT_PROXY -n com.vat.vpn/.ui.ProxyReceiver --es address {ip} --es port {port}  --es username {username} --es password {password}";
                }
                var connectProxie = ADBHelper.ADB(deviceId, cmd);
                if (string.IsNullOrEmpty(connectProxie) || !connectProxie.Contains("successful"))
                {
                    int i = 6;
                    while (i > 0)
                    {
                        LDController.RunApp("index", index, "com.vat.vpn");
                        string connect = Path.Combine(Environment.CurrentDirectory, "Database\\ImagesClick\\Proxy\\Connect.PNG");
                        string oke = Path.Combine(Environment.CurrentDirectory, "Database\\ImagesClick\\Proxy\\Ok.PNG");
                        string disconnection = Path.Combine(Environment.CurrentDirectory, "Database\\ImagesClick\\Proxy\\Disconnection.PNG");
                        if (ADBHelper.FindImage(deviceId, connect, 0.9, 30000))
                        {
                            ADBHelper.TapByPercent(deviceId, 35.6, 39.2);
                            Thread.Sleep(200);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(200);
                            ADBHelper.InputTextWithADBKeyboard(deviceId, ip);
                            Thread.Sleep(200);
                            ADBHelper.TapByPercent(deviceId, 79.8, 39.1);
                            Thread.Sleep(200);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(200);
                            ADBHelper.InputTextWithADBKeyboard(deviceId, port);
                            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                            {
                                username = "";
                                password = "";
                            }
                            Thread.Sleep(200);
                            ADBHelper.TapByPercent(deviceId, 49.4, 51.6);
                            Thread.Sleep(200);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(200);
                            ADBHelper.InputTextWithADBKeyboard(deviceId, username);
                            Thread.Sleep(200);
                            ADBHelper.TapByPercent(deviceId, 49.7, 61.1);
                            Thread.Sleep(200);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(200);
                            ADBHelper.InputTextWithADBKeyboard(deviceId, password);
                            Thread.Sleep(200);
                            if (ADBHelper.FindImageTap(deviceId, connect, 0.9, 30000))
                            {
                                if (ADBHelper.FindImageTap(deviceId, oke, 0.9, 30000))
                                {
                                    if (ADBHelper.FindImage(deviceId, disconnection, 0.25, 30000))
                                    {
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        LDController.KillApp("index", index, "com.vat.vpn");
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR: {nameof(fMain)}, params; {nameof(StartDevice)},Device; {index}, Proxy; {ip}:{port}:{username}:{password}, Error; {ex.Message}, Exception; {ex}");
                return;
            }
        }
        private bool CheckProxy(string host, string port, string? username, string? password)
        {
            try
            {
                string proxyAddress = host + ":" + port;

                // Create a WebRequest using proxy
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api64.ipify.org/");
                request.Proxy = new WebProxy(proxyAddress);

                // Set proxy credentials if provided
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    request.Proxy.Credentials = new NetworkCredential(username, password);
                }

                // Send a GET request to check the proxy
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check the HTTP status code to determine proxy validity
                    HttpStatusCode statusCode = response.StatusCode;

                    // Return true if status code indicates success
                    return statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.Ambiguous;
                }
            }
            catch (WebException)
            {
                // An error occurred when using the proxy
                return false;
            }
        }
        private async Task StartDevice(DeviceInfo device)
        {
            try
            {
                while (true)
                {
                    ProfileModel selectAccount;
                    writeLog(device, $"Connect", null, device.View.StatusLabel, 0);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (true)
                    {
                        await LDController.DelayAsync();
                        if (DeviceHelper.Connect(device))
                        {
                            device.Status = "Connect";
                            ADBHelper.TurnOnADBKeyboard(device.Id);
                            sw.Stop();
                            break;
                        }
                        if (sw.ElapsedMilliseconds > 60000)
                        {
                            device.Status = "reboot";
                            sw.Stop(); break;
                        }
                    }
                    if (device.Status == "Connect")
                    {
                        DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tDeviceId", device.Id, 2);
                        DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tName", device.Data.Model, 2);
                        writeLog(device, "Connect Success", null, device.View.StatusLabel, 2);
                        lock (_lockObject)
                        {
                            selectAccount = profileModels.FirstOrDefault(x => x.IsUsing == false);
                        }
                        if (selectAccount == null)
                        {
                            LDController.Close("index", device.IndexLDPlayer);
                            tableLayoutPanel.Invoke((MethodInvoker)delegate
                            {
                                tableLayoutPanel.Controls.Remove(device.View.Panel);
                            });
                            RepositionLDPlayers();
                            return;
                        }
                        selectAccount.IsUsing = true;
                        if (!rdoNoProxy.Checked && !string.IsNullOrEmpty(selectAccount.Proxy))
                        {
                            writeLog(device, "Connect Proxy", null, device.View.StatusLabel);
                            string[] proxy = selectAccount.Proxy.Split(':');
                            string ip = string.Empty, port = string.Empty, username = string.Empty, password = string.Empty;
                            if (proxy.Length > 0)
                            {
                                ip = proxy[0];
                                port = proxy[1];
                                if (proxy.Length > 3)
                                {
                                    username = proxy[2];
                                    password = proxy[3];
                                }
                                if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                                {
                                    if (CheckProxy(ip, port, username, password))
                                    {
                                        writeLog(device, $"{selectAccount.Proxy}", null, device.View.StatusLabel);
                                        await ChangeProxy(device.IndexLDPlayer, device.Id, ip, port, username, password);
                                        await LDController.DelayAsync(10);
                                    }
                                    else
                                    {
                                        writeLog(device, $"Proxy Fail: {selectAccount.Proxy}", null, device.View.StatusLabel, 1);
                                        Random random = new Random();
                                        selectAccount.Proxy = _listProxy[random.Next(_listProxy.Count)];
                                        device.Status = "reboot";
                                    }
                                }
                            }
                        }
                        if (device.Status != "reboot")
                        {
                            ADBHelper.ClearCaches(device.Id, "org.telegram.messenger");
                            await LDController.DelayAsync(2);
                            writeLog(device, $"Run App Telegram", null, device.View.StatusLabel);
                            if (await _telegram.CheckLayoutTelegram(device.IndexLDPlayer, device.Data, device.AdbClient))
                            {
                                var idPhone = await _telegram.ImportPhone(device.IndexLDPlayer, GlobalModels.Service, txtApikey.Text.Trim(), GlobalModels.Contry, device.Data, device.AdbClient, device.View.StatusLabel);
                                if (idPhone != null && long.TryParse(idPhone.IdPhoneNumber, out long result))
                                {
                                    var code = await _telegram.ImportPhoneCode(device.IndexLDPlayer, GlobalModels.Service, txtApikey.Text.Trim(), idPhone.IdPhoneNumber, device.Data, device.AdbClient, device.View.StatusLabel);
                                    if (!string.IsNullOrEmpty(code) && int.TryParse(code, out int resultCode))
                                    {
                                        writeLog(device, $"Import Code: {code}", null, device.View.StatusLabel);
                                        LDController.InputText("index", device.IndexLDPlayer, code);
                                        await LDController.DelayAsync(5, 10);
                                        if (await _telegram.ImportFullname(device.IndexLDPlayer, selectAccount.Lastname, selectAccount.Firstname, device.Data, device.AdbClient))
                                        {
                                            writeLog(device, $"Fullname: {selectAccount.Firstname} {selectAccount.Lastname}", null, device.View.StatusLabel);
                                        }
                                        else
                                        {
                                            selectAccount.Firstname = "";
                                            selectAccount.Lastname = "";
                                        }
                                        await LDController.DelayAsync();
                                        if (ADBClientController.ElementIsExist(device.Data, device.AdbClient, "content-desc='Open navigation menu'", 10))
                                        {
                                            Account account = new Account();
                                            lock (_lockObject)
                                            {
                                                account.Id = Guid.NewGuid();
                                                account.FullName = $"{selectAccount.Firstname} {selectAccount.Lastname}";
                                                account.CreateDate = DateTime.Now;
                                                account.PhoneNumber = idPhone.PhoneNumber;
                                                if (rdoProxy.Checked && selectAccount.Proxy != null)
                                                {
                                                    account.Proxy = selectAccount.Proxy;
                                                }
                                                _accountRepository.Add(account);
                                                loadAccount();
                                            }
                                            string hash = string.Empty; string user = FileHelper.GetUserAgents();
                                            string apId = "28066177"; string apHash = "5ecf08b7b2a7aff9f355abac0c73d450";
                                            string cookie = string.Empty;
                                            TelegramAPI telegramAPI = new TelegramAPI();
                                            int i = 5;
                                            while (i > 0)
                                            {
                                                i--;
                                                hash = await telegramAPI.LoginPhone(account.PhoneNumber, user, account.Proxy);
                                                if (!string.IsNullOrEmpty(hash))
                                                {
                                                    var codeApi = await _telegram.GetCodeLoginAPILDPlayerTelegram(device.Data, device.AdbClient);
                                                    if (!string.IsNullOrEmpty(codeApi))
                                                    {
                                                        cookie = await telegramAPI.GetCookie(account.PhoneNumber, user, hash, codeApi, account.Proxy);
                                                        if (!string.IsNullOrEmpty(cookie))
                                                        {
                                                            i = 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(cookie))
                                            {
                                                var resultApIdandApHash = await telegramAPI.GetApiIdAndHash(cookie, hash, account.Proxy, user);
                                                if (!string.IsNullOrEmpty(resultApIdandApHash))
                                                {
                                                    var parts = resultApIdandApHash.Split('|');
                                                    if (parts.Length == 2)
                                                    {
                                                        apId = parts[0];
                                                        apHash = parts[1];
                                                    }
                                                }
                                            }
                                            string pathSession = Path.Combine(Environment.CurrentDirectory, $"Data\\DataImport\\Admin\\Session\\{account.PhoneNumber}.session");
                                            int numAp = 0;
                                            if (int.TryParse(apId, out numAp))
                                            {
                                                if (File.Exists(pathSession))
                                                {
                                                    File.Delete(pathSession);
                                                }
                                                try
                                                {
                                                    Client client = new Client(numAp, apHash, pathSession);
                                                    if (rdoProxy.Checked && !string.IsNullOrEmpty(selectAccount.Proxy))
                                                    {
                                                        client.TcpHandler = async (address, port) =>
                                                        {
                                                            var proxy = xNet.HttpProxyClient.Parse(selectAccount.Proxy);
                                                            return proxy.CreateConnection(address, port);
                                                        };
                                                    }
                                                    var verification_code = await _telegram.LoginApiTelegram(device.IndexLDPlayer, account.PhoneNumber, client);
                                                    if (verification_code == "verification_code")
                                                    {
                                                        var verifiCode = await _telegram.GetCodeLDPlayerTelegram(device.IndexLDPlayer, device.Data, device.AdbClient);
                                                        if (!string.IsNullOrEmpty(verifiCode) && int.TryParse(verifiCode, out int input))
                                                        {
                                                            if (await _telegram.AuthorApiTelegram(device.IndexLDPlayer, verifiCode, client))
                                                            {
                                                                account.Session = pathSession;
                                                                account.ApId = apId;
                                                                account.ApHash = apHash;
                                                                _accountRepository.Update(account);
                                                                loadAccount();
                                                                RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {device.IndexLDPlayer}  verification_code: {verifiCode}", 0);
                                                                LabaleHelper.WriteLabale(device.View.StatusLabel, $"  verification_code: {verifiCode}", 0);
                                                                if (await _telegram.ImportUsername(device.IndexLDPlayer, selectAccount.Username, client) && selectAccount.Username != null)
                                                                {
                                                                    account.UserName = selectAccount.Username;
                                                                    _accountRepository.Update(account);
                                                                    loadAccount();
                                                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {device.IndexLDPlayer}   Import Username: {selectAccount.Username}", 0);
                                                                    LabaleHelper.WriteLabale(device.View.StatusLabel, $"Import Username: {selectAccount.Username}", 0);
                                                                }
                                                                if (cbUpdateAvatar.Checked && File.Exists(selectAccount.FileImage))
                                                                {
                                                                    if (await _telegram.ImportAvatar(device.IndexLDPlayer, selectAccount.FileImage, client))
                                                                    {
                                                                        account.Avatar = selectAccount.FileImage;
                                                                        _accountRepository.Update(account);
                                                                        loadAccount();
                                                                        RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {device.IndexLDPlayer}  Update Avatar: {selectAccount.FileImage}", 0);
                                                                        LabaleHelper.WriteLabale(device.View.StatusLabel, $"Update Avatar: {selectAccount.FileImage}", 0);
                                                                    }
                                                                }
                                                                if (await _telegram.ImportPassword(device.IndexLDPlayer, selectAccount.Password, client, device.Data, device.AdbClient) && selectAccount.Password != null)
                                                                {
                                                                    account.Password = selectAccount.Password;
                                                                    _accountRepository.Update(account);
                                                                    loadAccount();
                                                                    RichTextBoxHelper.WriteLogRichTextBox($"LDPlayer {device.IndexLDPlayer}  Import Password: {selectAccount.Password}", 0);
                                                                    LabaleHelper.WriteLabale(device.View.StatusLabel, $"Import Password: {selectAccount.Password}", 0);
                                                                }
                                                                string message = $"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss\t")}    {account.PhoneNumber}|{account.Password}|{account.UserName}|{account.FullName}|{account.Proxy}";
                                                                rtbResult.Invoke((MethodInvoker)delegate
                                                                {
                                                                    rtbResult.InvokeEx(s => s.AppendText(message, Color.Green, rtbResult.Font, true));
                                                                });
                                                            }
                                                        }
                                                    }
                                                    client.Dispose();
                                                }
                                                catch (Exception ex)
                                                {
                                                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                                                    Log.Error(ex, ex.Message);
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        selectAccount.IsUsing = false;
                                    }
                                }
                                else
                                {
                                    selectAccount.IsUsing = false;
                                }
                            }
                        }

                    }
                    writeLog(device, $" $Reboot", null, device.View.StatusLabel);
                    LDController.Close("index", device.IndexLDPlayer);
                    await LDController.DelayAsync(7).ConfigureAwait(false); ;
                    lock (_lockObject)
                    {
                        try
                        {
                            File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}\\data.vmdk");
                            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "LDPlayer\\data.zip"), $"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}");
                            Thread.Sleep(7000);
                        }
                        catch (Exception ex)
                        {
                            Thread.Sleep(3000);
                            File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}\\data.vmdk");
                            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "LDPlayer\\data.zip"), $"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}");
                        }
                    }
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (this.InvokeRequired) // Kiểm tra xem hiện tại có đang ở UI thread hay không
                        {
                            this.Invoke((Action)(() =>
                            {
                                LDController.Close("index", device.IndexLDPlayer);
                                tableLayoutPanel.Controls.Remove(device.View.Panel);
                                RepositionLDPlayers();

                            }));
                            return;
                        }
                        else
                        {
                            LDController.Close("index", device.IndexLDPlayer);
                            tableLayoutPanel.Controls.Remove(device.View.Panel);
                            RepositionLDPlayers();
                            return;
                        }
                    }
                    rebootLDPlayer(device, true); // reboot lại 
                }

            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private void rtbLogs_TextChanged(object sender, EventArgs e)
        {
            rtbLogs.SelectionStart = rtbLogs.Text.Length;
            rtbLogs.ScrollToCaret();
        }

        private void rtbResult_TextChanged(object sender, EventArgs e)
        {
            rtbResult.SelectionStart = rtbResult.Text.Length;
            rtbResult.ScrollToCaret();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            try
            {
                txtPathLDPlayers.Text = LDController.PathFolderLDPlayer;
                loadFileJsonService();
                loadAccount();
                //   loadDevice();
                //AdbServer adbServer = new AdbServer();
                //adbServer.StartServer($"{LDController.PathFolderLDPlayer}\\adb.exe", true);
                if (cbUpdateAvatar.Checked)
                {
                    txtFolderAvatar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
            }


        }
        private async void loadDevice()
        {
            try
            {
                var listLDPlayer = await DeviceHelper.GetLDplayersAnysc();
                dtgvLDPlayers.Invoke((MethodInvoker)delegate
                {
                    if (listLDPlayer.Count > 0)
                    {
                        // Clear existing rows
                        dtgvLDPlayers.Rows.Clear();
                        foreach (var device in listLDPlayer)
                        {
                            if (int.Parse(device.Index) < 999)
                            {
                                // Add rows with indexes 1, 5, and 8
                                dtgvLDPlayers.Rows.Add(device.Index, device.Name, device.DeviceId, device.Status);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }
        private void loadFileJsonService()
        {
            CountryCodeHelper cn = new CountryCodeHelper();
            listCountryCode = cn.GetCountryCodes();
            foreach (var code in listCountryCode)
            {
                listItem.Add(new Helper.ComboBoxItem
                {
                    Value = code.Dial_code,
                    DisplayText = code.Name + " " + code.Dial_code
                });
            }
            CBServiceCode.DataSource = listItem;
            var t = jsonHelper.GetValuesFromInputString("PhoneCode");
            if (!string.IsNullOrEmpty(t))
            {
                var countryCodeSelected = listCountryCode.FirstOrDefault(s => s.Dial_code == t);
                if (countryCodeSelected != null)
                {
                    var index = listCountryCode.IndexOf(countryCodeSelected);
                    CBServiceCode.SelectedIndex = index;
                }
            }
            if (CBoxOtpService.Text == "viotp.com" || CBoxOtpService.Text == "chothuesimcode.com")
            {
                panel7.Visible = false;
            }
            else
            {
                panel7.Visible = true;
            }
        }
        private async void dtgvAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dtgvAccount.Columns["cGetCode"].Index)
            {
                DataGridViewHelper.SetCellValue(dtgvAccount, e.RowIndex, "cStatus", $"Running");
                var phone = DataGridViewHelper.GetCellValue(dtgvAccount, e.RowIndex, "cPhone");
                var proxy = DataGridViewHelper.GetCellValue(dtgvAccount, e.RowIndex, "cProxy");
                var apId = DataGridViewHelper.GetCellValue(dtgvAccount, e.RowIndex, "cApId");
                var apHash = DataGridViewHelper.GetCellValue(dtgvAccount, e.RowIndex, "cApHash");

                var fileSession = DataGridViewHelper.GetCellValue(dtgvAccount, e.RowIndex, "cSession");
                if (!string.IsNullOrEmpty(phone) && !string.IsNullOrWhiteSpace(fileSession))
                {
                    var code = await getCodeLogin(apId, apHash, phone, proxy, fileSession);
                    if (!string.IsNullOrEmpty(code))
                    {
                        DataGridViewHelper.SetCellValue(dtgvAccount, e.RowIndex, "cStatus", $"Code Login: {code}", 2);
                        return;
                    }
                    else
                    {
                        MessageCommon.ShowMessageBox("something went wrong, Plea try again", 3);
                        return;
                    }

                }
                else
                {
                    MessageCommon.ShowMessageBox("Missing Data", 3);
                }

            }
        }
        private async Task<string> getCodeLogin(string apId, string apHash, string phone, string? addProxy, string fileSession)
        {
            int apIdnum;
            if (int.TryParse(apId, out apIdnum))
            {
                Client client = new Client(apIdnum, apHash, fileSession);
                try
                {

                    if (!string.IsNullOrEmpty(addProxy))
                    {
                        client.TcpHandler = async (address, port) =>
                        {
                            var proxy = xNet.HttpProxyClient.Parse(addProxy);
                            return proxy.CreateConnection(address, port);
                        };
                    }
                    var a = await client.Login(phone);
                    var dialogs = await client.Messages_GetAllDialogs(null);
                    for (int j = 0; j < dialogs.TotalCount; j++)
                    {
                        if (dialogs.Messages[j].Peer.ID == 777000)
                        {
                            var sms = (TL.Message)dialogs.messages[j];
                            string body = sms.message;
                            string[] line = body.Split(':', '.', '\n', '\t');
                            string code = line[1];
                            client.Dispose();
                            return code;
                        }
                    }
                    client.Dispose();
                    return null;
                }
                catch (Exception ex)
                {
                    client.Dispose();
                    Log.Error(ex, ex.Message);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    return null;
                }
            }
            return null;
        }
        private void btnLastName_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("Last Name", _listLastname);
            fAddFile.ShowDialog();
            _listLastname.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathLastName))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listLastname.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }

        private void btnAddFirstname_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("First Name", _listFirtname);
            fAddFile.ShowDialog();
            _listFirtname.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathFirstName))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listFirtname.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }
        private void radioRadomFullNameUs_CheckedChanged(object sender, EventArgs e)
        {
            plTenTuDat.Enabled = false;
        }

        private void radioRadomFullNameVN_CheckedChanged(object sender, EventArgs e)
        {
            plTenTuDat.Enabled = false;
        }

        private void radioCustomizeFullNameUs_CheckedChanged(object sender, EventArgs e)
        {
            plTenTuDat.Enabled = true;
        }

        private void radioRandomPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.Enabled = false;
            NumberPass.Enabled = true;
        }

        private void radioCustomizePass_CheckedChanged(object sender, EventArgs e)
        {
            NumberPass.Enabled = false;
            txtPass.Enabled = true;
        }
        string phoneCode;
        private void CBServiceCode_SelectedValueChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(phoneCode))
            {
                phoneCode = CBServiceCode.SelectedValue.ToString();
                SettingsTool.GetSettings("configGeneral").AddValue("PhoneCode", phoneCode);
                SettingsTool.UpdateSetting("configGeneral");
                if (CBoxOtpService.Text == ApiService.Web5Sim)
                {

                    var line = listCountryCode.Where(x => x.Dial_code == phoneCode).FirstOrDefault().alias;
                    if (!string.IsNullOrEmpty(line))
                    {
                        GlobalModels.Contry = line;
                    }
                    else
                    {
                        MessageCommon.ShowMessageBox(" No Support Country", 3);
                        return;
                    }

                }
                if (CBoxOtpService.Text == ApiService.Sms365 || CBoxOtpService.Text.Contains(ApiService.getsmscode))
                {
                    var line = listCountryCode.Where(x => x.Dial_code == phoneCode).FirstOrDefault().countryId;
                    if (!string.IsNullOrEmpty(line))
                    {
                        GlobalModels.Contry = line;
                    }
                    else
                    {
                        MessageCommon.ShowMessageBox(" No Support Country", 3);
                        return;
                    }
                }
                SettingsTool.GetSettings("configGeneral").AddValue("Contry", GlobalModels.Contry);
                SettingsTool.UpdateSetting("configGeneral");
            }
        }

        private void CBoxOtpService_SelectedIndexChanged(object sender, EventArgs e)
        {
            var server = CBServiceCode.SelectedValue;
            SettingsTool.GetSettings("configGeneral").AddValue("PhoneCode", CBServiceCode.SelectedValue.ToString());
            SettingsTool.UpdateSetting("configGeneral");
            phoneCode = jsonHelper.GetValuesFromInputString("PhoneCode");
        }
        private void CBoxOtpService_TextChanged(object sender, EventArgs e)
        {
            if (CBoxOtpService.Text == "viotp.com" || CBoxOtpService.Text == "chothuesimcode.com")
            {
                panel7.Visible = false;
            }
            else
            {
                panel7.Visible = true;
            }
            SettingsTool.GetSettings("configGeneral").AddValue("Service", CBoxOtpService.Text);
            SettingsTool.UpdateSetting("configGeneral");
        }
        private async void btnLoadDevices_Click(object sender, EventArgs e)
        {

        }

        private void btnSettingUserName_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("User Name", _listUsername);
            fAddFile.ShowDialog();
            _listUsername.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathUserName))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listUsername.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }
        private async void btnStop_Click(object sender, EventArgs e)
        {
            AdbClient adbClient = new AdbClient();
            var device = adbClient.GetDevices().FirstOrDefault();
            string phone = "+573145437837";
            string proxy = "38.111.111.170:20473";
            string hash = string.Empty; string user = FileHelper.GetUserAgents();
            string apId = "28066177"; string apHash = "5ecf08b7b2a7aff9f355abac0c73d450";
            string cookie = string.Empty;

            //TelegramServices telegramServices = new TelegramServices(phone);
            //hash = telegramServices.GetRandomHash(proxy);
            //if (!string.IsNullOrEmpty(hash))
            //{
            //    var codeApi = await _telegram.GetCodeLoginAPILDPlayerTelegram(device, adbClient);
            //    if (!string.IsNullOrEmpty(codeApi) && !string.IsNullOrEmpty(hash))
            //    {
            //        while (true)
            //        {
            //            var s = telegramServices.GetApiIdAndHashId(hash, codeApi, proxy);
            //        }
               
            //    }
            //}

            TelegramAPI telegramAPI = new TelegramAPI();
            int i = 5;
            while (i > 0)
            {
                i--;
                hash = await telegramAPI.LoginPhone(phone, user, proxy);
                if (!string.IsNullOrEmpty(hash))
                {
                    var codeApi = await _telegram.GetCodeLoginAPILDPlayerTelegram(device, adbClient);
                    if (!string.IsNullOrEmpty(codeApi))
                    {
                        cookie = await telegramAPI.GetCookie(phone, user, hash, codeApi, proxy);
                        if (!string.IsNullOrEmpty(cookie))
                        {
                            i = 0;
                            break;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(cookie))
            {
                var result = await telegramAPI.GetHash(cookie, user, proxy);
                var resultApIdandApHash = await telegramAPI.GetApiIdAndHash(cookie, result, proxy, user);
                if (!string.IsNullOrEmpty(resultApIdandApHash))
                {
                    var parts = resultApIdandApHash.Split('|');
                    if (parts.Length == 2)
                    {
                        apId = parts[0];
                        apHash = parts[1];
                    }
                }
            }
            string pathSession = Path.Combine(Environment.CurrentDirectory, $"Data\\DataImport\\Admin\\Session\\{phone}.session");
            int numAp = 0;
            if (int.TryParse(apId, out numAp))
            {
                if (File.Exists(pathSession))
                {
                    File.Delete(pathSession);
                }
                try
                {
                    Client client = new Client(numAp, apHash, pathSession);
                    if (rdoProxy.Checked && !string.IsNullOrEmpty(proxy))
                    {
                        client.TcpHandler = async (address, port) =>
                        {
                            var proxyUrl = xNet.HttpProxyClient.Parse(proxy);
                            return proxyUrl.CreateConnection(address, port);
                        };
                    }
                    var verification_code = await _telegram.LoginApiTelegram("0", phone, client);
                    if (verification_code == "verification_code")
                    {
                        var verifiCode = await _telegram.GetCodeLDPlayerTelegram("0", device, adbClient);
                        if (!string.IsNullOrEmpty(verifiCode) && int.TryParse(verifiCode, out int input))
                        {
                            if (await _telegram.AuthorApiTelegram("0", verifiCode, client))
                            {

                            }
                        }
                    }
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }

            }









            cancellationTokenSource?.Cancel();
            profileModels.Clear();

            GlobalModels.Devices.Clear();
            tableLayoutPanel.Controls.Clear();
        }
        private async void btnSetup_Click(object sender, EventArgs e)
        {
            btnSetup.Enabled = false;
            var ldplayer = LDController.GetDevices2().FirstOrDefault();
            DeviceInfo device = new DeviceInfo();
            device.IndexLDPlayer = ldplayer.index.ToString();
            device.AdbClient = new AdbClient();
            device.Data = new DeviceData();
            device.View = new ViewInfo();
            device.View.Embeddedpanel = new Panel();
            device.View.StatusLabel = new Label();
            device.View.LdplayerHandle = new IntPtr();
            device.View.Panel = new Panel();
            device.View.PanelButton = new Panel();
            device.View.BtnClose = new Button();
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel.AutoScroll = true;
            LDController.EditFileConfigLDPlayer();
            LDController.SettingLDPlayerByIndex(device.IndexLDPlayer, 4, 4096);
            await LDController.DelayAsync(1);
            bool isRunning = false;
            for (int i = 0; i < 10; i++)
            {
                LDController.Open("index", device.IndexLDPlayer);
                await LDController.DelayAsync(2);
                if (LDController.IsDevice_Running("index", device.IndexLDPlayer))
                {
                    if (columnCount >= int.Parse(btnDisplay.Text.Trim()))
                    {
                        rowCount++;
                        columnCount = 0;
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }
                    addViewControl(device, tableLayoutPanel);
                    writeLog(device, "Running", null, device.View.StatusLabel);
                    columnCount++;
                    isRunning = true;
                    break;
                }
            }
            if (isRunning)
            {
                cancellationTokenSource = new CancellationTokenSource();
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(async () =>
                {
                    await SetupLDPlayer(device);
                }, cancellationTokenSource.Token));
                await Task.WhenAll(tasks);
                btnSetup.Enabled = true;
                tableLayoutPanel.Controls.Clear();
                pDevice.Controls.Clear();
            }
        }
        private async Task SetupLDPlayer(DeviceInfo device)
        {
            string adbKeyboard = Path.Combine(Environment.CurrentDirectory, "App\\ADBKeyboard.apk");
            string vpnProxy = Path.Combine(Environment.CurrentDirectory, "App\\VAT-VpnProxy_v1.0.0.apk");
            string youtube = Path.Combine(Environment.CurrentDirectory, "App\\Telegram.apk");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                await LDController.DelayAsync().ConfigureAwait(false);
                if (DeviceHelper.Connect(device))
                {
                    writeLog(device, "Done Connect", null, device.View.StatusLabel);
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tDeviceId", device.Id, 2);
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tName", device.Data.Model, 2);
                    device.Status = "Connect";
                    sw.Stop();
                    break;
                }
                if (sw.ElapsedMilliseconds > 60000)
                {
                    writeLog(device, "Connect Fail", null, device.View.StatusLabel, 1);
                    device.Status = "reboot";
                    sw.Stop();
                    break;
                }
            }
            if (device.Status == "Connect")
            {
                sw.Start();
                while (true)
                {
                    LDController.InstallApp_File("index", device.IndexLDPlayer, adbKeyboard);
                    await LDController.DelayAsync(1);
                    LDController.InstallApp_File("index", device.IndexLDPlayer, vpnProxy);
                    await LDController.DelayAsync(1);
                    LDController.InstallApp_File("index", device.IndexLDPlayer, youtube);
                    await LDController.DelayAsync(15);
                    var cmdResutl = LDController.ADB("index", device.IndexLDPlayer, "shell pm list package");
                    if (!string.IsNullOrEmpty(cmdResutl) && cmdResutl.Contains("org.telegram.messenger") && cmdResutl.Contains("com.android.adbkeyboard") && cmdResutl.Contains("com.vat.vpn"))
                    {
                        writeLog(device, "Done InstallApp", null, device.View.StatusLabel, 2);
                        device.Status = "Done InstallApp";
                        sw.Stop();
                        break;
                    }
                    if (sw.ElapsedMilliseconds > 250000)
                    {
                        writeLog(device, "Fail InstallApp", null, device.View.StatusLabel, 1);
                        device.Status = "reboot";
                        sw.Stop();
                        break;
                    }
                }

            }
            if (device.Status == "Done InstallApp")
            {
                writeLog(device, "Setting", null, device.View.StatusLabel);
                await ChangeProxy(device.IndexLDPlayer, device.Id, "127.0.0.1.256", "8080", "", "");
                if (BackupDataLDplayer($"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}\\data.vmdk", Path.Combine(Environment.CurrentDirectory, $"LDplayer")))
                {
                    writeLog(device, "Done", null, device.View.StatusLabel, 2);
                    LDController.Close("index", device.IndexLDPlayer);
                    return;
                }
            }
            if (device.Status == "reboot")
            {
                writeLog(device, "Error", null, device.View.StatusLabel, 1);
                LDController.Close("index", device.IndexLDPlayer);
                return;
            }
        }
        private bool BackupDataLDplayer(string fileData, string foler)
        {
            try
            {
                // Tạo thư mục đích nếu nó không tồn tại
                if (!Directory.Exists(foler))
                {
                    Directory.CreateDirectory(foler);
                }

                // Tạo đường dẫn đến tệp Zip
                string zipFilePath = Path.Combine(foler, "data.zip");

                // Sao chép tệp nguồn đến thư mục đích
                string destinationFilePath = Path.Combine(foler, Path.GetFileName(fileData));
                File.Copy(fileData, destinationFilePath, true);

                // Tạo một FileStream để ghi dữ liệu vào tệp Zip
                using (FileStream fs = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (ZipOutputStream zipStream = new ZipOutputStream(fs))
                    {
                        // Tạo một đối tượng ZipEntry cho tệp bạn muốn nén
                        ZipEntry entry = new ZipEntry(Path.GetFileName(destinationFilePath));
                        zipStream.PutNextEntry(entry);

                        // Đọc dữ liệu từ tệp nguồn và sao chép vào tệp Zip
                        byte[] buffer = new byte[4096];
                        using (FileStream sourceStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read))
                        {
                            int bytesRead;
                            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                zipStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                // Xoá tệp nguồn ở thư mục chỉ định
                File.Delete(destinationFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR: {nameof(fMain)}, params; {nameof(BackupDataLDplayer)}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }

        }
        private void dtgvLDPlayers_BindingContextChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            if (null != gridView)
            {
                foreach (DataGridViewRow r in gridView.Rows)
                {
                    gridView.Rows[r.Index].HeaderCell.Value = (r.Index + 1).ToString();
                }
            }
        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            if (this.Width < originalFormWidth)
            {
                // Giới hạn việc thu kéo Form lại không vượt quá Width ban đầu của Form
                this.Width = originalFormWidth;
            }
        }

        private void btnSettingProxy_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("Proxy", _listProxy);
            fAddFile.ShowDialog();
            _listProxy.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathProxy))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listProxy.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }
        private void cbUpdateAvatar_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUpdateAvatar.Checked)
            {
                txtFolderAvatar.Enabled = true;
            }
            else
            {
                txtFolderAvatar.Enabled = false;
            }
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformAction("All");
        }

        private void selectAllHighlightedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformAction("SelectHighline");
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformAction("UnAll");
        }

        private void ExportFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                WriteDataGridViewToFile(saveFileDialog.FileName, dtgvAccount);
            }
        }
        private void WriteDataGridViewToFile(string filePath, DataGridView dataGridView)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string columnHeader = "";
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    if (!column.Visible || column.Index == 0 || column.Index == 1 || column.Index == dataGridView.Columns.Count - 1 || column.Index == dataGridView.Columns.Count - 2) continue;
                    if (!string.IsNullOrEmpty(columnHeader)) columnHeader += "|";
                    columnHeader += column.HeaderText;
                }
                writer.WriteLine(columnHeader);
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    string rowData = "";
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!cell.Visible || cell.ColumnIndex == 0 || cell.ColumnIndex == 1 || cell.ColumnIndex == row.Cells.Count - 1 || cell.ColumnIndex == row.Cells.Count - 2) continue;
                        if (!string.IsNullOrEmpty(rowData)) rowData += "|";
                        rowData += cell.Value != null ? cell.Value.ToString() : "";
                    }
                    writer.WriteLine(rowData);
                }
            }
        }
        private void deleteAccToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> list = GetListSelect();
                if (list.Count == 0)
                {
                    MessageCommon.ShowMessageBox("Please select the account to delete!", 4);
                    return;
                }
                if (MessageCommon.ShowConfirmationBox(string.Format("Do you want to delete the {0} selected accounts?", list.Count)) == DialogResult.No)
                {
                    return;
                }
                var check = _accountRepository.DeleteRange(list);
                if (check)
                {
                    MessageCommon.ShowMessageBox("Account deleted successfully!");
                    loadAccount();
                }
                else
                {
                    MessageCommon.ShowMessageBox("Delete failed, please try again later!", 4);
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }
        private void PerformAction(string action)
        {
            try
            {
                switch (action)
                {
                    case "ToggleCheck":
                        {
                            for (int k = 0; k < dtgvAccount.SelectedRows.Count; k++)
                            {
                                int index = dtgvAccount.SelectedRows[k].Index;
                                SetCellAccount(index, "ChooseCol", !Convert.ToBoolean(GetCellValue(index, "ChooseCol")));
                            }
                            //UpdateSelectedRowCount();
                            break;
                        }
                    case "SelectHighline":
                        {
                            DataGridViewSelectedRowCollection selectedRows = dtgvAccount.SelectedRows;
                            for (int j = 0; j < selectedRows.Count; j++)
                            {
                                SetCellAccount(selectedRows[j].Index, "ChooseCol", true);
                            }
                            //UpdateSelectedRowCount();
                            break;
                        }
                    case "UnAll":
                        {
                            for (int l = 0; l < dtgvAccount.RowCount; l++)
                            {
                                SetCellAccount(l, "ChooseCol", false);
                            }
                            //UpdateSelectedRowCount(0);
                            break;
                        }
                    case "All":
                        {
                            for (int i = 0; i < dtgvAccount.RowCount; i++)
                            {
                                SetCellAccount(i, "ChooseCol", true);
                            }
                            //UpdateSelectedRowCount(dtgvAcc.RowCount);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                Log.Error(ex, ex.Message);
            }

        }
        private void SetCellAccount(int rowIndex, string columnName, object cellValue, bool allowNull = true)
        {
            if (allowNull || !(cellValue.ToString().Trim() == ""))
            {
                DataGridViewHelper.SetCellValue(dtgvAccount, rowIndex, columnName, cellValue);
            }
        }
        private string GetCellValue(int rowIndex, string columnName)
        {
            return DataGridViewHelper.GetCellValue(dtgvAccount, rowIndex, columnName);
        }

        private void btnDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableLayoutPanel.Controls.Count > 0)
            {
                RepositionLDPlayers(int.Parse(btnDisplay.Text.Trim()));
            }

        }
        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageCommon.ShowConfirmationBox("Are you sure you want to close the software?");
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true; // Hủy việc đóng Form nếu người dùng không đồng ý
                }
                else
                {
                    Application.ExitThread();
                    Application.Exit();
                }
            }
        }
        private List<string> GetListSelect()
        {
            List<string> list = new List<string>();
            try
            {
                for (int i = 0; i < dtgvAccount.RowCount; i++)
                {
                    if (Convert.ToBoolean(dtgvAccount.Rows[i].Cells["ChooseCol"].Value))
                    {
                        list.Add(dtgvAccount.Rows[i].Cells["cIdAccount"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
            return list;
        }
        private void loadAccount()
        {
            try
            {
                lock (_lockObject)
                {
                    var accounts = _accountRepository.GetAll();
                    if (accounts.Count > 0)
                    {
                        dtgvAccount.Invoke((MethodInvoker)delegate
                        {
                            dtgvAccount.Rows.Clear();
                            int i = 1;
                            foreach (var a in accounts)
                            {
                                dtgvAccount.Rows.Add(false, i, a.PhoneNumber, a.Password, a.UserName, a.ApId, a.ApHash, a.FullName, a.Proxy, a.Session, a.Avatar, a.CreateDate, "", "Get Code", a.Id);
                                i++;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
            }
        }
        private void dtgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                try
                {
                    dtgvAccount.CurrentRow.Cells["ChooseCol"].Value = !Convert.ToBoolean(dtgvAccount.CurrentRow.Cells["ChooseCol"].Value);
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }
            }
        }

        private void dtgvAccount_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                try
                {
                    dtgvAccount.CurrentRow.Cells["ChooseCol"].Value = !Convert.ToBoolean(dtgvAccount.CurrentRow.Cells["ChooseCol"].Value);
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }
            }
        }
        public class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetParent(IntPtr hWnd);
        }
        private void addViewControl(DeviceInfo device, TableLayoutPanel tableLayoutPanel)
        {
            try
            {
                //IntPtr ldPlayerHandle = IntPtr.Zero; // Lưu trữ handle của cha trước khi gim
                string name = $"Qnibot{device.IndexLDPlayer}";
                LDController.ReName("index", device.IndexLDPlayer, name);
                bool result = true;
                while (result)
                {
                    Thread.Sleep(2000);
                    // Thử tìm LDPlayer với tên "name"
                    var procs = Process.GetProcessesByName("dnplayer");
                    foreach (var proc in procs)
                    {
                        if (proc.MainWindowTitle == name)
                        {
                            device.View.LdplayerHandle = proc.MainWindowHandle;

                            // Hiển thị LDPlayer trong panel nếu nó đã bị gỡ gim
                            if (!device.View.IsPinned && device.View.LdplayerHandle != IntPtr.Zero)
                            {
                                device.View.Panel.Size = new Size(200, 400);
                                device.View.Embeddedpanel.Size = new Size(200, 350);
                                device.View.Embeddedpanel.Location = new Point(0, 0);
                                device.View.Embeddedpanel.Enabled = false;
                                SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                                MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                                device.View.Panel.Controls.Add(device.View.Embeddedpanel);

                                device.View.PanelButton.Size = new Size(200, 50);
                                device.View.PanelButton.Location = new Point(0, device.View.Embeddedpanel.Height);

                                // Thêm Label cho mỗi LDPlayer
                                device.View.StatusLabel.ForeColor = Color.Green;
                                device.View.StatusLabel.Text = "Running";
                                device.View.StatusLabel.Dock = DockStyle.Bottom;
                                device.View.PanelButton.Controls.Add(device.View.StatusLabel);

                                // Thêm Button để gỡ gim hoặc gim lại
                                device.View.BtnClose.Text = device.View.IsPinned ? "Pin" : "Unpin";
                                device.View.BtnClose.Dock = DockStyle.Bottom;
                                PictureBox iconPictureBox = new PictureBox();
                                iconPictureBox.Size = new Size(16, 16); // Kích thước biểu tượng
                                iconPictureBox.Location = new Point(5, 5); // Vị trí của biểu tượng trong nút
                                iconPictureBox.Image = Properties.Resources.UnpinIcon; // Thay đổi thành biểu tượng của bạn
                                device.View.BtnClose.Click += (sender, args) =>
                                {
                                    device.View.IsPinned = !device.View.IsPinned;
                                    // Đảo ngược trạng thái của LDPlayer (gỡ gim hoặc gim lại)
                                    if (device.View.IsPinned)
                                    {
                                        // Gỡ gim LDPlayer
                                        device.View.Embeddedpanel.Invoke((Action)(() =>
                                        {
                                            SetParent(device.View.LdplayerHandle, device.View.originalParentHandle);
                                        }));
                                        //SetParent(device.View.LdplayerHandle, device.View.originalParentHandle); // Đặt lại cha của LDPlayer
                                        MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                                        iconPictureBox.Image = Properties.Resources.PinIcon;
                                    }
                                    else
                                    {
                                        // Lưu trữ cha hiện tại của LDPlayer và gỡ gim
                                        device.View.originalParentHandle = NativeMethods.GetParent(device.View.LdplayerHandle);
                                        device.View.Embeddedpanel.Invoke((Action)(() =>
                                        {
                                            SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                                        }));
                                        //SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                                        MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                                        iconPictureBox.Image = Properties.Resources.UnpinIcon;
                                    }
                                    device.View.BtnClose.Text = device.View.IsPinned ? "Pin" : "Unpin";
                                };
                                device.View.BtnClose.Controls.Add(iconPictureBox);
                                device.View.PanelButton.Controls.Add(device.View.BtnClose);
                                device.View.Panel.Controls.Add(device.View.PanelButton);
                                tableLayoutPanel.Controls.Add(device.View.Panel);
                                tableLayoutPanel.SetCellPosition(device.View.Panel, new TableLayoutPanelCellPosition(columnCount, rowCount));
                                pDevice.Controls.Add(tableLayoutPanel);
                                result = false;
                                break;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }
        private void RepositionLDPlayers(int isRow = 5)
        {
            try
            {
                tableLayoutPanel.Invoke((MethodInvoker)delegate
                {
                    columnCount = 0;
                    rowCount = 0;
                    foreach (Control control in tableLayoutPanel.Controls)
                    {
                        if (control is Panel embeddedPanel)
                        {
                            int column = tableLayoutPanel.GetColumn(embeddedPanel);
                            int row = tableLayoutPanel.GetRow(embeddedPanel);

                            if (columnCount >= isRow)
                            {
                                rowCount++;
                                columnCount = 0;
                            }

                            tableLayoutPanel.SetCellPosition(embeddedPanel, new TableLayoutPanelCellPosition(columnCount, rowCount));
                            columnCount++;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        public void EmbedLDPlayer(IntPtr ldPlayerHandle, IntPtr panelHandle)
        {
            try
            {
                SetParent(ldPlayerHandle, panelHandle);
                const int GWL_STYLE = -16;
                const int WS_VISIBLE = 0x10000000;
                const int WS_CHILD = 0x40000000;
                int style = GetWindowLong(ldPlayerHandle, GWL_STYLE);
                style = style & ~WS_VISIBLE;
                style = style | WS_CHILD;
                SetWindowLong(ldPlayerHandle, GWL_STYLE, new IntPtr(style));
                MoveWindow(ldPlayerHandle, 0, 0, Width, Height, true);
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private void rebootLDPlayer(DeviceInfo device, bool isChange = true)
        {
            try
            {
                bool result = true;
                while (result)
                {
                    LDController.Open("index", device.IndexLDPlayer, isChange);
                    Thread.Sleep(5000);
                    var name = $"Qnibot{device.IndexLDPlayer}";
                    LDController.ReName("index", device.IndexLDPlayer, name);
                    var proc1 = Process.GetProcessesByName("dnplayer");
                    Parallel.ForEach(proc1, proc =>
                    {
                        if (proc.MainWindowTitle == name)
                        {
                            Thread.Sleep(1000);
                            device.View.LdplayerHandle = proc.MainWindowHandle;
                            device.View.Embeddedpanel.Invoke((Action)(() =>
                            {
                                SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                            }));
                            MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                            Thread.Sleep(3000);
                            result = false;
                            device.View.IsPinned = false; // Hoặc true tùy thuộc vào trạng thái sau reboot
                            device.View.BtnClose.Text = device.View.IsPinned ? "Pin" : "Unpin";
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }

        private void rtbLogs_DoubleClick(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("Log.txt"))
            {
                if (!File.Exists("Log.txt"))
                {
                    File.WriteAllBytes("Log.txt", new byte[0]);
                }

                writer.Write(rtbLogs.Text);
                writer.Close();
            }
            string notepadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");
            Process.Start(notepadPath, Path.Combine("Log.txt"));
        }

        private void rtbResult_DoubleClick(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("Result.txt"))
            {
                if (!File.Exists("Result.txt"))
                {
                    File.WriteAllBytes("Result.txt", new byte[0]);
                }
                writer.Write(rtbResult.Text);
                writer.Close();
            }
            string notepadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");
            Process.Start(notepadPath, Path.Combine("Result.txt"));
        }

        private void CBServiceCode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnLoadDevice_Click(object sender, EventArgs e)
        {
            loadDevice();
        }

        private void dtgvLDPlayers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                try
                {
                    dtgvLDPlayers.CurrentRow.Cells["cChose"].Value = !Convert.ToBoolean(dtgvLDPlayers.CurrentRow.Cells["cChose"].Value);
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }
            }
        }
    }
}