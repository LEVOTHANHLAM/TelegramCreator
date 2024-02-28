using AdvancedSharpAdbClient;
using LDPlayerAndADBController.ADBClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;

namespace LDPlayerAndADBController.AdbController
{
    public class ADBClientController
    {
        public static bool ClearTextElement(DeviceData deviceData, AdbClient adbClient, string xpath, int charCount, int timeout = 30)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(1));
                    if (element != null)
                    {
                        element.Click();
                        element.ClearInput(30);
                        return true;
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ADBClientController)}, params; {nameof(ClearTextElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        public static bool ClickElement(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(1));
                    if (element != null)
                    {
                        element.Click();
                        return true;
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ADBClientController)}, params; {nameof(ClickElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        public static bool ElementIsExist(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(3));
                    if (element != null)
                    {
                        return true;
                    }
                    Thread.Sleep(500);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ElementIsExist)}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
            return false;
        }

        public static Cords FindElement(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            Cords result = new Cords();
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(1));
                    if (element != null)
                    {
                        result = element.Cords;
                        return result;
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(FindElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
                return result;
            }
            return result;
        }

        public static bool FindElementAndClickCondition(DeviceData deviceData, AdbClient adbClient, string xpath, string condition, int timeout = 30)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    var findElements = adbClient.FindElements(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(1));
                    if (findElements != null && findElements.Any())
                    {
                        foreach (var element in findElements)
                        {
                            adbClient.Click(deviceData, element.Cords);
                            if (ElementIsExist(deviceData, adbClient, "//node[@" + condition + "]", 10) == true)
                            {
                                return true;
                            }
                            else
                            {
                                adbClient.BackBtn(deviceData);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(FindElementAndClickCondition)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }

            return false;
        }

        public static bool InputElement(DeviceData deviceData, AdbClient adbClient, string xpath, string text, int timeout = 30)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(1));
                    if (element != null)
                    {
                        element.Click();
                        element.ClearInput(30);
                        element.SendText(text);
                        return true;
                    }
                    Thread.Sleep(500);

                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ADBClientController)}, params; {nameof(InputElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }

        public static bool SwipeElement(DeviceData deviceData, AdbClient adbClient, string xpathFirst, string xpathSecond, int timeout)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    var start = adbClient.FindElement(deviceData, "//node[@" + xpathFirst + "]", TimeSpan.FromSeconds(1));
                    var end = adbClient.FindElement(deviceData, "//node[@" + xpathSecond + "]", TimeSpan.FromSeconds(1));
                    if (start != null && end != null)
                    {
                        adbClient.Swipe(deviceData, start, end, timeout);
                        return true;
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ADBClientController)}, params; {nameof(SwipeElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        public static List<Element> FindElements(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            List<Element> result = new List<Element>();
            try
            {
                for (int i = 0; i < timeout; i++)
                {
                    var elements = adbClient.FindElements(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(1));
                    result.Clear();
                    if (elements != null && elements.Any())
                    {
                        foreach (Element element in elements)
                        {
                            result.Add(element);
                        }
                        if (result.Count > 0)
                        {
                            return result;
                        }
                    }
                    Thread.Sleep(500);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(FindElements)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return result;
        }
        public static bool ClickButton(DeviceData deviceData, AdbClient adbClient, string name, int timeout = 30)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    var elements = adbClient.FindElements(deviceData, "//node[@class='android.widget.Button']", TimeSpan.FromSeconds(1));
                    if (elements != null && elements.Any())
                    {
                        foreach (Element element in elements)
                        {
                            if (element.Attributes.ContainsValue(name))
                            {
                                element.Click();
                                return true;
                            }
                        }
                    }
                    Thread.Sleep(500);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ClickButton)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
        }
        public static bool FindElementIsExistOrClickByClass(DeviceData deviceData, AdbClient adbClient, string name, string nameClass, int timeout = 30, bool isClick = false)
        {
            try
            {
                for (int j = 0; j < timeout; j++)
                {
                    var elements = adbClient.FindElements(deviceData, $"//node[@class='{nameClass}']", TimeSpan.FromSeconds(1));
                    if (elements != null && elements.Count() > 0)
                    {
                        foreach (Element element in elements)
                        {
                            if (element.Attributes.ContainsValue(name))
                            {
                                element.Click();
                                return true;
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ClickButton)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
        }
        public static XmlDocument GetDump(DeviceData deviceData, AdbClient adbClient)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    try
                    {
                        var dump = adbClient.DumpScreen(deviceData);
                        if (dump != null)
                        {
                            return dump;
                        }
                        else
                        {
                            ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                            ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                            if (!AdbServer.Instance.GetStatus().IsRunning)
                            {
                                AdbServer server = new AdbServer();
                                StartServerResult result = server.StartServer($"{LDController.PathFolderLDPlayer}\\adb.exe", false);
                                if (result != StartServerResult.Started)
                                {
                                }
                            }
                            adbClient.Connect(deviceData.Serial);
                        }
                    }
                    catch
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        if (!AdbServer.Instance.GetStatus().IsRunning)
                        {
                            AdbServer server = new AdbServer();
                            StartServerResult result = server.StartServer($"{LDController.PathFolderLDPlayer}\\adb.exe", false);
                            if (result != StartServerResult.Started)
                            {
                            }
                        }
                        adbClient.Connect(deviceData.Serial);
                    }

                    i++;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }

        }
        public static bool FindElementbyDump(XmlDocument doc, DeviceData device, AdbClient adbClient, string xpath = "hierarchy/node")
        {
            try
            {
                if (doc != null)
                {
                    var xmlNode = doc.InnerXml;
                    if (!string.IsNullOrEmpty(xmlNode))
                    {
                        return xmlNode.Contains(xpath);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }

        }
    }
}
