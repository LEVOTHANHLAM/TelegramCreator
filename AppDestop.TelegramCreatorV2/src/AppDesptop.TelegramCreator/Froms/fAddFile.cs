using AppDesptop.TelegramCreator.Helper;
using Serilog;

namespace AppDesptop.TelegramCreator.Froms
{
    public partial class fAddFile : Form
    {
        private string _type { get; set; }
        internal List<string> _list = new List<string>();
        public fAddFile(string type, List<string> list)
        {
            _type = type;
            InitializeComponent();
            CommonMethods.WireUpMouseEvents(lblHeader, btnClose);
            lblHeader.Text = $"Import {type}";
            string str2 = "System.Collections.Generic.List`1[System.String]";
            if (list.Count > 0 && !list[0].Equals(str2))
            {
                rtbData.Lines = list.ToArray();
            }
            _list.AddRange(list);
            lblTotal.Text = rtbData.Lines.ToList().Count.ToString();
            switch (type)
            {
                case "Proxy":
                    {
                        lbNotifi.Text = "Format: address:port:username:password";
                        return;
                    }
                default:
                    {
                        lbNotifi.Text = "Format: each person is a line";
                        return;
                    }
            }
            
        }
        private async void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            try
            {
                switch (_type)
                {
                    case "Proxy":
                        {
                            _list.Clear();
                            int success = 0;
                            int error = 0;
                            List<Task> tasks = new List<Task>();
                            foreach (var item in rtbData.Lines)
                            {
                                tasks.Add(Task.Run(async () =>
                                {
                                    ProcessLineAsync(item, ref success, ref error);
                                }));
                            }
                            await Task.WhenAll(tasks);
                            lblSuccess.Text = success.ToString();
                            lblError.Text = error.ToString();

                            break;
                        }
                    default:
                        {
                            _list = rtbData.Text.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            lblSuccess.Text = _list.Count.ToString();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                btnSave.Enabled = true;
                return;
            }
            MessageCommon.ShowMessageBox("Successfully Import");
            btnSave.Enabled = true;
        }
        private void ProcessLineAsync(string line, ref int success, ref int error)
        {
            var parts = line.Split(':');
            try
            {
                if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]))
                {
                    string username = null;
                    string password = null;
                    if (parts.Length >= 4)
                    {
                        username = parts[2];
                        password = parts[3];
                    }
                    lock (_list)
                    {
                        _list.Add(line);
                        Interlocked.Increment(ref success);
                    }
                }
                else
                {
                    Interlocked.Increment(ref error);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Interlocked.Increment(ref error);
            }
        }
        public class ProcessingResult
        {
            public int SuccessCount { get; set; }
            public int ErrorCount { get; set; }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rtbData_TextChanged(object sender, EventArgs e)
        {
            List<string> listAccount = rtbData.Lines.ToList();
            listAccount = CommonMethods.RemoveEmptyLines(listAccount);
            lblTotal.Text = listAccount.Count.ToString();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
