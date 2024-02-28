using AdvancedSharpAdbClient;
using System.Xml;

namespace AppDesptop.TelegramCreator.ProxyDroid
{
    public class XmlHelper
    {
        public static string GetElementValue(string filePath, string elementName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Load tệp tin XML
            xmlDoc.Load(filePath);
            // Lấy nút gốc của tài liệu XML
            XmlNode root = xmlDoc.DocumentElement;
            XmlNode node = root.SelectSingleNode($"//*[@name='{elementName}']");
            if (node != null)
            {
                return node.InnerText;
            }
            return null;
        }
        public static bool GetBooleanElementValue(string filePath, string elementName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Load tệp tin XML
            xmlDoc.Load(filePath);
            // Lấy nút gốc của tài liệu XML
            XmlNode root = xmlDoc.DocumentElement;
            XmlNode node = root.SelectSingleNode($"//*[@name='{elementName}']");
            if (node != null && node.Attributes != null && node.Attributes["value"] != null)
            {
                if (bool.TryParse(node.Attributes["value"].Value, out bool value))
                {
                    return value;
                }
            }
            return false;
        }

        // Phương thức thiết lập giá trị của một phần tử trong tài liệu XML
        public static void SetElementValue(string filePath, string elementName, string newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Load tệp tin XML
            xmlDoc.Load(filePath);
            // Lấy nút gốc của tài liệu XML
            XmlNode root = xmlDoc.DocumentElement;
            XmlNode node = root.SelectSingleNode($"//*[@name='{elementName}']");
            if (node != null)
            {
                node.InnerText = newValue;
            }
            // Lưu thay đổi vào tệp tin XML
            xmlDoc.Save(filePath);
        }
        public static void SetBooleanElementValue(string filePath, string elementName, bool newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Load tệp tin XML
            xmlDoc.Load(filePath);
            // Lấy nút gốc của tài liệu XML
            XmlNode root = xmlDoc.DocumentElement;
            XmlNode node = root.SelectSingleNode($"//*[@name='{elementName}']");
            if (node != null && node.Attributes != null && node.Attributes["value"] != null)
            {
                node.Attributes["value"].Value = newValue.ToString().ToLower();
            }
            // Lưu thay đổi vào tệp tin XML
            xmlDoc.Save(filePath);
        }
    }
}
