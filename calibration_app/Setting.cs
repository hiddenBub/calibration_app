using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.IO;

namespace calibration_app
{
    public static class Setting
    {
        private const string filePath = "/setting.xml";

        
        private static List<object> gather;
        public static List<object> Gather { get => gather; set => gather = value; }

        private static List<object> project;

        public static List<object> Project { get => project; set => project = value; }

        public static bool IsFirst()
        {
            bool isFirst = File.Exists(filePath);
            return (!isFirst);
        }

        static Setting ()
        {
            XmlToEntityList
        }
        

    }
}
