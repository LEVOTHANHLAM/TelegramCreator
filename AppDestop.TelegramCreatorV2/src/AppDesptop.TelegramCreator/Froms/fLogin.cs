using AppDesptop.TelegramCreator.Helper;
using AppDesptop.TelegramCreator.Migrations;
using AppDestop.TelegramCreator.ApiQnibot;
using AutoUpdaterDotNET;
using LDPlayerAndADBController;
using LDPlayerAndADBController.ADBClient;
using Serilog;
using System.Diagnostics;
using System.Xml.Linq;

namespace AppDesptop.TelegramCreator.Forms
{
    public partial class fLogin : Form
    {
        private readonly fMain _fMain;
        private JsonHelper jsonHelper;
        public fLogin(fMain fMain)
        {
            InitializeComponent();
            string path = Path.Combine(Environment.CurrentDirectory, "settings\\Applicetion.json");
            jsonHelper = new JsonHelper(path, isJsonString: false);

            CommonMethods.WireUpMouseEvents(bunifuCustomLabel1, btnClose);
            _fMain = fMain;
            txtKey.Text = jsonHelper.GetValuesFromInputString("Key");
            txtLDPlayer.Text = jsonHelper.GetValuesFromInputString("LDPlayer");
            btnLogin.Text = "Login";
            lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
        }
        string updateUrl;
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (btnLogin.Text == "Login")
            {
                try
                {
                    if (string.IsNullOrEmpty(txtKey.Text.Trim()) || string.IsNullOrEmpty(txtLDPlayer.Text.Trim()))
                    {
                        MessageCommon.ShowMessageBox("Please import the required information above", 4);
                        return;
                    }
                    if (!File.Exists($"{txtLDPlayer.Text.Trim()}\\adb.exe"))
                    {
                        MessageCommon.ShowMessageBox("Please check the LDPlayer directory again", 4);
                        return;
                    }
                    HttpHelper httpHelper = new HttpHelper();
                    string hardwareId = httpHelper.GetHardwareId();
                    Constant.licenseKey = txtKey.Text.Trim();
                    var softwareId = Constant.SoftwareId;
                    var checkLicenseResult = await httpHelper.CheckLicense(Constant.licenseKey, hardwareId, softwareId);
                    if (checkLicenseResult.Data is true)
                    {
                        try
                        {
                            ADBHelper.PathFolderADB = txtLDPlayer.Text.Trim();
                            LDController.PathFolderLDPlayer = txtLDPlayer.Text.Trim();
                            SettingsTool.GetSettings("Applicetion").AddValue("Key", txtKey.Text.Trim());
                            SettingsTool.GetSettings("Applicetion").AddValue("LDPlayer", txtLDPlayer.Text.Trim());
                            SettingsTool.UpdateSetting("Applicetion");
                            _fMain.Show();
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            MessageCommon.ShowMessageBox(ex.Message, 4);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {

                            MessageCommon.ShowMessageBox(checkLicenseResult.Message, 3);
                            var ps = new ProcessStartInfo("https://qnibot.com/")
                            {
                                UseShellExecute = true,
                                Verb = "open"
                            };
                            Process.Start(ps);
                            return;

                        }
                        catch (Exception ex)
                        {
                            MessageCommon.ShowMessageBox(ex.Message, 4);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox($"Error: {ex.Message}", 4);
                }
            }
            else
            {
                try
                {
                    AutoUpdater.Start(updateUrl);
                    lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
                    return;
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    return;
                }
            }
        }
        private async void fLogin_Load(object sender, EventArgs e)
        {
            try
            {
                string version = jsonHelper.GetValuesFromInputString("Version");
                lbVersion.Text = $"vs {version}";
                HttpHelper httpHelper = new HttpHelper();
                var softwareId = Constant.SoftwareId;
                var checkLicenseResult = await httpHelper.CheckVersion(softwareId, version);
                if (checkLicenseResult.Data is false)
                {
                    btnLogin.Text = "Update";
                    updateUrl = checkLicenseResult.Message;
                    AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                    AutoUpdater.DownloadPath = "update";
                    lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox($"Error: {ex.Message}", 4);
                Log.Error(ex, ex.Message);
            }

        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            try
            {
                if (AutoUpdater.DownloadUpdate(args))
                {
                    btnLogin.Text = "Login";
                    lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
                    Application.Exit();

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}
