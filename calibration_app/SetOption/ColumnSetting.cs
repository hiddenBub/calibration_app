using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calibration_app.SetOption
{
    public class ColumnSetting
    {
        private string name;

        private static readonly List<string> dataType = new List<string> { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal" };

        private string type;

        private int frequency;

        private static readonly List<string> calculateMethod = new List<string> { "sum", "avg", "max", "min" };

        private string method;

        private double oldAverageDeviation = 0;
        private double newAverageDeviation = 0;
        private double oldAverageAbsoluteDeviation = 0;
        private double newAverageAbsoluteDeviation = 0;
        private double oldSensitivity = 0;
        private double newSensitivity = 0;



        public string Name { get => name; set => name = value; }

        public static List<string> DataType => dataType;

        public string Type { get => type; set => type = value; }
        public int Frequency { get => frequency; set => frequency = value; }

        public static List<string> CalculateMethod => calculateMethod;

        public string Method { get => method; set => method = value; }
        public double NewSensitivity { get => newSensitivity; set => newSensitivity = value; }
        public double OldSensitivity { get => oldSensitivity; set => oldSensitivity = value; }
        public double OldAverageDeviation { get => oldAverageDeviation; set => oldAverageDeviation = value; }
        public double NewAverageDeviation { get => newAverageDeviation; set => newAverageDeviation = value; }
        public double OldAverageAbsoluteDeviation { get => oldAverageAbsoluteDeviation; set => oldAverageAbsoluteDeviation = value; }
        public double NewAverageAbsoluteDeviation { get => newAverageAbsoluteDeviation; set => newAverageAbsoluteDeviation = value; }

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
