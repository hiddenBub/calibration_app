using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.IO;
using calibration_app.lib;

namespace calibration_app.SetOption
{
    public class Setting
    {
        protected const string filePath = "/setting.xml";

        protected string ParentNode = "root";

        private List<object> options;

        public List<object> Options { get => options; set => options = value; }

        public static bool IsFirst()
        {
            bool isFirst = File.Exists(filePath);
            return (!isFirst);
        }

        public void Save()
        {

        }
        public Setting ()
        {
            string ft = File.ReadAllText(filePath);
            XmlHelper<Setting> xml = 
            List<string> setting = XmlHelper<>.XmlToEntity(xml);

        }
        

    }
}
