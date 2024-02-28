using Serilog;

namespace AppDesptop.TelegramCreator.Helper
{
    public class ProfileHelper
    {
        public static List<string> ReadFile(string path)
        {
            var list = new List<string>();
            try
            {
                var result = File.ReadAllLines(path);
                Log.Information("ReadFile " + path);
                for (int i = 0; i < result.Length; i++)
                {
                    string[] lines = result[i].Split(':', ';', ',', '|');
                    if (!string.IsNullOrEmpty(lines[0]))
                    {
                        list.Add(lines[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return list;
        }
    }
}
