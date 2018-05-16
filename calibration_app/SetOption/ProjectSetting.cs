using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace calibration_app.SetOption
{
    public class ProjectSetting
    {
        public const string XmlPath = "./Setting.xml";
       public static bool IsFirst()
       {
            return !File.Exists(XmlPath);
        }
    }
}
