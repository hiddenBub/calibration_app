using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calibration_app.SetOption
{
    class ColumnSetting
    {
        private string name;

        private static readonly List<string> dataType = new List<string> { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal" };

        private string type;

        private int frequency;

        private static readonly List<string> calculateMethod = new List<string> { "sum", "avg", "max", "min" };

        private string method;

       
        public string Name { get => name; set => name = value; }

        public static List<string> DataType => dataType;

        public string Type { get => type; set => type = value; }
        public int Frequency { get => frequency; set => frequency = value; }

        public static List<string> CalculateMethod => calculateMethod;

        public string Method { get => method; set => method = value; }
        

        public ColumnSetting ()
        {
            Name = "";
            Type = "double";
            Frequency = 60;
            Method = "avg";
        }

        /// <summary>
        /// 确定项目的构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        /// <param name="method"></param>
        public ColumnSetting(string name, string type, int frequency, string method)
        {
            Name = name;
            Type = type;
            Frequency = frequency;
            Method = method;
        }
        public ColumnSetting(string name, string type, int frequency)
        {
            Name = name;
            Type = type;
            Frequency = frequency;
            Method = "sum";
        }
        public ColumnSetting(string name, string type)
        {
            Name = name;
            Type = type;
            Frequency = 60;
            Method = "avg";
        }
        public ColumnSetting(string name)
        {
            Name = name;
            Type = "double";
            Frequency = 60;
            Method = "avg";
        }
    }
}
