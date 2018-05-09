using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.IO;

namespace calibration_app
{
    public class Setting
    {
        private static readonly string filePath = "/setting.xml";

        public static string FilePath => filePath;

        public static bool IsFirst()
        {
            bool isFirst = File.Exists(FilePath);
            return (!isFirst);
        }
        public static string ReadNodeFromXML(string filePath, string nodeName)
        {
            string result = "error";
            try
            {
                 //判断文件是否存在
                if (File.Exists(filePath) && nodeName != "")
                {
                    XmlDocument xmlDoc = new XmlDocument();//新建XML文件
                    xmlDoc.Load(filePath);//加载XML文件
 
                    XmlNode xm = xmlDoc.GetElementsByTagName(nodeName)[0];
                    result = xm.InnerText;

                }
                else
                {
                    result = "error";
                }
            }
            catch
            {
                result = "error";
            }
            return result;
        }
    }
}
